using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Proyecto_Estadias.Models; // Asegúrate de incluir el namespace de tus modelos

namespace Proyecto_Estadias.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuariosService _usuariosService = new UsuariosService();

        // GET: Lista de Usuarios
        // Vista Principal con Tabs
        public ActionResult Index()
        {
            ViewBag.Roles = _usuariosService.ObtenerTodosLosRoles();
            var usuarios = _usuariosService.ObtenerTodosLosUsuarios();
            return View(usuarios);
        }

        // --- CRUD USUARIOS (Fetch/Modales) ---
        public ActionResult FormUsuario(int id = 0)
        {
            var model = id == 0 ? new UsuariosVM() : _usuariosService.ObtenerUsuariosPorId(id);
            return PartialView("_FormUsuario", model);
        }

        [HttpPost]
        public JsonResult SaveUsuario(UsuariosVM model)
        {
            try
            {
                if (model.Id == 0)
                    _usuariosService.CrearUsuario(model.Usuario, model.Contrasenia, model.BitAdministrador);
                else
                    _usuariosService.EditarUsuario(model);

                return Json(new { success = true, message = "Usuario guardado" });
            }
            catch
            {
                return Json(new { success = false, message = "Error al guardar el usuario" });
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

        // --- CRUD ROLES (Fetch/Modales) ---
        public ActionResult FormRol(int id = 0)
        {
            var model = id == 0 ? new RolVM() : _usuariosService.ObtenerRolPorId(id);
            return PartialView("_FormRol", model);
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
        public ActionResult AsignarRol(int id) // Nombre coincide con el botón de tu tabla
        {
            var model = new AsignarPermisosVM
            {
                IdUsuario = id,
                ListaRoles = _usuariosService.ObtenerTodosLosRoles(),
                ListaSubEmpresas = _usuariosService.ObtenerTodasLasSubEmpresas()
            };
            return View("AsignarPermisos", model);
        }
        [HttpGet]
        public ActionResult AsignarAccesos(int id)
        {
            var _db = new Vacaciones_ProyectoEntities(); // Reutiliza el contexto de DB o inyéctalo si lo tienes global

            // 1. Buscamos si el usuario ya tiene algún acceso registrado.
            // Tomamos el primero que encontremos para saber su Rol y Empresa Madre
            var accesoActual = _db.UsuariosEmpresas.FirstOrDefault(ue => ue.idUsuario == id);

            // 2. Obtenemos la lista de todas las subempresas a las que ya tiene acceso
            var subEmpresasSeleccionadas = _db.UsuariosEmpresas
                                              .Where(ue => ue.idUsuario == id)
                                              .Select(ue => ue.idSubEmpresa)
                                              .ToList();

            var model = new AsignarPermisosVM
            {
                IdUsuario = id,
                ListaRoles = _usuariosService.ObtenerTodosLosRoles(),
                ListaEmpresas = _usuariosService.ObtenerTodasLasEmpresas(),
                ListaSubEmpresas = _usuariosService.ObtenerTodasLasSubEmpresas(),

                // 3. Pre-cargamos los IDs que encontramos. 
                // Si no tiene nada, quedarán en nulo/cero y no seleccionará nada.
                IdRol = accesoActual != null ? accesoActual.idRol : 0,
                IdEmpresa = accesoActual != null ? accesoActual.idEmpresa : 0,
                idSubEmpresas = subEmpresasSeleccionadas
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AsignarAccesos(AsignarPermisosVM model)
        {
            // Pasamos el IdEmpresa (Madre) también
            _usuariosService.AsignarRolYEmpresas(model.IdUsuario, model.IdRol, model.IdEmpresa, model.idSubEmpresas);
            return RedirectToAction("Index");
        }

    }
}