using Microsoft.AspNetCore.Mvc;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Web.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuariosService _usuariosService;
        private readonly SistemaVacacionesDbContext _db;

        public UsuariosController(IUsuariosService usuariosService, SistemaVacacionesDbContext db)
        {
            _usuariosService = usuariosService;
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.Roles = _usuariosService.ObtenerTodosLosRoles();
            var usuarios = _usuariosService.ListarUsuarios().Select(u => new UsuariosVM
            {
                Id = u.Id,
                Usuario = u.NombreUsuario,
                BitAdministrador = u.BitAdministrador
            }).ToList();

            return View(usuarios);
        }

        public IActionResult FormUsuario(int id = 0)
        {
            UsuariosVM model;
            if (id == 0)
            {
                model = new UsuariosVM();
            }
            else
            {
                var u = _usuariosService.ObtenerUsuarioPorId(id);
                model = u == null ? new UsuariosVM() : new UsuariosVM
                {
                    Id = u.Id,
                    Usuario = u.NombreUsuario,
                    BitAdministrador = u.BitAdministrador
                };
            }
            return PartialView("_FormUsuario", model);
        }

        [HttpPost]
        public JsonResult SaveUsuario(UsuariosVM model)
        {
            try
            {
                if (model.Id == 0)
                    _usuariosService.CrearUsuario(model);
                else
                    _usuariosService.EditarUsuario(model);

                return Json(new { success = true, message = "Usuario guardado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar el usuario: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUsuario(int id)
        {
            try
            {
                _usuariosService.EliminarUsuario(id);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        public IActionResult FormRol(int id = 0)
        {
            var model = id == 0 ? new RolVM() : _usuariosService.ObtenerRolPorId(id);
            return PartialView("_FormRol", model ?? new RolVM());
        }

        [HttpPost]
        public JsonResult SaveRol(RolVM model)
        {
            try
            {
                if (model.Id == 0)
                    _usuariosService.CrearRol(model);
                else
                    _usuariosService.EditarRol(model);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteRol(int id)
        {
            try
            {
                _usuariosService.EliminarRol(id);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public IActionResult AsignarAccesos(int id)
        {
            var accesoActual = _db.UsuariosEmpresas.FirstOrDefault(ue => ue.IdUsuario == id);

            var subEmpresasSeleccionadas = _db.UsuariosEmpresas
                .Where(ue => ue.IdUsuario == id)
                .Select(ue => ue.IdSubEmpresa)
                .ToList();

            var model = new AsignarPermisosVM
            {
                IdUsuario = id,
                ListaRoles = _usuariosService.ObtenerTodosLosRoles(),
                ListaEmpresas = _db.Empresas.OrderBy(e => e.Nombre).ToList(),
                ListaSubEmpresas = _usuariosService.ObtenerTodasLasSubEmpresas(),
                IdRol = accesoActual != null ? accesoActual.IdRol : 0,
                IdEmpresa = accesoActual != null ? accesoActual.IdEmpresa : 0,
                idSubEmpresas = subEmpresasSeleccionadas
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AsignarAccesos(AsignarPermisosVM model)
        {
            _usuariosService.AsignarRolYEmpresas(model.IdUsuario, model.IdRol, model.IdEmpresa, model.idSubEmpresas);
            return RedirectToAction("Index");
        }
    }
}
