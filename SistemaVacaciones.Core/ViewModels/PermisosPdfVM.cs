using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.ViewModels
{
    public class PermisosPdfVM
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoPermiso { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public Empleado? Empleados { get; set; }
        public int DiasDisponibles { get; set; }
        public SubEmpresa? SubEmpresas { get; set; }
        public MotivoAusencia? MotivosAusencia { get; set; }
        public int DiasLaboralesSemana { get; set; }
        public string? Estado { get; set; }
        public int? CantidadHorasSolicitadas { get; set; }
        public int? HorasDisponibles { get; set; }
    }
}
