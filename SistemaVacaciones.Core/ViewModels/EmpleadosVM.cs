namespace SistemaVacaciones.Core.ViewModels
{
    public class EmpleadosVM
    {
        public int Id { get; set; }
        public int NumEmpleado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaDeIngreso { get; set; }
        public int AniosTrabajados { get; set; }
        public int MesesTrabajados { get; set; }
        public int intDiasVacaciones { get; set; }
        public int IdEmpresa { get; set; }
        public int DiasDisponibles { get; set; }
        public int IdDepartamento { get; set; }
        public int DiasLaboralesPorSemana { get; set; }
        public int HorasPorDia { get; set; }
        public int HorasRestantesDelDia { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? NombreDepartamento { get; set; }
        public string? Estado { get; set; }
        public string? UsuarioEmpleado { get; set; }

        public List<DepartamentosVM> DepatamentosVM { get; set; } = new List<DepartamentosVM>();
        public List<SubEmpresasVM> SubEmpresaVM { get; set; } = new List<SubEmpresasVM>();
    }
}
