using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.ViewModels
{
    public class PermisosVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Selecciones un empleado")]
        public int IdEmpleado { get; set; }
        [Required(ErrorMessage = "Seleccione el motivo")]
        public int IdTipoPermiso { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaGeneracion { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFinal { get; set; }
        public string NombreEmpleado { get; set; }
        public string EmpresaNombre { get; set; }
        public int VacacionesDisponibles { get; set; }
        public int DiasLaboralesSemana { get; set; }
        public int? CantidadHorasSolicitadas { get; set; }
        public int HorasDisponibles { get; set; }
        public List<Empresas> Empresas { get; set; }
       
        public List<Empleados> Empleados { get; set; }
       
        public List<SubEmpresas> SubEmpresas { get; set; }


        public List<MotivosAusencia> MotivosAusencia { get; set; }
    }
}