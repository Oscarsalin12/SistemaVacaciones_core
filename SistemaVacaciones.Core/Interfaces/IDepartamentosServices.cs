using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IDepartamentosServices
    {
        void Agregar(DepartamentosVM departamento);
        void Editar(DepartamentosVM depNuevo);
        void Eliminar(int id);
        List<DepartamentosVM> Listar();
        DepartamentosVM? ObtenerPorId(int id);
    }
}
