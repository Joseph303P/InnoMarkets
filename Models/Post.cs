using System.ComponentModel.DataAnnotations;

namespace InnoMarkets.Models
{

public class Post
{
    public int PostId { get; set; }
    [Required(ErrorMessage ="El Titulo es Requerido.")]
    [StringLength(100, MinimumLength =5, ErrorMessage ="El Titulo debe tener entre 5 y 100 Caracteres.")]
    
    public string Titulo { get; set;} 
    [Required(ErrorMessage ="El Contenido es Requerido.")]
    [StringLength(100, MinimumLength =5, ErrorMessage ="El Contenido debe tener entre 50 y 5000 Caracteres.")]

    public string Contenido { get; set; }
    [Required(ErrorMessage ="La Categoria es Requerida.")]

    public CategoriaEnum Categoria { get; set; }
    [Required(ErrorMessage ="Fecha de Creacion es Requerida.")]


    public DateTime FechaCreacion {get; set; }

}

    public class CategoriaEnum
    {
    }
}