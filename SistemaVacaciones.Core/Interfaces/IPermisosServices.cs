using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IPermisosServices
    {
        bool CambiarEstadoPermiso(int idPermiso, string nuevoEstado, out string error);
        bool CrearPermiso(PermisosVM model, out string error);
        bool CrearSolicitud(PermisosVM model, out string error);
        Empleado? ObtenerEmpleadoPorId(int id);
        List<Empleado> ObtenerEmpleados();
        List<SubEmpresa> ObtenerEmpresas();
        List<MotivoAusencia> ObtenerMotivos();
        PermisosPdfVM? ObtenerPermisoPorId(int idPermiso);
        List<Permiso> ObtenerPermisosConDetalles();
        Tuple<Empleado?, List<Permiso>> ObtenerPermisosPorEmpleados(int idEmpleado);
        Tuple<Empleado?, List<Permiso>> ObtenerPermisosPorEmpleadosA(int idEmpleado);
        List<Permiso> ObtenerPermisosPorEmpresa(int idEmpresa);
        List<Permiso> ObtenerPermisosPorEmpresaConFiltros(int idEmpresa, string? nombre, int? departamento, int? motivo);
        List<Permiso> PermisosAll();
        List<Permiso> PermisosPorEmpresa(int idEmpresa);
    }
}
