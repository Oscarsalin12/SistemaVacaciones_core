using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Proyecto_Estadias.Services
{
    public class EmpresasService : IEmpresasService
    {
        private readonly Vacaciones_ProyectoEntities db = null;

        public EmpresasService()
        {
            this.db = new  Vacaciones_ProyectoEntities();
        }
        

        public void InsertarEmpresa(Empresas empresas, HttpPostedFileBase archivoLogo)
        {
            if (archivoLogo != null && archivoLogo.ContentLength > 0)
            {
                using (var reader = new System.IO.BinaryReader(archivoLogo.InputStream))
                {
                    empresas.Logo = reader.ReadBytes(archivoLogo.ContentLength);
                }
                db.Empresas.Add(empresas);
                db.SaveChanges();
            }
        }

        public void InsertarSubEmpresas(SubEmpresas subEmpresas, HttpPostedFileBase archivoLogo)
        {
            if (archivoLogo  != null && archivoLogo.ContentLength > 0)
            {
                using(var reader = new System.IO.BinaryReader(archivoLogo.InputStream))
                {
                    subEmpresas.Logo = reader.ReadBytes(archivoLogo.ContentLength);
                }
                db.SubEmpresas.Add(subEmpresas);
                db.SaveChanges();
            }
        }



    }
}