using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IUsuariosService
    {
        List<Usuario> ListarUsuarios();
        Usuario? ObtenerUsuarioPorId(int id);
        int CrearUsuario(UsuariosVM model);
        void EditarUsuario(UsuariosVM model);
        void EliminarUsuario(int id);
        List<SubEmpresa> ObtenerTodasLasSubEmpresas();
        List<Rol> ObtenerTodosLosRoles();
        RolVM? ObtenerRolPorId(int id);
        int CrearRol(RolVM model);
        void EditarRol(RolVM model);
        void EliminarRol(int id);
        void AsignarRolYEmpresas(int idUsuario, int idRol, int idEmpresaMadre, List<int> idSubEmpresas);
        List<SubEmpresa> ObtenerEmpresasPermitidas(int idUsuario);
    }
}
