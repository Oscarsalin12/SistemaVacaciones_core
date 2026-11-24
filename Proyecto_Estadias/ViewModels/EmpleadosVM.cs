
using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class EmpleadosVM
    {
        public int Id { get; set; }
        public int NumEmpleado { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaDeIngreso { get; set; }
        public int AniosTrabajados { get; set; }
        public int MesesTrabajados { get; set; }
        public int intDiasVacaciones { get; set; }
        public int IdEmpresa { get; set; }
        public int DiasDisponibles { get; set; }
        public int IdDepartamento { get; set; }
        public int DiasLaboralesPorSemana { get; set; } // 5 o 6
        public int HorasPorDia { get; set; }
        public int HorasRestantesDelDia { get; set; }
        public string NombreEmpresa { get; set; }
        public string NombreDepartamento { get; set; }
        public string Estado { get; set; } 

        public string UsuarioEmpleado { get; set; }
        public List<DepartamentosVM> DepatamentosVM { get; set; } 
        public List<SubEmpresasVM> SubEmpresaVM { get; set; }
        //public virtual Empresas Empresa { get; set; }
    }
}