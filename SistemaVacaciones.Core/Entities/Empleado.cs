using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        public int NumEmpleado { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaIDeIngreso { get; set; }

        public int AniosTrabajados { get; set; }

        public int MesesTrabajados { get; set; }

        public int intDiasVacaciones { get; set; }

        [ForeignKey(nameof(SubEmpresa))]
        public int IdEmpresa { get; set; }

        public int DiasDisponibles { get; set; }

        [ForeignKey(nameof(Departamento))]
        public int IdDepartamento { get; set; }

        public int DiasLaboralesPorSemana { get; set; }

        [StringLength(100)]
        public string? UsuarioEmpleado { get; set; }

        public int HorasPorDia { get; set; }

        public int HorasRestantesDelDia { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; }

        public virtual Departamento? Departamento { get; set; }
        [NotMapped]
        public virtual Departamento? Departamentos => Departamento;

        public virtual SubEmpresa? SubEmpresa { get; set; }
        [NotMapped]
        public virtual SubEmpresa? SubEmpresas => SubEmpresa;

        public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
    }
}
