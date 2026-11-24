using Proyecto_Estadias.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.ViewModels
{
    public class MotivosAusenciaVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del motivo es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Nombre del Motivo")]
        public string NombreMotivo { get; set; }

        [Display(Name = "¿Descuenta días?")]
        public bool BitDescuentoDias { get; set; }
        [Display(Name = "¿descuenta horas?")]
        public bool BitDescuentoHoras { get; set; }
        public PermisosVM PermisosVM { get; set; }
    
    }
}