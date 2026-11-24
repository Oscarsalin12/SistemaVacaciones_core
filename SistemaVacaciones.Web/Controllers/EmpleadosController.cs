using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Web.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly IEmpleadosServices _empleadoServices;
        private readonly IAuthServices _authServices;
        private readonly SistemaVacacionesDbContext _db;

        public EmpleadosController(
            IEmpleadosServices empleadoServices,
            IAuthServices authServices,
            SistemaVacacionesDbContext db)
        {
            _empleadoServices = empleadoServices;
            _authServices = authServices;
            _db = db;
        }

        [HttpGet]
        public IActionResult InsertarEmpleado(int idsub)
        {
            ViewBag.Departamentos = new SelectList(_authServices.GetDepartamentos(), "Id", "NombreDep");

            var nuevoEmpleado = new Empleado
            {
                IdEmpresa = idsub,
                FechaIDeIngreso = DateTime.Now,
                Estado = "Activo"
            };

            return PartialView("_FormularioEmpleado", nuevoEmpleado);
        }

        [HttpPost]
        public JsonResult InsertarEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _empleadoServices.InsertarEmpleados(empleado);
                return Json(new { success = true, message = "Empleado agregado correctamente" });
            }
            return Json(new { success = false, message = "Verifique los datos ingresados" });
        }

        [HttpGet]
        public IActionResult EditarEmpleado(int id)
        {
            var empleado = _empleadoServices.ObtenerEmpleadoPorId(id);
            if (empleado == null) return NotFound();

            ViewBag.Departamentos = new SelectList(_authServices.GetDepartamentos(), "Id", "NombreDep", empleado.IdDepartamento);
            return PartialView("_FormularioEmpleado", empleado);
        }

        [HttpPost]
        public JsonResult EditarEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _empleadoServices.ActualizarEmpleado(empleado);
                return Json(new { success = true, message = "Empleado actualizado correctamente" });
            }
            return Json(new { success = false, message = "Verifique los datos ingresados" });
        }

        [HttpGet]
        public IActionResult DetallesEmpleado(int id)
        {
            var empleado = _empleadoServices.ObtenerEmpleadoPorId(id);
            if (empleado == null) return NotFound();

            return PartialView("_DetallesEmpleado", empleado);
        }

        [HttpGet]
        public IActionResult LoginEmpleado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginEmpleado(EmpleadosLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor, ingrese un usuario y contraseña válidos.";
                return View(model);
            }

            var empleado = _empleadoServices.LoginEmpleados(model.usuario, model.NumeroEmpleado);
            if (empleado != null)
            {
                HttpContext.Session.SetInt32("EmpleadoId", empleado.Id);
                HttpContext.Session.SetString("EmpleadoNombre", empleado.UsuarioEmpleado ?? "");
                HttpContext.Session.SetInt32("EmpleadoNum", empleado.NumEmpleado);

                TempData["LoginExitoso"] = true;
                return RedirectToAction("PerfilEmpleado", "Empleados");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View(model);
        }

        [HttpGet]
        public IActionResult LogoutEmpleado()
        {
            HttpContext.Session.Clear();
            TempData["LogoutExitoso"] = true;
            return RedirectToAction("LoginEmpleado", "Empleados");
        }

        [HttpGet]
        public IActionResult PerfilEmpleado()
        {
            int? empleadoId = HttpContext.Session.GetInt32("EmpleadoId");
            if (!empleadoId.HasValue)
            {
                return RedirectToAction("LoginEmpleado", "Empleados");
            }

            var empleado = _empleadoServices.ObtenerEmpleadoPorId(empleadoId.Value);
            if (empleado == null)
            {
                TempData["ErrorEmpleado"] = true;
                return NotFound();
            }

            return View(empleado);
        }

        [Authorize]
        public IActionResult ListaEmpleadosSubempresa(int idSub, int? bus, string? busqueda, int? page, int pageSize = 5)
        {
            if (idSub <= 0) return NotFound("ID de subempresa no válido");

            _empleadoServices.ActualizarVacacionesYPermisos();

            var empleadosQuery = _empleadoServices.FiltrarEmpleados(idSub, "Activo", busqueda, bus);

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
                NombreEmpresa = e.SubEmpresa != null ? e.SubEmpresa.Nombre : "",
                NombreDepartamento = e.Departamento != null ? e.Departamento.NombreDep : ""
            })
            .OrderBy(e => e.Nombre)
            .ToPagedList(page ?? 1, pageSize);

            ViewBag.SubempresaNombre = _authServices.GetNombreSubempresas(idSub);
            ViewBag.IdSub = idSub;

            var idMadre = _db.SubEmpresas
                .Where(s => s.Id == idSub)
                .Select(s => s.IdEmpresa_madre)
                .FirstOrDefault();
            ViewBag.IdEmpresaMadre = idMadre;

            ViewBag.Departamentos = new SelectList(_authServices.GetDepartamentos(), "Id", "NombreDep");

            return View("EmpleadosPorEmpresa", empleadosVM);
        }

        [Authorize]
        public IActionResult ListaEmpleadosDadosDeBaja(int idSub, int? bus, string? busqueda, int? page, int pageSize = 5)
        {
            if (idSub <= 0) return NotFound("ID de subempresa no válido");

            var empleadosQuery = _empleadoServices.FiltrarEmpleados(idSub, "Inactivo", busqueda, bus);

            var empleadosVM = empleadosQuery.Select(e => new EmpleadosVM
            {
                Id = e.Id,
                NumEmpleado = e.NumEmpleado,
                Nombre = e.Nombre,
                FechaDeIngreso = e.FechaIDeIngreso,
                IdEmpresa = e.IdEmpresa,
                IdDepartamento = e.IdDepartamento,
                NombreEmpresa = e.SubEmpresa != null ? e.SubEmpresa.Nombre : "",
                NombreDepartamento = e.Departamento != null ? e.Departamento.NombreDep : ""
            })
            .OrderBy(e => e.Nombre)
            .ToPagedList(page ?? 1, pageSize);

            ViewBag.SubempresaNombre = _authServices.GetNombreSubempresas(idSub);
            ViewBag.IdSub = idSub;

            var idMadre = _db.SubEmpresas
                .Where(s => s.Id == idSub)
                .Select(s => s.IdEmpresa_madre)
                .FirstOrDefault();
            ViewBag.IdEmpresaMadre = idMadre;

            ViewBag.Departamentos = new SelectList(_authServices.GetDepartamentos(), "Id", "NombreDep");

            return View(empleadosVM);
        }
    }
}
