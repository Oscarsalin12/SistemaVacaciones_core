using Microsoft.AspNetCore.Mvc;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;

namespace SistemaVacaciones.Web.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly IEmpresasService _empresasService;

        public EmpresasController(IEmpresasService empresasService)
        {
            _empresasService = empresasService;
        }

        private bool IsAjaxRequest => Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        [HttpGet]
        public IActionResult InsertarEmpresa()
        {
            if (IsAjaxRequest)
            {
                return PartialView("_InsertarEmpresaPartial", new Empresa());
            }
            return View(new Empresa());
        }

        [HttpPost]
        public IActionResult InsertarEmpresa(Empresa empresa, IFormFile? archivoLogo)
        {
            if (ModelState.IsValid)
            {
                byte[]? logoBytes = null;
                if (archivoLogo != null && archivoLogo.Length > 0)
                {
                    using var ms = new MemoryStream();
                    archivoLogo.CopyTo(ms);
                    logoBytes = ms.ToArray();
                }

                _empresasService.InsertarEmpresa(empresa, logoBytes);
                TempData["EmpresaCreada"] = true;

                if (IsAjaxRequest)
                {
                    return Ok();
                }
                return RedirectToAction("Perfil", "Auth");
            }

            if (IsAjaxRequest)
            {
                return PartialView("_InsertarEmpresaPartial", empresa);
            }
            return View(empresa);
        }

        [HttpGet]
        public IActionResult InsertarSubEmpresa(int idEmpresaMadre)
        {
            var modelo = new SubEmpresa
            {
                IdEmpresa_madre = idEmpresaMadre
            };

            ViewBag.NombreEmpresaMadre = "Registro para Empresa ID: " + idEmpresaMadre;
            return PartialView("_InsertarSubEmpresaPartial", modelo);
        }

        [HttpPost]
        public IActionResult InsertarSubEmpresa(SubEmpresa subEmpresa, IFormFile? archivoLogo)
        {
            if (ModelState.IsValid)
            {
                byte[]? logoBytes = null;
                if (archivoLogo != null && archivoLogo.Length > 0)
                {
                    using var ms = new MemoryStream();
                    archivoLogo.CopyTo(ms);
                    logoBytes = ms.ToArray();
                }

                _empresasService.InsertarSubEmpresas(subEmpresa, logoBytes);
                TempData["ExitoSub"] = true;

                if (IsAjaxRequest)
                {
                    return Ok();
                }
                return RedirectToAction("SeleccionarSubempresa", "Auth", new { id = subEmpresa.IdEmpresa_madre });
            }

            if (IsAjaxRequest)
            {
                ViewBag.IdMadre = subEmpresa.IdEmpresa_madre;
                return PartialView("_InsertarSubEmpresaPartial", subEmpresa);
            }
            return View(subEmpresa);
        }
    }
}
