namespace InnoMasketss.Models.ViewModels
{
    public class PostDetalleViewModels
    {
        public Post? Post { get; set; }
        public List<Comentario>? ComentariosPrincipales { get; set; }
        public List<Comentario>? ComentariosHijos { get; set; }
        public List<Comentario>? ComentariosNietos { get; set; }

        public List<Post>? PostRecientes { get; set; }
    }
}
