using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAdministrativa.Models
{
    public class Usuario
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El usuario es obligatorio.")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contrasena es obligatoria.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Contrasena")]
        public string Contrasena { get; set; }
    }
}
