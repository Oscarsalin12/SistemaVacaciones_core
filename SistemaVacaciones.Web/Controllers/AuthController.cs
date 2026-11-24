using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;
using System.Security.Claims;

namespace SistemaVacaciones.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthServices _authServices;
        private readonly IEmpleadosServices _empleadoServices;
        private readonly SistemaVacacionesDbContext _db;

        public AuthController(
            IAuthServices authServices,
            IEmpleadosServices empleadoServices,
            SistemaVacacionesDbContext db)
        {
            _authServices = authServices;
            _empleadoServices = empleadoServices;
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UsuariosVM model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                // 1. BACKDOOR: Si el sistema está en ceros
                if (!_db.Usuarios.Any())
                {
                    if (model.Usuario == "Admin" && model.Contrasenia == "Admin123")
                    {
                        var rescueClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, "0"),
                            new Claim(ClaimTypes.Name, "Admin"),
                            new Claim(ClaimTypes.Role, "AdministradorGeneral"),
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                        var rescueIdentity = new ClaimsIdentity(rescueClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(rescueIdentity));

                        TempData["LoginExitoso"] = true;
                        TempData["mensaje"] = "Modo de rescate activo. Por favor, ve a Gestión de Accesos y crea el primer usuario.";
                        return Redirect(returnUrl ?? Url.Action("Perfil", "Auth")!);
                    }
                }

                // 2. FLUJO NORMAL
                var usuario = _authServices.Login(model.Usuario, model.Contrasenia);
                if (usuario != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.NombreUsuario)
                    };

                    if (usuario.BitAdministrador)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "AdministradorGeneral"));
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }

                    if (usuario.UsuariosEmpresas != null && usuario.UsuariosEmpresas.Any())
                    {
                        var firstEmpresa = usuario.UsuariosEmpresas.First();
                        claims.Add(new Claim("IdEmpresa", firstEmpresa.IdEmpresa.ToString()));
                        claims.Add(new Claim("IdEmpresaMadre", firstEmpresa.IdEmpresa.ToString()));

                        foreach (var item in usuario.UsuariosEmpresas)
                        {
                            if (item.Rol != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, item.Rol.Nombre));
                                claims.Add(new Claim("RolEmpresa", $"{item.Rol.Nombre}:{item.IdEmpresa}"));
                                claims.Add(new Claim("RolSubEmpresa", $"{item.Rol.Nombre}:{item.IdSubEmpresa}"));
                            }
                            claims.Add(new Claim("EmpresaId", item.IdEmpresa.ToString()));
                        }
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

                    TempData["LoginExitoso"] = true;
                    return Redirect(returnUrl ?? Url.Action("Perfil", "Auth")!);
                }

                ModelState.AddModelError("", "Credenciales incorrectas.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [Authorize]
        public IActionResult Perfil()
        {
            var usuarioNombre = User.Identity?.Name ?? "";

            if (usuarioNombre == "Admin")
            {
                var modelSA = new PerfilVM
                {
                    Usuario = new Usuario { NombreUsuario = "Admin", BitAdministrador = true },
                    Empresas = _authServices.GetAllEmpresas() ?? new List<Empresa>()
                };
                return View(modelSA);
            }

            var usuario = _authServices.GetUsuarioConRelaciones(usuarioNombre);
            bool esAdminGlobal = _authServices.EsAdmin(usuarioNombre);

            if (usuario == null && !esAdminGlobal) return RedirectToAction("Login");

            var model = new PerfilVM
            {
                Usuario = usuario,
                Empresas = new List<Empresa>()
            };

            if (esAdminGlobal)
            {
                model.Empresas = _authServices.GetAllEmpresas();
            }
            else if (usuario?.UsuariosEmpresas != null)
            {
                model.Empresas = usuario.UsuariosEmpresas
                    .Where(ue => ue.SubEmpresa?.Empresa != null)
                    .Select(ue => ue.SubEmpresa!.Empresa!)
                    .Distinct()
                    .ToList();
            }

            return View(model);
        }

        [Authorize]
        public IActionResult SeleccionarSubempresa(int id)
        {
            var usuarioNombre = User.Identity?.Name ?? "";
            bool esAdminGlobal = false;

            var currentUser = _db.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuarioNombre);
            if (currentUser != null && currentUser.BitAdministrador)
            {
                esAdminGlobal = true;
            }

            List<SubEmpresasVM> subempresas;

            if (esAdminGlobal)
            {
                subempresas = _authServices.GetSubEmpresasPorEmpresa(id);
            }
            else
            {
                subempresas = _db.UsuariosEmpresas
                    .Where(ue => ue.Usuario != null && ue.Usuario.NombreUsuario == usuarioNombre && ue.IdEmpresa == id && ue.SubEmpresa != null)
                    .Select(ue => new SubEmpresasVM
                    {
                        Id = ue.SubEmpresa!.Id,
                        Nombre = ue.SubEmpresa.Nombre,
                        Sucursal = ue.SubEmpresa.Sucursal,
                        Rfc = ue.SubEmpresa.Rfc,
                        Direccion = ue.SubEmpresa.Direccion,
                        Logo = ue.SubEmpresa.Logo
                    }).ToList();

                if (!subempresas.Any())
                {
                    TempData["mensaje"] = "No tienes sucursales asignadas para esta empresa.";
                    return RedirectToAction("Perfil", "Auth");
                }
            }

            ViewBag.EmpresaMadre = _db.Empresas.Where(e => e.Id == id).Select(e => e.Nombre).FirstOrDefault();
            ViewBag.EmpresaId = id;

            return View("SeleccionarSubempresa", subempresas);
        }

        [HttpGet]
        public IActionResult AsignarRol(int id)
        {
            var roles = _authServices.GetAllRoles();
            var empresas = _authServices.GetAllEmpresas();

            var model = new AsignarRolVM
            {
                IdUsuario = id
            };
            ViewBag.ListaRoles = roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre }).ToList();
            ViewBag.ListaEmpresas = empresas.Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nombre }).ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CambiarContrasena()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarContrasena(string viejaContrasenia, string nuevaContrasenia)
        {
            if (!viejaContrasenia.Equals(nuevaContrasenia))
            {
                if (_authServices.CambiarContrasenia(User.Identity?.Name ?? "", viejaContrasenia, nuevaContrasenia))
                {
                    TempData["ContrasenaCambiada"] = true;
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "AdministradorGeneral,Admin")]
        public IActionResult CambiarContraseniaU()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "AdministradorGeneral,Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarContraseniaU(string nombreUsuario, string viejaContrasenia, string nuevaContrasenia)
        {
            if (!viejaContrasenia.Equals(nuevaContrasenia))
            {
                if (_authServices.CambiarContrasenia(nombreUsuario, viejaContrasenia, nuevaContrasenia))
                {
                    TempData["ContrasenaCambiada"] = true;
                    return RedirectToAction("Perfil");
                }
            }
            return View();
        }
    }
}
