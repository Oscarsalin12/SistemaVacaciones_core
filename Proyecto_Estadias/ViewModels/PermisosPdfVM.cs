using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class PermisosPdfVM                                                          
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoPermiso { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaGeneracion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }

        public Empleados Empleados { get; set; }
        public int DiasDisponibles { get; set; }
        public SubEmpresas SubEmpresas { get; set; }
        public MotivosAusencia MotivosAusencia { get; set; }
        public int DiasLaboralesSemana { get; set; }
        public string Estado { get; set; }
        public int? CantidadHorasSolicitadas { get; set; }
        public int? HorasDisponibles { get; set; }
        //public List<SubEmpresasVM> SubEmpresasVM { get; set; }
        //public List<EmpleadosVM> EmpleadosVM { get; set; }
        //public List<MotivosAusenciaVM> MotivosAusenciaVM { get; set; }
    }
}