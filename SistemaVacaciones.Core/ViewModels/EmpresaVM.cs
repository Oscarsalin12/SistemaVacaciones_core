namespace SistemaVacaciones.Core.ViewModels
{
    public class EmpresaVM
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Rfc { get; set; }
        public string? Direccion { get; set; }
        public byte[]? Logo { get; set; }
        public RolVM? RolesVM { get; set; }
        public UsuariosVM? UsuariosVM { get; set; }
        public EmpleadosVM? EmpleadosVM { get; set; }
    }
}
