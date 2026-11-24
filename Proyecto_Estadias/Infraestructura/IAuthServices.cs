using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Infraestructura
{
    public interface IAuthServices
    {
   
        bool CambiarContrasenia(string nombreUsuario, string viejaContrasenia, string nuevaContrasenia);
        bool EsAdmin(string nombreusuario);
        List<Empresas> GetAllEmpresas();
        List<Roles> GetAllRoles();
        List<SubEmpresas> GetAllSubEmpresas();
        List<Departamentos> GetDepartamentos();
        List<Empleados> GetEmpleados(int idEmpresa);
        List<EmpleadosVM> GetEmpleadosPorEmpresaYSubempresa(int idEmpresa);
        string GetEmpresaNombre(int idEmpresa);
        string GetNombreSubempresas(int idSub);
        string GetSubEmpresasNombre(int idEmpresa_madre);
        List<SubEmpresasVM> GetSubEmpresasPorEmpresa(int idEmpresa);
        Usuarios GetUsuarioConRelaciones(string nombreusuario);
       


        #region Lista de empleados

        #endregion

        #region empresas y sub

        #endregion
        Usuarios login(string nombreusuario, string contrasenia);
        List<SubEmpresas> ObtenerSubEmpresas();
      
    }
}