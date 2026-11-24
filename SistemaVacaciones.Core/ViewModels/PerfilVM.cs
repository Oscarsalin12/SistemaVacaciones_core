using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.ViewModels
{
    public class PerfilVM
    {
        public Usuario? Usuario { get; set; }
        public Empleado? Empleado { get; set; }
        public List<Empresa> Empresas { get; set; } = new List<Empresa>();
        public List<SubEmpresa> SubEmpresas { get; set; } = new List<SubEmpresa>();
        public Dictionary<int, List<EmpleadosVM>> EmpleadosPorEmpresa { get; set; } = new Dictionary<int, List<EmpleadosVM>>();
    }
}
