namespace SistemaVacaciones.Core.ViewModels
{
    public class UsuariosRolVM
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdEmpresa { get; set; }
        public int IdEmpresaMadre { get; set; }
        public EmpresaVM? EmpresaVM { get; set; }
        public RolVM? RolesVM { get; set; }
        public UsuariosVM? UsuariosVM { get; set; }
    }
}
