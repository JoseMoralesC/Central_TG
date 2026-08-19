using System.ComponentModel.DataAnnotations;

namespace WebAdministrativa.Models
{
    public class ActivarLineaViewModel
    {
        [Required]
        public int IdLinea { get; set; }

        [Required(ErrorMessage = "La identificacion del cliente es obligatoria.")]
        [Display(Name = "Identificacion del cliente")]
        public string IdentificacionCliente { get; set; }
    }
}
