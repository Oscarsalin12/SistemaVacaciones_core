
using PagedList;
using Proyecto_Estadias.Filters;
using Proyecto_Estadias.Helpers;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.Security;
using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Security;

namespace Proyecto_Estadias.Controllers
{
    public class AuthController : Controller
    {
        private readonly IEmpleadosServices empleadoServices;
        private readonly IAuthServices authServices;
        private readonly Vacaciones_ProyectoEntities db = new Vacaciones_ProyectoEntities();

        public AuthController()
        {
            authServices = new AuthServices();
            empleadoServices = new EmpleadoServices();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuariosVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // 1. BACKDOOR: Si el sistema está en ceros
                using (var db = new Proyecto_Estadias.Models.Vacaciones_ProyectoEntities())
                {
                    if (!db.Usuarios.Any()) // Verifica si NO hay ningún registro en la tabla Usuarios
                    {
                        // Credenciales maestras (puedes cambiarlas a las que tú quieras)
                        if (model.Usuario == "Admin" && model.Contrasenia == "Admin123")
                        {
                            FormsAuthentication.SetAuthCookie("Admin", false);
                            Session["UsuarioId"] = 0; // ID temporal de rescate
                            TempData["LoginExitoso"] = true;
                            TempData["mensaje"] = "Modo de rescate activo. Por favor, ve a Gestión de Accesos y crea el primer usuario.";
                            return Redirect(returnUrl ?? Url.Action("Perfil", "Auth"));
                        }
                    }
                }

                // 2. FLUJO NORMAL
                var usuario = authServices.login(model.Usuario, model.Contrasenia);
                if (usuario != null)
                {
                    // Sistema de cookies nativo de ASP.NET
                    FormsAuthentication.SetAuthCookie(usuario.Usuario, false);
                    Session["UsuarioId"] = usuario.Id;

                    TempData["LoginExitoso"] = true;
                    return Redirect(returnUrl ?? Url.Action("Perfil", "Auth"));
                }
                ModelState.AddModelError("", "Credenciales incorrectas.");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut(); // Adiós OWIN
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [Authorize]
        public ActionResult Perfil()
        {
            var usuarioNombre = User.Identity.Name;

            // 1. FLUJO SUPER ADMIN (No busca en la BD para evitar errores)
            if (usuarioNombre == "Admin")
            {
                var modelSA = new PerfilVM
                {
                    Usuario = new Proyecto_Estadias.Models.Usuarios { Usuario = "Admin" },
                    Empresas = authServices.GetAllEmpresas() ?? new List<Proyecto_Estadias.Models.Empresas>()
                };
                return View(modelSA);
            }

            // 2. FLUJO NORMAL
            var usuario = authServices.GetUsuarioConRelaciones(usuarioNombre);
            bool esAdminGlobal = authServices.EsAdmin(usuarioNombre);

            if (usuario == null && !esAdminGlobal) return RedirectToAction("Login");

            // Inicializamos el ViewModel
            var model = new PerfilVM
            {
                Usuario = usuario,
                Empresas = new List<Proyecto_Estadias.Models.Empresas>() // Evita el NullReferenceException
            };

            if (esAdminGlobal)
            {
                model.Empresas = authServices.GetAllEmpresas();
            }
            else
            {
                // Obtenemos las empresas "Madre" únicas a las que pertenece el usuario
                model.Empresas = usuario.UsuariosEmpresas
                    .Select(ue => ue.SubEmpresas.Empresas)
                    .Where(e => e != null)
                    .Distinct()
                    .ToList();
            }

            return View(model);
        }

        #region sub
        [Authorize]
        public ActionResult SeleccionarSubempresa(int id)
        {
            var usuarioNombre = User.Identity.Name;

            // 1. Verificamos en la Base de Datos si el usuario logueado tiene el BitAdministrador en true
            bool esAdminGlobal = false;

            var currentUser = db.Usuarios.FirstOrDefault(u => u.Usuario == usuarioNombre);
            if (currentUser != null && currentUser.BitAdministrador == true)
            {
                esAdminGlobal = true;
            }

            List<SubEmpresasVM> subempresas;

            if (esAdminGlobal)
            {
                // 2. Si el usuario resultó ser administrador, le traemos TODAS las subempresas
                subempresas = authServices.GetSubEmpresasPorEmpresa(id);
            }
            else
            {
                // 3. Si es usuario normal, filtramos solo las subempresas que le asignaste en "Gestión de Accesos"
                subempresas = db.UsuariosEmpresas
                    .Where(ue => ue.Usuarios.Usuario == usuarioNombre && ue.idEmpresa == id)
                    .Select(ue => new SubEmpresasVM
                    {
                        Id = ue.SubEmpresas.Id,
                        Nombre = ue.SubEmpresas.Nombre,
                        Sucursal = ue.SubEmpresas.Sucursal,
                        Rfc = ue.SubEmpresas.Rfc,
                        Direccion = ue.SubEmpresas.Direccion,
                        Logo = ue.SubEmpresas.Logo
                    }).ToList();

                // 4. CORRECCIÓN DEL LOGIN: Si no tiene empresas, no lo expulses.
                if (!subempresas.Any())
                {
                    TempData["mensaje"] = "No tienes sucursales asignadas para esta empresa.";
                    return RedirectToAction("Perfil", "Auth");
                }
            }

            ViewBag.EmpresaMadre = db.Empresas.Where(e => e.Id == id).Select(e => e.Nombre).FirstOrDefault();
            ViewBag.EmpresaId = id;

            return View("SeleccionarSubempresa", subempresas);
        }




        #endregion


        #region metodos de usuarios 


        [HttpGet]
        public ActionResult AsignarRol(int id)
        {

            var roles = authServices.GetAllRoles();
            var empresas = authServices.GetAllEmpresas();

            var model = new AsignarRolVM
            {
                IdUsuario = id,
                ListaRoles = roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre }).ToList(),
                ListaEmpresas = empresas.Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nombre }).ToList()
            };
            return View(model);
        }

        #endregion

        [HttpGet]
        [Authorize()]
        public ActionResult CambiarContrasena()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize()]
        public ActionResult CambiarContrasena(string viejaContrasenia, string nuevaContrasenia)
        {

            {
                if (!viejaContrasenia.Equals(nuevaContrasenia))
                {
                    if (authServices.CambiarContrasenia(User.Identity.Name, viejaContrasenia, nuevaContrasenia))
                    {
                        TempData["ContrasenaCambiada"] = true;
                        return RedirectToAction("Login");
                    }
                    return View();
                }

                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "AdministadorGeneral")]
        public ActionResult CambiarContraseniaU()
        { 
              if (User.Identity.IsAuthenticated)
            {
                return View();
    }
            return RedirectToAction("Login");
}

[HttpPost]
        [Authorize(Roles = "AdministadorGeneral")]
        public ActionResult CambiarContraseniaU(string nombreUsuario, string viejaContrasenia, string nuevaContrasenia)
        {
         
                if (!viejaContrasenia.Equals(nuevaContrasenia))
                {
                    if (authServices.CambiarContrasenia(nombreUsuario, viejaContrasenia, nuevaContrasenia))
                    {
                        TempData["ContrasenaCambiada"] = true;
                        return RedirectToAction("Perfil");
                    }
                }
                return View();
            
            
        }
     }
}
    

