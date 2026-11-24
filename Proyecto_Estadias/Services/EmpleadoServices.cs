using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Services
{
    public class EmpleadoServices : IEmpleadosServices
    {
        private readonly Vacaciones_ProyectoEntities db = null;

        public EmpleadoServices()
        {
            db = new Vacaciones_ProyectoEntities();
        }

        #region metodo para insertar empleados

        public void InsertarEmpleados(Empleados emp)
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

            // ✅ Asignar horas por día según la jornada
            if (emp.DiasLaboralesPorSemana == 5)
                emp.HorasPorDia = 9;
            else if (emp.DiasLaboralesPorSemana == 6)
                emp.HorasPorDia = 8;
            else
                emp.HorasPorDia = 0; // Valor por defecto por si acaso
            emp.Estado = "Activo"; // Asignar estado por defecto
            db.Empleados.Add(emp);
            db.SaveChanges();
        }


        private int CalcularDiasVacaciones(int anios)
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

            return 30; // Por si excede los 30 años
        }

        #endregion

        #region metodo de actualizacion 
        public Empleados ObtenerEmpleadoPorId(int id)
        {
            return db.Empleados
                .Include("Departamentos")
                .Include("SubEmpresas")
                .ToList()
                .FirstOrDefault(e => e.Id == id);
        }



        public void ActualizarEmpleado(Empleados empleadoActualizado)
        {
            var empleado = db.Empleados.Find(empleadoActualizado.Id);
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

                db.SaveChanges();
            }
        }



        #endregion

        #region metodo para calculo de vaciones 
        public void ActualizarVacacionesYPermisos()
        {
            var empleados = db.Empleados.ToList();
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

                var permisos = db.Permisos
                                 .Include("MotivosAusencia")
                                 .Where(p => p.IdEmpleado == emp.Id && p.Estado == "Aprobado")
                                 .OrderBy(p => p.FechaInicio)
                                 .ToList();

                int diasTomados = 0;
                int totalHorasTomadas = 0;

                foreach (var permiso in permisos)
                {
                    if (permiso.MotivosAusencia.BitDescuentoHoras == true)
                    {
                        totalHorasTomadas += permiso.CantidadHorasSolicitadas;
                    }
                    else if (permiso.MotivosAusencia.BitDescuentoDias == true)
                    {
                        diasTomados += ContarDiasHabiles(permiso.FechaInicio, permiso.FechaFinal, diasLaborales);
                    }
                }

                // CORRECCIÓN MATEMÁTICA
                int diasPorHoras = totalHorasTomadas / horasPorDia;
                int horasSueltas = totalHorasTomadas % horasPorDia;

                // Sumamos los días exactos que se armaron con la acumulación de horas
                diasTomados += diasPorHoras;
                int horasRestantes = horasPorDia;

                // Si sobran horas sueltas (ej. tomó 5 horas), rompemos SOLO UN (1) día
                if (horasSueltas > 0)
                {
                    diasTomados += 1;
                    horasRestantes = horasPorDia - horasSueltas;
                }

                int diasDisponibles = Math.Max(diasBase - diasTomados, 0);

                emp.AniosTrabajados = anios;
                emp.MesesTrabajados = meses;
                emp.intDiasVacaciones = diasBase;
                emp.DiasDisponibles = diasDisponibles;
                emp.HorasRestantesDelDia = horasRestantes;
            }

            db.SaveChanges();
        }





        // Método que cuenta solo días hábiles (lunes a viernes)
        private int ContarDiasHabiles(DateTime fechaInicio, DateTime fechaFin, int diasLaboralesPorSemana = 5)
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

        public Empleados GetEmpleadoPorId(int id)
        {
            return db.Empleados.FirstOrDefault(e => e.Id == id);
        }

        #endregion
        
       public Empleados LoginEmpleados(string usuario, int numeroEmpleado)
        {
          
            if (string.IsNullOrWhiteSpace(usuario))
            return null; // O lanzar una excepción si prefieres
            var empleado = db.Empleados.FirstOrDefault(e => e.UsuarioEmpleado == usuario && e.NumEmpleado == numeroEmpleado);
                return empleado;
          
        }


        public IQueryable<Empleados> FiltrarEmpleados(int idSub, string estado, string busqueda, int? idDepartamento)
        {
            var query= db.Empleados
                .Include("Departamentos")
                .Include("SubEmpresas")
                .Where(e => e.IdEmpresa == idSub && e.Estado == estado);
            if(!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(e => e.Nombre.Contains(busqueda) || e.NumEmpleado.ToString().Contains(busqueda));
            }
            if(idDepartamento.HasValue && idDepartamento.Value > 0 )
            {
                query = query.Where(e => e.IdDepartamento == idDepartamento.Value);
            }

            return query;
        }
    }
}