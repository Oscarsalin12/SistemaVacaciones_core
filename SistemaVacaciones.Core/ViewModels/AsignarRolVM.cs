using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaVacaciones.Core.ViewModels
{
    public class AsignarRolVM
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public int IdEmpresa { get; set; }
        public List<SelectListItem> ListaRoles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaEmpresas { get; set; } = new List<SelectListItem>();
    }
}
