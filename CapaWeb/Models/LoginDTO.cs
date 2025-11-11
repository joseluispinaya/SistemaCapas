using System.ComponentModel.DataAnnotations;

namespace CapaWeb.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Ingrese su correo")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "Ingrese su clave")]
        public string Clave { get; set; } = null!;
    }
}
