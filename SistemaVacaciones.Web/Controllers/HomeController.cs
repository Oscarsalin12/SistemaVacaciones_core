using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SistemaVacaciones.Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles = "AdministradorGeneral,Admin,administrador")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Aplicación de Gestión de Vacaciones y Permisos.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Contacto del Sistema.";
            return View();
        }
    }
}
