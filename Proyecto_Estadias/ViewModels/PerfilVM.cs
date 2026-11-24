using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class PerfilVM
    {
        public Usuarios Usuario { get;set; }
       
        public Empleados Empleado { get;set; }
        public List<Empresas> Empresas { get; set; }
        public List<SubEmpresas> SubEmpresas { get; set; }
        public Dictionary<int, List<EmpleadosVM>> EmpleadosPorEmpresa {  get; set; }
    }
}