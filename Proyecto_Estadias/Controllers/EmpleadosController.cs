
using PagedList;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;


namespace Proyecto_Estadias.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IEmpleadosServices empleadoServices;
        private readonly IAuthServices authServices;
        public EmpleadosController() 
        {
            authServices = new AuthServices();
            empleadoServices = new EmpleadoServices();
        }

        #region Métodos CRUD SPA (Fetch API)

        // GET: Formulario de Creación
        [HttpGet]
        public ActionResult InsertarEmpleado(int idsub)
        {
            ViewBag.Departamentos = new SelectList(authServices.GetDepartamentos(), "Id", "NombreDep");

            var nuevoEmpleado = new Empleados
            {
                IdEmpresa = idsub,
                FechaIDeIngreso = DateTime.Now,
                Estado = "Activo"
            };

            // Retornamos la vista parcial
            return PartialView("_FormularioEmpleado", nuevoEmpleado);
        }

        // POST: Guardar Nuevo Empleado
        [HttpPost]
        public JsonResult InsertarEmpleado(Empleados empleado)
        {
            if (ModelState.IsValid)
            {
                empleadoServices.InsertarEmpleados(empleado);
                return Json(new { success = true, message = "Empleado agregado correctamente" });
            }
            return Json(new { success = false, message = "Verifique los datos ingresados" });
        }

        // GET: Formulario de Edición
        [HttpGet]
        public ActionResult EditarEmpleado(int id)
        {
            var empleado = empleadoServices.ObtenerEmpleadoPorId(id);
            if (empleado == null) return HttpNotFound();

            ViewBag.Departamentos = new SelectList(authServices.GetDepartamentos(), "Id", "NombreDep", empleado.IdDepartamento);

            return PartialView("_FormularioEmpleado", empleado);
        }

        // POST: Guardar Edición
        [HttpPost]
        public JsonResult EditarEmpleado(Empleados empleado)
        {
            if (ModelState.IsValid)
            {
                empleadoServices.ActualizarEmpleado(empleado);
                return Json(new { success = true, message = "Empleado actualizado correctamente" });
            }
            return Json(new { success = false, message = "Verifique los datos ingresados" });
        }

        // GET: Vista Parcial con Detalles (Nuevo Método)
        [HttpGet]
        public ActionResult DetallesEmpleado(int id)
        {
            var empleado = empleadoServices.ObtenerEmpleadoPorId(id);
            if (empleado == null) return HttpNotFound();

            return PartialView("_DetallesEmpleado", empleado);
        }

        #endregion

        [HttpGet]
        public ActionResult LoginEmpleado()
        {
        return View();
        }

        [HttpPost]
        public ActionResult LoginEmpleado(EmpleadosLoginVM model)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, ingrese un usuario y contraseña válidos.";
                return View(model);
            }
            var empleado = empleadoServices.LoginEmpleados(model.usuario, model.NumeroEmpleado);
            if (empleado != null)
            {
                Session["EmpleadoId"] = empleado.Id;
                Session["EmpleadoNombre"] = empleado.UsuarioEmpleado;
                Session["EmpleadoNum"] = empleado.NumEmpleado;

                TempData["LoginExitoso"] = true;
                return RedirectToAction("PerfilEmpleado", "Empleados");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult LogoutEmpleado()
        {
            Session.Clear();
            Session.Abandon();
            TempData["LogoutExitoso"] = true;
            return RedirectToAction("LoginEmpleado", "Empleados");
        }
        [HttpGet]
        public ActionResult PerfilEmpleado()
        {
            if (Session["EmpleadoId"] == null)
            {
                return RedirectToAction("LoginEmpleado", "Empleados");
            }

            int empleadoId = (int)Session["EmpleadoId"];
            var empleado = empleadoServices.ObtenerEmpleadoPorId(empleadoId);

            if (empleado == null)
            {
                TempData["ErrorEmpleado"] = true;
                return HttpNotFound();
            }

            return View(empleado);
        }



        #region Listados de Empleados

        #region Listados de Empleados

        [Authorize]
        public ActionResult ListaEmpleadosSubempresa(int idSub, int? bus, string busqueda, int? page, int pageSize = 5)
        {
            if (idSub <= 0) return HttpNotFound("ID de subempresa no válido");

            empleadoServices.ActualizarVacacionesYPermisos();

            // Usamos el nuevo método del servicio para buscar "Activos"
            var empleadosQuery = empleadoServices.FiltrarEmpleados(idSub, "Activo", busqueda, bus);

            var empleadosVM = empleadosQuery.Select(e => new EmpleadosVM
            {
                Id = e.Id,
                NumEmpleado = e.NumEmpleado,
                Nombre = e.Nombre,
                FechaDeIngreso = e.FechaIDeIngreso,
                AniosTrabajados = e.AniosTrabajados,
                MesesTrabajados = e.MesesTrabajados,
                DiasLaboralesPorSemana = e.DiasLaboralesPorSemana,
                intDiasVacaciones = e.intDiasVacaciones,
                HorasRestantesDelDia = e.HorasRestantesDelDia,
                DiasDisponibles = e.DiasDisponibles,
                IdEmpresa = e.IdEmpresa,
                IdDepartamento = e.IdDepartamento,
                NombreEmpresa = e.SubEmpresas.Nombre,
                NombreDepartamento = e.Departamentos.NombreDep
            })
            .OrderBy(e => e.Nombre)
            .ToPagedList(page ?? 1, pageSize);

            ViewBag.SubempresaNombre = authServices.GetNombreSubempresas(idSub);
            ViewBag.IdSub = idSub;

            // =========================================================
            // NUEVO: Buscamos el ID de la Empresa Madre para el botón de Regresar
            // =========================================================
            using (var db = new Vacaciones_ProyectoEntities())
            {
                var idMadre = db.SubEmpresas
                                .Where(s => s.Id == idSub)
                                .Select(s => s.IdEmpresa_madre)
                                .FirstOrDefault();
                ViewBag.IdEmpresaMadre = idMadre;
            }
            // =========================================================

            // Utilizamos el servicio para llenar el DropDown, sin tocar la BD directamente desde el controlador
            ViewBag.Departamentos = new SelectList(authServices.GetDepartamentos(), "Id", "NombreDep");

            return View("EmpleadosPorEmpresa", empleadosVM);
        }

        [Authorize]
        public ActionResult ListaEmpleadosDadosDeBaja(int idSub, int? bus, string busqueda, int? page, int pageSize = 5)
        {
            if (idSub <= 0) return HttpNotFound("ID de subempresa no válido");

            // Usamos el nuevo método del servicio para buscar "Inactivos"
            var empleadosQuery = empleadoServices.FiltrarEmpleados(idSub, "Inactivo", busqueda, bus);

            var empleadosVM = empleadosQuery.Select(e => new EmpleadosVM
            {
                Id = e.Id,
                NumEmpleado = e.NumEmpleado,
                Nombre = e.Nombre,
                FechaDeIngreso = e.FechaIDeIngreso,
                IdEmpresa = e.IdEmpresa,
                IdDepartamento = e.IdDepartamento,
                NombreEmpresa = e.SubEmpresas.Nombre,
                NombreDepartamento = e.Departamentos.NombreDep
            })
            .OrderBy(e => e.Nombre)
            .ToPagedList(page ?? 1, pageSize);

            ViewBag.SubempresaNombre = authServices.GetNombreSubempresas(idSub);
            ViewBag.IdSub = idSub;

            // =========================================================
            // NUEVO: También lo agregamos aquí por si el botón existe en la vista de bajas
            // =========================================================
            using (var db = new Vacaciones_ProyectoEntities())
            {
                var idMadre = db.SubEmpresas
                                .Where(s => s.Id == idSub)
                                .Select(s => s.IdEmpresa_madre)
                                .FirstOrDefault();
                ViewBag.IdEmpresaMadre = idMadre;
            }
            // =========================================================

            ViewBag.Departamentos = new SelectList(authServices.GetDepartamentos(), "Id", "NombreDep");

            return View(empleadosVM);
        }

        #endregion

        #endregion
    }
}

