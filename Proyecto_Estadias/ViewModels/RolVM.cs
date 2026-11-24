using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class RolVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool BitAdministrador { get; set; }
        public List<UsuariosVM> UsuariosVMs { get; set; }
    }
}