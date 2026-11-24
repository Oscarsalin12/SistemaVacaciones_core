using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Infraestructura
{
    public interface IEmpresasService
    {
        void InsertarEmpresa(Empresas empresas, HttpPostedFileBase archivoLogo);
        void InsertarSubEmpresas(SubEmpresas subEmpresas, HttpPostedFileBase archivoLogo);
    }
}