namespace SistemaVacaciones.Core.ViewModels
{
    public class SubEmpresasVM
    {
        public int Id { get; set; }
        public int IdEmpresa_madre { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Sucursal { get; set; }
        public string? Rfc { get; set; }
        public string? Direccion { get; set; }
        public byte[]? Logo { get; set; }

        public EmpleadosVM? EmpleadosVM { get; set; }
        public PermisosVM? PermisosVM { get; set; }
        public EmpresaVM? EmpresaVM { get; set; }
    }
}
