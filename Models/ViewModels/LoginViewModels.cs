using System.ComponentModel.DataAnnotations;

namespace InnoMasketss.Models.ViewModels
{
    public class LoginViewModels
    {
        [Required(ErrorMessage = "El correo electronico es requerido.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo electronico valido.")]
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
        public bool MantenerActivo { get; set; }
    }
}
