using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Math.EC.Rfc8032;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
namespace Proyecto_Estadias.Services
{
    public class PermisosService : IPermisosServices
    {
        private readonly Vacaciones_ProyectoEntities db = null;
        public PermisosService()
        {
            this.db = new Vacaciones_ProyectoEntities();
        }
        #region Listas auxiliares 
        public List<Permisos> PermisosAll()
        {
            return db.Permisos.Include("SubEmpresas").Include("Empleados").Include("MotivosAusencia").ToList();
        }

        public List<Empleados> ObtenerEmpleados()
        {
            return db.Empleados.OrderBy(e => e.Nombre).ToList();
        }

        public List<MotivosAusencia> ObtenerMotivos()
        {
            return db.MotivosAusencia.OrderBy(m => m.Id).ToList();
        }
    
        public List<SubEmpresas> ObtenerEmpresas()
        {
            return db.SubEmpresas.OrderBy(emp => emp.Id).ToList();
        }

        #endregion

        #region motodo de permisos
        public Empleados ObtenerEmpleadoPorId(int id)
        {
            var permiso = db.Permisos
            .Include("SubEmpresas.Empresas") // Muy importante para cargar Empresa dentro de SubEmpresas
            .Include("Empleados.Departamentos")
            .Include("MotivosAusencia")
            .Where(p => p.Estado == "Aprobado");
            return db.Empleados.FirstOrDefault(e => e.Id == id);

        }


        public Tuple<Empleados, List<Permisos>> ObtenerPermisosPorEmpleadosA(int idEmpleado)
        {
            var empleado = db.Empleados
                .Include("SubEmpresas.Empresas")
                .Include("Departamentos")
                .FirstOrDefault(e => e.Id == idEmpleado);

            var permisosAprobados = db.Permisos
                .Include("MotivosAusencia")
                .Where(p => p.IdEmpleado == idEmpleado && p.Estado == "Aprobado")
                .ToList();

            return Tuple.Create(empleado, permisosAprobados);
        }


        public bool CrearPermiso(PermisosVM model, out string error)
        {
            error = "";

            var empleado = db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);
            if (empleado == null) {error = "Empleado no encontrado";return false;}

            var tipoPermiso = db.MotivosAusencia.FirstOrDefault(m => m.Id == model.IdTipoPermiso);
            if (tipoPermiso == null){error = "Motivo de permiso no encontrado"; return false;}

            bool descuentaHoras = tipoPermiso.BitDescuentoHoras == true;
            bool descuentaDias = tipoPermiso.BitDescuentoDias == true;

            int horasSolicitadas = 0;
            int diasSolicitados = 0;

            if (descuentaHoras)
            {
                 horasSolicitadas = model.CantidadHorasSolicitadas ?? 0;
                if (horasSolicitadas <= 0) {error = "Debe ingresar la cantidad de horas a descontar."; return false;}

                int horasDisponibles = empleado.HorasRestantesDelDia;

                // Si las horas no alcanzan, intentamos convertir un día en horas
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

                else if(descuentaDias)
                {
                    diasSolicitados = ContarDiasHabiles(model.FechaInicio, model.FechaFinal, empleado.DiasLaboralesPorSemana);
                    if(diasSolicitados <= 0)
                    {
                        error = "El rango de fechas no contiene días hábiles válidos";
                        return false;
                    }
                    empleado.DiasDisponibles -= diasSolicitados;
                }

                var permiso = new Permisos
                {
                    IdEmpleado = model.IdEmpleado,
                    IdTipoPermiso = model.IdTipoPermiso,
                    IdEmpresa = model.IdEmpresa,
                    FechaGeneracion = DateTime.Now,
                    FechaInicio = model.FechaInicio,
                    FechaFinal = model.FechaFinal,
                    DiasDisponibles = empleado.DiasDisponibles,
                    CantidadHorasSolicitadas = descuentaHoras ? horasSolicitadas :0,
                    HorasDisponibles = empleado.HorasRestantesDelDia,
                    Estado = "Aprobado"
                };

                db.Permisos.Add(permiso);
                db.Entry(empleado).State = EntityState.Modified; // 👈 Importante
                db.SaveChanges();
                return true;
          }
        

        
        private int ContarDiasHabiles(DateTime fechaInicio, DateTime fechaFin, int diasLaboralesPorSemana)
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


