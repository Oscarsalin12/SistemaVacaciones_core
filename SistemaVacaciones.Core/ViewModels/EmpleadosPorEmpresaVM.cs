namespace SistemaVacaciones.Core.ViewModels
{
    public class EmpleadosPorEmpresaVM
    {
        public string? Busqueda { get; set; }
        public int? IdSubEmpresa { get; set; }
        public List<EmpleadosVM> Empleados { get; set; } = new List<EmpleadosVM>();
    }
}
