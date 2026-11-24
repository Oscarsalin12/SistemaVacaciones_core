using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("MotivosAusencia")]
    public class MotivoAusencia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreMotivo { get; set; } = string.Empty;

        public bool BitDescuentoDias { get; set; }

        public bool BitDescuentoHoras { get; set; }

        public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
    }
}
