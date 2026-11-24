using Org.BouncyCastle.Crypto;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_Estadias.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly IEmpresasService empresasService;
        private readonly IAuthServices authServices;
        public EmpresasController()
        {
            empresasService = new EmpresasService();
            authServices = new AuthServices();
        }
        [HttpGet]
        public ActionResult InsertarEmpresa()
        {
            if (Request.IsAjaxRequest()) // Verifica si es una llamada asíncrona
            {
                return PartialView("_InsertarEmpresaPartial"); // Crea esta vista parcial
            }
            return View();
        }
        [HttpPost]
        public ActionResult InsertarEmpresa(Empresas empresa, HttpPostedFileBase archivoLogo)
        {
            if (ModelState.IsValid)
            {
                empresasService.InsertarEmpresa(empresa, archivoLogo);
                TempData["EmpresaCreada"] = true;

                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(200); // Éxito para Fetch
                }
                return RedirectToAction("Perfil", "Auth");
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_InsertarEmpresaPartial", empresa); // Devuelve errores de validación al modal
            }
            return View(empresa);
        }

        [HttpGet]
        public ActionResult InsertarSubEmpresa(int idEmpresaMadre)
        {
            // Solo inicializamos el modelo con el ID recibido del botón
            var modelo = new Proyecto_Estadias.Models.SubEmpresas
            {
                IdEmpresa_madre = idEmpresaMadre
            };

            // Si no quieres consultar la base, puedes pasar un texto fijo o dejarlo vacío
            ViewBag.NombreEmpresaMadre = "Registro para Empresa ID: " + idEmpresaMadre;

            return PartialView("_InsertarSubEmpresaPartial", modelo);
        }

        [HttpPost]
        public ActionResult InsertarSubEmpresa(SubEmpresas subEmpresa, HttpPostedFileBase archivoLogo)
        {
            if (ModelState.IsValid)
            {
                // Tu lógica de servicio para insertar
                empresasService.InsertarSubEmpresas(subEmpresa, archivoLogo);
                TempData["ExitoSub"] = true;

                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(200);
                }
                return RedirectToAction("SeleccionarSubempresa", "Auth", new { id = subEmpresa.IdEmpresa_madre });
            }

            if (Request.IsAjaxRequest())
            {
                ViewBag.IdMadre = subEmpresa.IdEmpresa_madre;
                return PartialView("_InsertarSubEmpresaPartial", subEmpresa);
            }
            return View(subEmpresa);
        }
    }
}