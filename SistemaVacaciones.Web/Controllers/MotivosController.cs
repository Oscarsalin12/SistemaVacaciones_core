using Microsoft.AspNetCore.Mvc;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Web.Controllers
{
    public class MotivosController : Controller
    {
        private readonly IMotivoServices _motivoServices;

        public MotivosController(IMotivoServices motivoServices)
        {
            _motivoServices = motivoServices;
        }

        public IActionResult Index()
        {
            return View(_motivoServices.Listar());
        }

        public IActionResult Create() => PartialView("_Formulario", new MotivosAusenciaVM());

        [HttpPost]
        public JsonResult Create(MotivosAusenciaVM model)
        {
            if (ModelState.IsValid)
            {
                _motivoServices.Agregar(model);
                return Json(new { success = true, message = "Guardado con éxito" });
            }
            return Json(new { success = false, message = "Datos inválidos" });
        }

        public IActionResult Edit(int id)
        {
            var model = _motivoServices.ObtenerPorId(id);
            return model == null ? NotFound() : PartialView("_Formulario", model);
        }

        [HttpPost]
        public JsonResult Edit(MotivosAusenciaVM model)
        {
            if (ModelState.IsValid)
            {
                _motivoServices.Editar(model);
                return Json(new { success = true, message = "Actualizado con éxito" });
            }
            return Json(new { success = false, message = "Error al actualizar" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _motivoServices.Eliminar(id);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
