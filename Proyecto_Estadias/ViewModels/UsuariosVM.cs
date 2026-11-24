using NLog.Web.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class UsuariosVM
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasenia { get; set; }

        public bool BitAdministrador { get; set; }
        public List<int> Roles { get; set; } = new List<int>();
        public List<int> Empresas { get; set; } = new List<int>();
       
        //public List<RolVM> RolVM { get; set; }
        //public List<EmpresaVM> empresaVM { get; set; }
        public List<UsuariosRolVM> UsuariosRolesVMs { get; set; } =new List<UsuariosRolVM>();
    }
}