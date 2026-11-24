using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class DepartamentosVM
    {
        public int Id { get; set; }
        public string ClaveDep { get; set; }

        public string NombreDep { get; set; }

        public string NombreEmcargado { get; set; }

        public EmpleadosVM EmpleadosVM {get; set;}
    }
}