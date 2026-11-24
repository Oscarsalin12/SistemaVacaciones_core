namespace SistemaVacaciones.Core.ViewModels
{
    public class UsuariosVM
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
        public bool BitAdministrador { get; set; }
        public List<int> Roles { get; set; } = new List<int>();
        public List<int> Empresas { get; set; } = new List<int>();
        public List<UsuariosRolVM> UsuariosRolesVMs { get; set; } = new List<UsuariosRolVM>();
    }
}
