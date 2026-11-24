using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IAuthServices
    {
        bool CambiarContrasenia(string nombreUsuario, string viejaContrasenia, string nuevaContrasenia);
        bool EsAdmin(string nombreusuario);
        List<Empresa> GetAllEmpresas();
        List<Rol> GetAllRoles();
        List<SubEmpresa> GetAllSubEmpresas();
        List<Departamento> GetDepartamentos();
        List<Empleado> GetEmpleados(int idEmpresa);
        List<EmpleadosVM> GetEmpleadosPorEmpresaYSubempresa(int idEmpresa);
        string? GetEmpresaNombre(int idEmpresa);
        string? GetNombreSubempresas(int idSub);
        string? GetSubEmpresasNombre(int idEmpresa_madre);
        List<SubEmpresasVM> GetSubEmpresasPorEmpresa(int idEmpresa);
        Usuario? GetUsuarioConRelaciones(string nombreusuario);
        Usuario? Login(string nombreusuario, string contrasenia);
        List<SubEmpresa> ObtenerSubEmpresas();
    }
}
