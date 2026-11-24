using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Usuario")]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Contrasenia { get; set; } = string.Empty;

        public bool BitAdministrador { get; set; }

        public virtual ICollection<UsuarioEmpresa> UsuariosEmpresas { get; set; } = new List<UsuarioEmpresa>();
    }
}
