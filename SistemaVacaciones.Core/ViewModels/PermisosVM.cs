using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.ViewModels
{
    public class PermisosVM
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoPermiso { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public string? NombreEmpleado { get; set; }
        public string? EmpresaNombre { get; set; }
        public int VacacionesDisponibles { get; set; }
        public int DiasLaboralesSemana { get; set; }
        public int? CantidadHorasSolicitadas { get; set; }
        public int HorasDisponibles { get; set; }

        public List<Empresa> Empresas { get; set; } = new List<Empresa>();
        public List<Empleado> Empleados { get; set; } = new List<Empleado>();
        public List<SubEmpresa> SubEmpresas { get; set; } = new List<SubEmpresa>();
        public List<MotivoAusencia> MotivosAusencia { get; set; } = new List<MotivoAusencia>();
    }
}
