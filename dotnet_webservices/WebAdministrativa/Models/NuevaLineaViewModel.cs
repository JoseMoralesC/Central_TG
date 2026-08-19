using System.ComponentModel.DataAnnotations;

namespace WebAdministrativa.Models
{
    public class NuevaLineaViewModel
    {
        [Required(ErrorMessage = "El numero telefonico es obligatorio.")]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "El numero debe tener 8 digitos.")]
        [Display(Name = "Numero telefonico")]
        public string NumeroTelefono { get; set; }

        [Required(ErrorMessage = "El identificador de telefono es obligatorio.")]
        [RegularExpression("^[0-9]{16}$", ErrorMessage = "El identificador debe tener 16 digitos.")]
        [Display(Name = "Identificador de telefono")]
        public string IdentificadorTelefono { get; set; }

        [Required(ErrorMessage = "El identificador de tarjeta es obligatorio.")]
        [RegularExpression("^[0-9]{19}$", ErrorMessage = "El identificador debe tener 19 digitos.")]
        [Display(Name = "Identificador de tarjeta")]
        public string IdentificadorTarjeta { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de linea.")]
        public string Tipo { get; set; }
    }
}