        #endregion

        #region  listado de permisos
        public List<Permisos> ObtenerPermisosConDetalles()
        {
            return db.Permisos
                .Include("SubEmpresas")
                .Include("Empleados").
                Include("MotivosAusencia").ToList();
        }

        public Tuple<Empleados, List<Permisos>> ObtenerPermisosPorEmpleados(int idEmpleado)
        {
            var empleado = db.Empleados.Include("SubEmpresas").FirstOrDefault(e => e.Id == idEmpleado);
            var permisos = db.Permisos
                .Include("SubEmpresas")
                .Include("MotivosAusencia")
                .Where(p => p.IdEmpleado == idEmpleado)
                .ToList();

            return Tuple.Create(empleado, permisos);
        }

        public PermisosPdfVM ObtenerPermisoPorId(int idPermiso)
        {


            var permiso = db.Permisos
   .Include("SubEmpresas.Empresas") // Muy importante para cargar Empresa dentro de SubEmpresas
   .Include("Empleados.Departamentos")
   .Include("MotivosAusencia")
   .FirstOrDefault(p => p.Id == idPermiso);

            if (permiso == null) return null;

            var result = new PermisosPdfVM
            {
                Id = permiso.Id,
                IdEmpleado = permiso.IdEmpleado,
                IdTipoPermiso = permiso.IdTipoPermiso,
                IdEmpresa = permiso.IdEmpresa,
                FechaGeneracion = permiso.FechaGeneracion,
                FechaInicio = permiso.FechaInicio,
                FechaFinal = permiso.FechaFinal,
                Empleados = permiso.Empleados,
                SubEmpresas = permiso.SubEmpresas,
                MotivosAusencia = permiso.MotivosAusencia,
                DiasDisponibles = permiso.DiasDisponibles,
                CantidadHorasSolicitadas = permiso.CantidadHorasSolicitadas,
                HorasDisponibles = permiso.HorasDisponibles,

            };

            return result;
        }
        

        public List<Permisos> PermisosPorEmpresa(int idEmpresa)
        {
            return db.Permisos
                .Include("SubEmpresas")
                .Include("Empleados")
                .Include("MotivosAusencia")
                .Where(p => p.IdEmpresa == idEmpresa)
                .ToList();
        }

        #endregion

        public bool CrearSolicitud(PermisosVM model, out string error)
        {
            error = "";

            var empleado = db.Empleados.FirstOrDefault(e => e.Id == model.IdEmpleado);
            if (empleado == null) { error = "Empleado no encontrado"; return false; }

            var tipoPermiso = db.MotivosAusencia.FirstOrDefault(m => m.Id == model.IdTipoPermiso);
            if (tipoPermiso == null) { error = "Motivo de permiso no encontrado"; return false; }

            bool descuentaHoras = tipoPermiso.BitDescuentoHoras == true;
            bool descuentaDias = tipoPermiso.BitDescuentoDias == true;

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

            var permiso = new Permisos
            {
                IdEmpleado = model.IdEmpleado,
                IdTipoPermiso = model.IdTipoPermiso,
                IdEmpresa = model.IdEmpresa,
                FechaGeneracion = DateTime.Now,
                FechaInicio = model.FechaInicio,
                FechaFinal = model.FechaFinal,
                DiasDisponibles = empleado.DiasDisponibles, // Aún no descontado
                CantidadHorasSolicitadas = descuentaHoras ? (model.CantidadHorasSolicitadas ?? 0) : 0,
                HorasDisponibles = empleado.HorasRestantesDelDia, // Aún no descontado
                Estado = "Pendiente"
            };

            db.Permisos.Add(permiso);
            db.SaveChanges();
            return true;
        }



