using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Departamentos")]
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? ClaveDep { get; set; }

        [StringLength(100)]
        public string? NombreDep { get; set; }

        [StringLength(150)]
        public string? NombreEmcargado { get; set; }

        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
    }
}
