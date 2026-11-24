namespace SistemaVacaciones.Core.ViewModels
{
    public class RolVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool BitAdministrador { get; set; }
        public List<UsuariosVM> UsuariosVMs { get; set; } = new List<UsuariosVM>();
    }
}
