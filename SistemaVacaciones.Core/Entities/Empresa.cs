using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("Empresas")]
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Rfc { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public byte[]? Logo { get; set; }

        public virtual ICollection<SubEmpresa> SubEmpresas { get; set; } = new List<SubEmpresa>();
        public virtual ICollection<UsuarioEmpresa> UsuariosEmpresas { get; set; } = new List<UsuarioEmpresa>();
    }
}
