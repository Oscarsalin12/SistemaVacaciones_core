using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Permisos")]
    public class Permiso
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Empleado))]
        public int IdEmpleado { get; set; }

        [ForeignKey(nameof(MotivoAusencia))]
        public int IdTipoPermiso { get; set; }

        [ForeignKey(nameof(SubEmpresa))]
        public int IdEmpresa { get; set; }

        public DateTime FechaGeneracion { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFinal { get; set; }

        public int DiasDisponibles { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; }

        public int CantidadHorasSolicitadas { get; set; }

        public int HorasDisponibles { get; set; }

        public virtual Empleado? Empleado { get; set; }
        [NotMapped]
        public virtual Empleado? Empleados => Empleado;

        public virtual MotivoAusencia? MotivoAusencia { get; set; }
        [NotMapped]
        public virtual MotivoAusencia? MotivosAusencia => MotivoAusencia;

        public virtual SubEmpresa? SubEmpresa { get; set; }
        [NotMapped]
        public virtual SubEmpresa? SubEmpresas => SubEmpresa;
    }
}
