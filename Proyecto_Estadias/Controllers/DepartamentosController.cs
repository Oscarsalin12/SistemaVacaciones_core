using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Proyecto_Estadias.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly IDepartamentosServices departamentosServices;

        public DepartamentosController()
        {
            departamentosServices = new DepartamentoServices();
        }

        public ActionResult Index()
        {
            var departamentos = departamentosServices.Listar();
            return View(departamentos);
        }

        public ActionResult Create() => PartialView("_Formulario", new DepartamentosVM());

        [HttpPost]
        public JsonResult Create(DepartamentosVM model)
        {
            if (ModelState.IsValid)
            {
                departamentosServices.Agregar(model);
                return Json(new { success = true, Message = "Departamento creado" });
            }
            return Json(new { success = false, Message = "Error al crear el departamento" });
        }

        public ActionResult Edit(int id)
        {
            var departamento = departamentosServices.ObtenerPorId(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Formulario", departamento);
        }

        [HttpPost]
        public JsonResult Edit(DepartamentosVM model)
        {
            if (ModelState.IsValid)
            {
                departamentosServices.Editar(model);
                return Json(new { success = true, Message = "Departamento actualizado" });
            }
            return Json(new { success = false, Message = "Error al actualizar el departamento" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                departamentosServices.Eliminar(id);
                return Json(new { success = true, Message = "Departamento eliminado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Error al eliminar el departamento: " + ex.Message });
            }
        }
    }
}