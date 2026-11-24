using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class EmpleadoServices : IEmpleadosServices
    {
        private readonly SistemaVacacionesDbContext _db;

        public EmpleadoServices(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public void InsertarEmpleados(Empleado emp)
        {
            var fechaIngreso = emp.FechaIDeIngreso;
            var hoy = DateTime.Now;

            int anios = hoy.Year - fechaIngreso.Year;
            int meses = hoy.Month - fechaIngreso.Month;

            if (hoy.Day < fechaIngreso.Day)
                meses--;

            if (meses < 0)
            {
                anios--;
                meses += 12;
            }

            int diasVacaciones = CalcularDiasVacaciones(anios);

            emp.AniosTrabajados = anios;
            emp.MesesTrabajados = meses;
            emp.intDiasVacaciones = diasVacaciones;
            emp.DiasDisponibles = diasVacaciones;

            if (emp.DiasLaboralesPorSemana == 5)
                emp.HorasPorDia = 9;
            else if (emp.DiasLaboralesPorSemana == 6)
                emp.HorasPorDia = 8;
            else
                emp.HorasPorDia = 0;

            emp.Estado = "Activo";
            _db.Empleados.Add(emp);
            _db.SaveChanges();
        }

        private static int CalcularDiasVacaciones(int anios)
        {
            if (anios <= 0) return 0;
            if (anios == 1) return 12;
            if (anios == 2) return 14;
            if (anios == 3) return 16;
            if (anios == 4) return 18;
            if (anios == 5) return 20;
            if (anios >= 6 && anios <= 10) return 22;
            if (anios >= 11 && anios <= 15) return 24;
            if (anios >= 16 && anios <= 20) return 26;
            if (anios >= 21 && anios <= 25) return 28;
            if (anios >= 26 && anios <= 30) return 30;
            return 30;
        }

        public Empleado? ObtenerEmpleadoPorId(int id)
        {
            return _db.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.SubEmpresa)
                .FirstOrDefault(e => e.Id == id);
        }

        public Empleado? GetEmpleadoPorId(int id) => ObtenerEmpleadoPorId(id);

        public void ActualizarEmpleado(Empleado empleadoActualizado)
        {
            var empleado = _db.Empleados.Find(empleadoActualizado.Id);
            if (empleado != null)
            {
                empleado.NumEmpleado = empleadoActualizado.NumEmpleado;
                empleado.Nombre = empleadoActualizado.Nombre;
                empleado.FechaIDeIngreso = empleadoActualizado.FechaIDeIngreso;
                empleado.IdEmpresa = empleadoActualizado.IdEmpresa;
                empleado.IdDepartamento = empleadoActualizado.IdDepartamento;
                empleado.DiasLaboralesPorSemana = empleadoActualizado.DiasLaboralesPorSemana;
                empleado.UsuarioEmpleado = empleadoActualizado.UsuarioEmpleado;
                empleado.Estado = empleadoActualizado.Estado;

                _db.SaveChanges();
            }
        }

        public void ActualizarVacacionesYPermisos()
        {
            var empleados = _db.Empleados.ToList();
            var hoy = DateTime.Today;

            foreach (var emp in empleados)
            {
                var fechaIngreso = emp.FechaIDeIngreso;

                int anios = hoy.Year - fechaIngreso.Year;
                int meses = hoy.Month - fechaIngreso.Month;
                if (hoy.Day < fechaIngreso.Day) meses--;
                if (meses < 0) { anios--; meses += 12; }

                int diasBase = CalcularDiasVacaciones(anios);
                int diasLaborales = (emp.DiasLaboralesPorSemana >= 5 && emp.DiasLaboralesPorSemana <= 6)
                                    ? emp.DiasLaboralesPorSemana : 5;
                int horasPorDia = emp.HorasPorDia > 0 ? emp.HorasPorDia : 8;

                var permisos = _db.Permisos
                                 .Include(p => p.MotivoAusencia)
                                 .Where(p => p.IdEmpleado == emp.Id && p.Estado == "Aprobado")
                                 .OrderBy(p => p.FechaInicio)
                                 .ToList();

                int diasRestantes = diasBase;
                int horasRestantes = horasPorDia;

                foreach (var p in permisos)
                {
                    bool descuentaDias = p.MotivoAusencia?.BitDescuentoDias ?? false;
                    bool descuentaHoras = p.MotivoAusencia?.BitDescuentoHoras ?? false;

                    if (descuentaDias && !descuentaHoras)
                    {
                        int diasHabiles = 0;
                        for (var d = p.FechaInicio.Date; d <= p.FechaFinal.Date; d = d.AddDays(1))
                        {
                            if (diasLaborales == 5 && d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                                diasHabiles++;
                            else if (diasLaborales == 6 && d.DayOfWeek != DayOfWeek.Sunday)
                                diasHabiles++;
                        }

                        diasRestantes -= diasHabiles;
                        p.DiasDisponibles = Math.Max(0, diasRestantes);
                        p.HorasDisponibles = horasRestantes;
                    }
                    else if (descuentaHoras)
                    {
                        int horasSolicitadas = p.CantidadHorasSolicitadas;
                        if (horasSolicitadas > 0)
                        {
                            if (horasRestantes >= horasSolicitadas)
                            {
                                horasRestantes -= horasSolicitadas;
                            }
                            else
                            {
                                int horasFaltantes = horasSolicitadas - horasRestantes;
                                int diasADescontar = (int)Math.Ceiling((double)horasFaltantes / horasPorDia);

                                diasRestantes -= diasADescontar;
                                horasRestantes = (diasADescontar * horasPorDia) - horasFaltantes;
                            }
                        }

                        p.DiasDisponibles = Math.Max(0, diasRestantes);
                        p.HorasDisponibles = horasRestantes;
                    }
                    else
                    {
                        p.DiasDisponibles = Math.Max(0, diasRestantes);
                        p.HorasDisponibles = horasRestantes;
                    }
                }

                emp.AniosTrabajados = anios;
                emp.MesesTrabajados = meses;
                emp.intDiasVacaciones = diasBase;
                emp.DiasDisponibles = Math.Max(0, diasRestantes);
                emp.HorasRestantesDelDia = horasRestantes;
            }

            _db.SaveChanges();
        }

        public IQueryable<Empleado> FiltrarEmpleados(int idSub, string? estado, string? busqueda, int? idDepartamento)
        {
            var query = _db.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.SubEmpresa)
                .Where(e => e.IdEmpresa == idSub);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(e => e.Estado == estado);

            if (!string.IsNullOrEmpty(busqueda))
            {
                if (int.TryParse(busqueda, out int numEmp))
                    query = query.Where(e => e.NumEmpleado == numEmp);
                else
                    query = query.Where(e => e.Nombre.Contains(busqueda));
            }

            if (idDepartamento.HasValue && idDepartamento.Value > 0)
                query = query.Where(e => e.IdDepartamento == idDepartamento.Value);

            return query.OrderBy(e => e.Nombre);
        }

        public Empleado? LoginEmpleados(string usuario, int contrasena)
        {
            return _db.Empleados.FirstOrDefault(e => e.UsuarioEmpleado == usuario && e.NumEmpleado == contrasena);
        }
    }
}
