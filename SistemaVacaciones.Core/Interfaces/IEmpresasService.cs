using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IEmpresasService
    {
        void InsertarEmpresa(Empresa empresa, byte[]? logoBytes);
        void InsertarSubEmpresas(SubEmpresa subEmpresa, byte[]? logoBytes);
    }
}
