using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class PermisosService : IPermisosServices
    {
        private readonly SistemaVacacionesDbContext _db;

        public PermisosService(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public List<Permiso> PermisosAll()
        {
            return _db.Permisos
                .Include(p => p.SubEmpresa)
                .Include(p => p.Empleado)
                .Include(p => p.MotivoAusencia)
                .ToList();
        }

        public List<Empleado> ObtenerEmpleados() => _db.Empleados.OrderBy(e => e.Nombre).ToList();

        public List<MotivoAusencia> ObtenerMotivos() => _db.MotivosAusencia.OrderBy(m => m.Id).ToList();

        public List<SubEmpresa> ObtenerEmpresas() => _db.SubEmpresas.OrderBy(emp => emp.Id).ToList();

        public Empleado? ObtenerEmpleadoPorId(int id)
        {
            return _db.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.SubEmpresa)
                    .ThenInclude(s => s!.Empresa)
                .FirstOrDefault(e => e.Id == id);
        }

        public Tuple<Empleado?, List<Permiso>> ObtenerPermisosPorEmpleadosA(int idEmpleado)
        {
            var empleado = _db.Empleados
                .Include(e => e.SubEmpresa)
                    .ThenInclude(s => s!.Empresa)
                .Include(e => e.Departamento)
                .FirstOrDefault(e => e.Id == idEmpleado);

            var permisosAprobados = _db.Permisos
                .Include(p => p.MotivoAusencia)
                .Where(p => p.IdEmpleado == idEmpleado && p.Estado == "Aprobado")
                .ToList();

            return Tuple.Create(empleado, permisosAprobados);
        }

        public bool CrearPermiso(PermisosVM model, out string error)
        {
            error = string.Empty;

            var empleado = _db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);
            if (empleado == null) { error = "Empleado no encontrado"; return false; }

            var tipoPermiso = _db.MotivosAusencia.FirstOrDefault(m => m.Id == model.IdTipoPermiso);
            if (tipoPermiso == null) { error = "Motivo de permiso no encontrado"; return false; }

            bool descuentaHoras = tipoPermiso.BitDescuentoHoras;
            bool descuentaDias = tipoPermiso.BitDescuentoDias;

            int horasSolicitadas = 0;
            int diasSolicitados;

            if (descuentaHoras)
            {
                horasSolicitadas = model.CantidadHorasSolicitadas ?? 0;
                if (horasSolicitadas <= 0) { error = "Debe ingresar la cantidad de horas a descontar."; return false; }

                int horasDisponibles = empleado.HorasRestantesDelDia;

                if (horasDisponibles < horasSolicitadas)
                {
                    if (empleado.DiasDisponibles > 0)
                    {
                        empleado.DiasDisponibles -= 1;
                        horasDisponibles += (empleado.HorasPorDia > 0 ? empleado.HorasPorDia : 8);
                    }
                    else { error = "No tiene días disponibles para convertir en horas."; return false; }

                    if (horasDisponibles < horasSolicitadas) { error = "Aún después de convertir un día, no tiene horas suficientes."; return false; }
                }
                empleado.HorasRestantesDelDia = horasDisponibles - horasSolicitadas;
            }
            else if (descuentaDias)
            {
                diasSolicitados = ContarDiasHabiles(model.FechaInicio, model.FechaFinal, empleado.DiasLaboralesPorSemana);
                if (diasSolicitados <= 0)
                {
                    error = "El rango de fechas no contiene días hábiles válidos";
                    return false;
                }
                empleado.DiasDisponibles -= diasSolicitados;
            }

            var permiso = new Permiso
            {
                IdEmpleado = model.IdEmpleado,
                IdTipoPermiso = model.IdTipoPermiso,
                IdEmpresa = model.IdEmpresa,
                FechaGeneracion = DateTime.Now,
                FechaInicio = model.FechaInicio,
                FechaFinal = model.FechaFinal,
                DiasDisponibles = empleado.DiasDisponibles,
                CantidadHorasSolicitadas = descuentaHoras ? horasSolicitadas : 0,
                HorasDisponibles = empleado.HorasRestantesDelDia,
                Estado = "Aprobado"
            };

            _db.Permisos.Add(permiso);
            _db.SaveChanges();
            return true;
        }

        private static int ContarDiasHabiles(DateTime fechaInicio, DateTime fechaFin, int diasLaboralesPorSemana)
        {
            int total = 0;
            for (var date = fechaInicio.Date; date <= fechaFin.Date; date = date.AddDays(1))
            {
                if (diasLaboralesPorSemana == 6)
                {
                    if (date.DayOfWeek != DayOfWeek.Sunday)
                        total++;
                }
                else
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        total++;
                }
            }
            return total;
        }

        public List<Permiso> ObtenerPermisosConDetalles()
        {
            return _db.Permisos
                .Include(p => p.SubEmpresa)
                .Include(p => p.Empleado)
                .Include(p => p.MotivoAusencia)
                .ToList();
        }

        public Tuple<Empleado?, List<Permiso>> ObtenerPermisosPorEmpleados(int idEmpleado)
        {
            var empleado = _db.Empleados.Include(e => e.SubEmpresa).FirstOrDefault(e => e.Id == idEmpleado);
            var permisos = _db.Permisos
                .Include(p => p.SubEmpresa)
                .Include(p => p.MotivoAusencia)
                .Where(p => p.IdEmpleado == idEmpleado)
                .ToList();

            return Tuple.Create(empleado, permisos);
        }

        public PermisosPdfVM? ObtenerPermisoPorId(int idPermiso)
        {
            var permiso = _db.Permisos
                .Include(p => p.SubEmpresa)
                    .ThenInclude(s => s!.Empresa)
                .Include(p => p.Empleado)
                    .ThenInclude(e => e!.Departamento)
                .Include(p => p.MotivoAusencia)
                .FirstOrDefault(p => p.Id == idPermiso);

            if (permiso == null) return null;

            return new PermisosPdfVM
            {
                Id = permiso.Id,
                IdEmpleado = permiso.IdEmpleado,
                IdTipoPermiso = permiso.IdTipoPermiso,
                IdEmpresa = permiso.IdEmpresa,
                FechaGeneracion = permiso.FechaGeneracion,
                FechaInicio = permiso.FechaInicio,
                FechaFinal = permiso.FechaFinal,
                Empleados = permiso.Empleado,
                SubEmpresas = permiso.SubEmpresa,
                MotivosAusencia = permiso.MotivoAusencia,
                DiasDisponibles = permiso.DiasDisponibles,
                CantidadHorasSolicitadas = permiso.CantidadHorasSolicitadas,
                HorasDisponibles = permiso.HorasDisponibles,
                Estado = permiso.Estado
            };
        }

        public List<Permiso> PermisosPorEmpresa(int idEmpresa)
        {
            return _db.Permisos
                .Include(p => p.SubEmpresa)
                .Include(p => p.Empleado)
                .Include(p => p.MotivoAusencia)
                .Where(p => p.IdEmpresa == idEmpresa)
                .ToList();
        }

        public bool CrearSolicitud(PermisosVM model, out string error)
        {
            error = string.Empty;

            var empleado = _db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);
            if (empleado == null) { error = "Empleado no encontrado"; return false; }

            var tipoPermiso = _db.MotivosAusencia.FirstOrDefault(m => m.Id == model.IdTipoPermiso);
            if (tipoPermiso == null) { error = "Motivo de permiso no encontrado"; return false; }

            bool descuentaHoras = tipoPermiso.BitDescuentoHoras;
            bool descuentaDias = tipoPermiso.BitDescuentoDias;

            if (descuentaHoras)
            {
                int horasSolicitadas = model.CantidadHorasSolicitadas ?? 0;
                if (horasSolicitadas <= 0) { error = "Debe ingresar la cantidad de horas a descontar."; return false; }

                int horasDisponibles = empleado.HorasRestantesDelDia;
                if (horasDisponibles < horasSolicitadas)
                {
                    if (empleado.DiasDisponibles > 0) { horasDisponibles += (empleado.HorasPorDia > 0 ? empleado.HorasPorDia : 8); }
                    else { error = "No tiene días disponibles para convertir en horas."; return false; }

                    if (horasDisponibles < horasSolicitadas) { error = "Aún después de convertir un día, no tiene horas suficientes."; return false; }
                }
            }
            else if (descuentaDias)
            {
                int diasSolicitados = ContarDiasHabiles(model.FechaInicio, model.FechaFinal, empleado.DiasLaboralesPorSemana);
                if (diasSolicitados <= 0) { error = "El rango de fechas no contiene días hábiles válidos"; return false; }
                if (empleado.DiasDisponibles < diasSolicitados) { error = "El empleado no tiene suficientes días disponibles"; return false; }
            }

            var permiso = new Permiso
            {
                IdEmpleado = model.IdEmpleado,
                IdTipoPermiso = model.IdTipoPermiso,
                IdEmpresa = model.IdEmpresa,
                FechaGeneracion = DateTime.Now,
                FechaInicio = model.FechaInicio,
                FechaFinal = model.FechaFinal,
                DiasDisponibles = empleado.DiasDisponibles,
                CantidadHorasSolicitadas = descuentaHoras ? (model.CantidadHorasSolicitadas ?? 0) : 0,
                HorasDisponibles = empleado.HorasRestantesDelDia,
                Estado = "Pendiente"
            };

            _db.Permisos.Add(permiso);
            _db.SaveChanges();
            return true;
        }

        public bool CambiarEstadoPermiso(int idPermiso, string nuevoEstado, out string error)
        {
            error = string.Empty;
            var permiso = _db.Permisos
                .Include(p => p.Empleado)
                .Include(p => p.MotivoAusencia)
                .FirstOrDefault(p => p.Id == idPermiso);

            if (permiso == null) { error = "Permiso no encontrado"; return false; }

            if (nuevoEstado == "Aprobado" && permiso.Empleado != null && permiso.MotivoAusencia != null)
            {
                bool descuentaHoras = permiso.MotivoAusencia.BitDescuentoHoras;
                bool descuentaDias = permiso.MotivoAusencia.BitDescuentoDias;

                if (descuentaHoras)
                {
                    int horasSolicitadas = permiso.CantidadHorasSolicitadas;
                    int horasDisponibles = permiso.Empleado.HorasRestantesDelDia;

                    if (horasDisponibles < horasSolicitadas)
                    {
                        if (permiso.Empleado.DiasDisponibles > 0)
                        {
                            permiso.Empleado.DiasDisponibles -= 1;
                            horasDisponibles += (permiso.Empleado.HorasPorDia > 0 ? permiso.Empleado.HorasPorDia : 8);
                        }
                        else { error = "El empleado ya no tiene días disponibles."; return false; }

                        if (horasDisponibles < horasSolicitadas) { error = "Aún convirtiendo un día, no alcanzan las horas."; return false; }
                    }
                    permiso.Empleado.HorasRestantesDelDia = horasDisponibles - horasSolicitadas;
                }
                else if (descuentaDias)
                {
                    int diasSolicitados = ContarDiasHabiles(permiso.FechaInicio, permiso.FechaFinal, permiso.Empleado.DiasLaboralesPorSemana);
                    if (permiso.Empleado.DiasDisponibles < diasSolicitados) { error = "El empleado no tiene suficientes días disponibles"; return false; }
                    permiso.Empleado.DiasDisponibles -= diasSolicitados;
                }

                permiso.DiasDisponibles = permiso.Empleado.DiasDisponibles;
                permiso.HorasDisponibles = permiso.Empleado.HorasRestantesDelDia;
            }

            permiso.Estado = nuevoEstado;
            _db.SaveChanges();
            return true;
        }

        public List<Permiso> ObtenerPermisosPorEmpresa(int idEmpresa)
        {
            return _db.Permisos
                .Include(p => p.SubEmpresa)
                    .ThenInclude(s => s!.Empresa)
                .Include(p => p.Empleado)
                .Include(p => p.MotivoAusencia)
                .Where(p => p.Estado == "Aprobado" && p.SubEmpresa != null && p.SubEmpresa.IdEmpresa_madre == idEmpresa)
                .ToList();
        }

        public List<Permiso> ObtenerPermisosPorEmpresaConFiltros(int idEmpresa, string? nombre, int? departamento, int? motivo)
        {
            var query = _db.Permisos
                .Include(p => p.Empleado)
                    .ThenInclude(e => e!.Departamento)
                .Include(p => p.MotivoAusencia)
                .Include(p => p.SubEmpresa)
                    .ThenInclude(s => s!.Empresa)
                .Where(p => p.SubEmpresa != null && p.SubEmpresa.IdEmpresa_madre == idEmpresa && p.Estado == "Aprobado");

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(p => p.Empleado != null && p.Empleado.Nombre.Contains(nombre));

            if (departamento.HasValue && departamento != 0)
                query = query.Where(p => p.Empleado != null && p.Empleado.IdDepartamento == departamento.Value);

            if (motivo.HasValue && motivo != 0)
                query = query.Where(p => p.IdTipoPermiso == motivo.Value);

            return query.ToList();
        }
    }
}
