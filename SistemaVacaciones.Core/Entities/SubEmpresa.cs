using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("SubEmpresas")]
    public class SubEmpresa
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Empresa))]
        public int IdEmpresa_madre { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Rfc { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public byte[]? Logo { get; set; }

        [StringLength(100)]
        public string? Sucursal { get; set; }

        public virtual Empresa? Empresa { get; set; }
        [NotMapped]
        public virtual Empresa? Empresas => Empresa;
        public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
        public virtual ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
        public virtual ICollection<UsuarioEmpresa> UsuariosEmpresas { get; set; } = new List<UsuarioEmpresa>();
    }
}
