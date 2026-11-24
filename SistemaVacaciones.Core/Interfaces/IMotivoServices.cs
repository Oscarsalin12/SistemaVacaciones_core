using SistemaVacaciones.Core.ViewModels;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IMotivoServices
    {
        void Agregar(MotivosAusenciaVM model);
        void Editar(MotivosAusenciaVM model);
        void Eliminar(int id);
        List<MotivosAusenciaVM> Listar();
        MotivosAusenciaVM? ObtenerPorId(int id);
    }
}
