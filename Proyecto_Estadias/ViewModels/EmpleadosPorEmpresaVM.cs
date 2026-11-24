using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.ViewModels
{
    public class EmpleadosPorEmpresaVM
    {
        public string Busqueda { get; set; }
        public int? IdSubEmpresa { get; set; }

        public List<SelectListItem> SubEmpresas { get; set; } = new List<SelectListItem>();
        public List<EmpleadosVM> Empleados { get; set; } = new List<EmpleadosVM>();
    }
}