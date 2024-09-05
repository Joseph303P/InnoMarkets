using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnoMarkets.Models;

public class Comentario
{
    public int ComentarioId { get; set; }
    public string Contenido { get; set; }
    public DateTime FechaCreacion {get; set; }
    public int UsuariosId {get; set; }
    public int PostId {get; set; }
    public int? ComentarioPrincipalId{ get; set; }
    public List<Comentario> ComentarioSecundarios { get; set; }
    [NotMapped]
    public string NombreUsuario {get; set; }
    public int? ComentarioTerciariosId { get; set; }

}