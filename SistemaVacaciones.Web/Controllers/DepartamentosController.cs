using Microsoft.AspNetCore.Mvc;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Web.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly IDepartamentosServices _departamentosServices;

        public DepartamentosController(IDepartamentosServices departamentosServices)
        {
            _departamentosServices = departamentosServices;
        }

        public IActionResult Index()
        {
            var departamentos = _departamentosServices.Listar();
            return View(departamentos);
        }

        public IActionResult Create() => PartialView("_Formulario", new DepartamentosVM());

        [HttpPost]
        public JsonResult Create(DepartamentosVM model)
        {
            if (ModelState.IsValid)
            {
                _departamentosServices.Agregar(model);
                return Json(new { success = true, Message = "Departamento creado" });
            }
            return Json(new { success = false, Message = "Error al crear el departamento" });
        }

        public IActionResult Edit(int id)
        {
            var departamento = _departamentosServices.ObtenerPorId(id);
            if (departamento == null) return NotFound();
            return PartialView("_Formulario", departamento);
        }

        [HttpPost]
        public JsonResult Edit(DepartamentosVM model)
        {
            if (ModelState.IsValid)
            {
                _departamentosServices.Editar(model);
                return Json(new { success = true, Message = "Departamento actualizado" });
            }
            return Json(new { success = false, Message = "Error al actualizar el departamento" });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _departamentosServices.Eliminar(id);
                return Json(new { success = true, Message = "Departamento eliminado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Error al eliminar el departamento: " + ex.Message });
            }
        }
    }
}
