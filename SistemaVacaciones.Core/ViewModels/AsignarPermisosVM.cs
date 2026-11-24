using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.ViewModels
{
    public class AsignarPermisosVM
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int IdEmpresa { get; set; }
        public List<int> idSubEmpresas { get; set; } = new List<int>();

        public List<Rol> ListaRoles { get; set; } = new List<Rol>();
        public List<Empresa> ListaEmpresas { get; set; } = new List<Empresa>();
        public List<SubEmpresa> ListaSubEmpresas { get; set; } = new List<SubEmpresa>();
    }
}
