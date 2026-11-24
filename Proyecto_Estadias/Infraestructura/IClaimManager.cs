using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.Infraestructura
{
    public interface IClaimManager
    {
        ClaimsIdentity CreateIdentity(UsuariosVM usuariosVM, bool rememberMe);
        ActionResult SignIn(UsuariosVM usuariosVM, bool rememberMe, Controller ctx, string _returnurl);
        void SignOut();
        bool SignIn(UsuariosVM usuariosVM, bool rememberMe, string _returnurl);
    }
}