using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.Security
{
    public class ClaimManager: IClaimManager
    {
        private readonly IAuthServices authservices;
        
        public ClaimManager()
        {
            authservices = new AuthServices();
        }
        #region  creacion Usuario
        public ClaimsIdentity CreateIdentity(UsuariosVM usuariosVM, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuariosVM.Id.ToString()),
                new Claim(ClaimTypes.Name, usuariosVM.Usuario),
                new Claim("IdEmpresa", usuariosVM.UsuariosRolesVMs.First().IdEmpresa.ToString()),
                new Claim("IdEmpresaMadre",usuariosVM.UsuariosRolesVMs.First().IdEmpresa.ToString())

            };
            if (usuariosVM.UsuariosRolesVMs != null && usuariosVM.UsuariosRolesVMs.Any())
            {
                foreach(var item in usuariosVM.UsuariosRolesVMs)
                {
                    //Roles 
                    claims.Add(new Claim(ClaimTypes.Role, item.RolesVM.Nombre));

                    // Empresa asignada al Rol 
                    claims.Add(new Claim("EmpresaId", item.IdEmpresa.ToString()));
                    // Sub empresa asignada al rol
                    claims.Add(new Claim("EmpresaMadreId", item.IdEmpresa.ToString()));
                    // Rol + Empresa combinados
                    claims.Add(new Claim("RolEmpresa", $"{item.RolesVM.Nombre}:{item.IdEmpresa}"));
                    // Rol + SubmpresaCombinados 
                    claims.Add(new Claim("RolSubEmpresa", $"{item.RolesVM.Nombre}:{item.IdEmpresa}"));
                }
            }
            return new ClaimsIdentity(claims,DefaultAuthenticationTypes.ApplicationCookie);
        }

        #endregion


        #region inicio de sesion 

        public ActionResult SignIn(UsuariosVM usuariosVM, bool rememberMe, Controller ctx, string _returnurl)
        {
          
            string returnUrl = _returnurl;

            var identity = CreateIdentity(usuariosVM, rememberMe);
            var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignIn(new AuthenticationProperties()
            {
                IsPersistent = rememberMe
            }, identity);
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = ctx.Url.Action("Login", "Auth");
            }
            return new RedirectResult(returnUrl);
                  
        }

        public bool SignIn(UsuariosVM usuariosVM, bool rememberMe, string _returnurl)
        {
            Usuarios usuarios = authservices.login(usuariosVM.Usuario, usuariosVM.Contrasenia);
            if (usuarios != null)
            {
                var userRol = usuarios.UsuariosEmpresas.Select(r => new UsuariosRolVM
                {
                    IdRol = r.idRol,
                    IdUsuario = r.idUsuario,
                    IdEmpresa = r.idEmpresa,
                    IdEmpresaMadre = r.idEmpresa,
                    RolesVM = new RolVM
                    {
                        Nombre = r.Roles.Nombre,
                        Id = r.Roles.Id
                    }
                }).ToList();
                if (userRol != null)
                {
                    usuariosVM.Usuario = usuarios.Usuario.Trim();
                    usuariosVM.Contrasenia = usuarios.Contrasenia.Trim();
                    usuariosVM.UsuariosRolesVMs = userRol;
                    string returnUrl = _returnurl;
                    var identity = CreateIdentity(usuariosVM, rememberMe);
                    var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties()
                    {
                        IsPersistent = rememberMe
                    }, identity);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Cerrar sesion 

        public void SignOut()
        {
            var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
        #endregion

    }
}