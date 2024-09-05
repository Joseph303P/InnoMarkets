//Completar Informacion Pendiente

using System.ComponentModel.DataAnnotations;

namespace InnoMarkets.Models;

public class Usuario
{
    public int UsuariosId { get; set;}

    [Required(ErrorMessage ="El Campo Nombre es Oblgatorio.")]
    [StringLength(50, ErrorMessage ="El Campo Nombre debe tener como maximo {1} caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage ="El Campo Apellido es Oblgatorio.")]
    [StringLength(50, ErrorMessage ="El Campo Apellido debe tener como maximo {1} caracteres")]
    public string Apellido { get; set; }

    [Required(ErrorMessage ="El Campo Correo es Oblgatorio.")]
    [EmailAddress(ErrorMessage ="El Campo Correo no es una direccion de correo electronico valida")]
    [StringLength(50, ErrorMessage ="El Campo Correo debe tener como maximo {1} caracteres")]
    public string Correo { get; set; }

    [Required(ErrorMessage ="El Campo Contraseña es Oblgatorio.")]
    [StringLength(100, MinimumLength =6, ErrorMessage ="El Campo Contraseña debe tener entre {2} y {1} caracteres")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; }

    [Required(ErrorMessage ="El Campo RolId es Oblgatorio.")]
    public int RolId { get; set; }
    public Roles Roles { get; set; }

    [Required(ErrorMessage ="El Campo Usuario es Oblgatario.")]
    [StringLength(50, ErrorMessage ="El Campo Usuario debe tener como maximo {1} caracteres")]
    public string NombreUsuario { get; set; }
    public Boolean Estado { get; set; }
    public string Token {get; set; }
    public DateTime FechaExpiracion { get; set; }
}