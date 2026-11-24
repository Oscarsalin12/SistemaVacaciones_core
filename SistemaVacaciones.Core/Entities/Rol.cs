using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Roles")]
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<UsuarioEmpresa> UsuariosEmpresas { get; set; } = new List<UsuarioEmpresa>();
    }
}
