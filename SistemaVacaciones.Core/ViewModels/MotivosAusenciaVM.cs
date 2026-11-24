namespace SistemaVacaciones.Core.ViewModels
{
    public class MotivosAusenciaVM
    {
        public int Id { get; set; }
        public string NombreMotivo { get; set; } = string.Empty;
        public bool BitDescuentoDias { get; set; }
        public bool BitDescuentoHoras { get; set; }
    }
}
