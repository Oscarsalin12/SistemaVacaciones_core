using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Services;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Proyecto_Estadias.Controllers
{
    public class MotivosController : Controller
    {
        private readonly IMotivoServices motivoServices;

        public MotivosController()
        {
            motivoServices = new MotivoServices();
        }

        public ActionResult Index()
        {
            return View(motivoServices.Listar());
        }

        // Devuelve solo el formulario de creación
        public ActionResult Create() => PartialView("_Formulario", new MotivosAusenciaVM());

        [HttpPost]
        public JsonResult Create(MotivosAusenciaVM model)
        {
            if (ModelState.IsValid)
            {
                motivoServices.Agregar(model);
                return Json(new { success = true, message = "Guardado con éxito" });
            }
            return Json(new { success = false, message = "Datos inválidos" });
        }

        // Devuelve solo el formulario cargado con datos
        public ActionResult Edit(int id)
        {
            var model = motivoServices.ObtenerPorId(id);
            return model == null ? (ActionResult)HttpNotFound() : PartialView("_Formulario", model);
        }

        [HttpPost]
        public JsonResult Edit(MotivosAusenciaVM model)
        {
            if (ModelState.IsValid)
            {
                motivoServices.Editar(model);
                return Json(new { success = true, message = "Actualizado con éxito" });
            }
            return Json(new { success = false, message = "Error al actualizar" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                motivoServices.Eliminar(id);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}

