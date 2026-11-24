using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.ViewModels
{
    public class AsignarPermisosVM
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int IdEmpresa { get; set; }
        public List<int> idSubEmpresas { get; set; } // Para selección múltiple de subs

        public List<Roles> ListaRoles { get; set; }
        public List<Empresas> ListaEmpresas { get; set; }
        public List<SubEmpresas> ListaSubEmpresas { get; set; }
       
    }
}