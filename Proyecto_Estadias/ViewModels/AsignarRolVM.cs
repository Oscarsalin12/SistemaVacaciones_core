using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.ViewModels
{
    public class AsignarRolVM
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int IdEmpresa { get; set; }

        public List<SelectListItem> ListaRoles { get; set; }
        public List<SelectListItem> ListaEmpresas    { get; set; }
    }
}