        public bool CambiarEstadoPermiso(int idPermiso, string nuevoEstado, out string error)
        {
            error = "";
            // IMPORTANTE: Incluimos MotivosAusencia para poder leer los Bits
            var permiso = db.Permisos.Include("Empleados").Include("MotivosAusencia").FirstOrDefault(p => p.Id == idPermiso);
            if (permiso == null) { error = "Permiso no encontrado"; return false; }

            if (nuevoEstado == "Aprobado")
            {
                bool descuentaHoras = permiso.MotivosAusencia.BitDescuentoHoras == true;
                bool descuentaDias = permiso.MotivosAusencia.BitDescuentoDias == true;

                if (descuentaHoras)
                {
                    int horasSolicitadas = permiso.CantidadHorasSolicitadas;
                    int horasDisponibles = permiso.Empleados.HorasRestantesDelDia;

                    if (horasDisponibles < horasSolicitadas)
                    {
                        if (permiso.Empleados.DiasDisponibles > 0)
                        {
                            permiso.Empleados.DiasDisponibles -= 1;
                            horasDisponibles += (permiso.Empleados.HorasPorDia > 0 ? permiso.Empleados.HorasPorDia : 8);
                        }
                        else { error = "El empleado ya no tiene días disponibles."; return false; }

                        if (horasDisponibles < horasSolicitadas) { error = "Aún convirtiendo un día, no alcanzan las horas."; return false; }
                    }
                    permiso.Empleados.HorasRestantesDelDia = horasDisponibles - horasSolicitadas;
                }
                else if (descuentaDias)
                {
                    int diasSolicitados = ContarDiasHabiles(permiso.FechaInicio, permiso.FechaFinal, permiso.Empleados.DiasLaboralesPorSemana);
                    if (permiso.Empleados.DiasDisponibles < diasSolicitados) { error = "El empleado no tiene suficientes días disponibles"; return false; }
                    permiso.Empleados.DiasDisponibles -= diasSolicitados;
                }

                // Actualizamos el registro histórico del permiso con los saldos que le quedaron
                permiso.DiasDisponibles = permiso.Empleados.DiasDisponibles;
                permiso.HorasDisponibles = permiso.Empleados.HorasRestantesDelDia;
            }

            permiso.Estado = nuevoEstado;
            db.SaveChanges();
            return true;
        }

        public List<Permisos> ObtenerPermisosPorEmpresa(int idEmpresa)
        {
            return db.Permisos
                .Include("SubEmpresas.Empresas")
                .Include("Empleados")
                .Include("MotivosAusencia")
                .Where(p => p.Estado == "Aprobado" && p.SubEmpresas.IdEmpresa_madre == idEmpresa)
                .ToList();
        }

        public List<Permisos> ObtenerPermisosPorEmpresaConFiltros(int idEmpresa, string nombre, int? departamento, int? motivo)
        {
            var query = db.Permisos
                .Include(p => p.Empleados.Departamentos)
                .Include(p => p.MotivosAusencia)
                .Include(p => p.SubEmpresas.Empresas)
                .Where(p => p.SubEmpresas.IdEmpresa_madre == idEmpresa && p.Estado == "Aprobado");

            if (!string.IsNullOrEmpty(nombre))
                query = query.Where(p => p.Empleados.Nombre.Contains(nombre));

            if (departamento.HasValue && departamento != 0)
                query = query.Where(p => p.Empleados.IdDepartamento == departamento.Value);

            if (motivo.HasValue && motivo != 0)
                query = query.Where(p => p.IdTipoPermiso == motivo.Value);

            return query.ToList();
        }



    }
}