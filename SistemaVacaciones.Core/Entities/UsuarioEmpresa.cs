using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVacaciones.Core.Entities
{
    [Table("UsuariosEmpresas")]
    public class UsuarioEmpresa
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int IdUsuario { get; set; }

        [ForeignKey(nameof(Empresa))]
        public int IdEmpresa { get; set; }

        [ForeignKey(nameof(Rol))]
        public int IdRol { get; set; }

        [ForeignKey(nameof(SubEmpresa))]
        public int IdSubEmpresa { get; set; }

        public virtual Usuario? Usuario { get; set; }
        public virtual Empresa? Empresa { get; set; }
        public virtual Rol? Rol { get; set; }
        public virtual SubEmpresa? SubEmpresa { get; set; }
    }
}
