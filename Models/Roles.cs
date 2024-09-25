namespace InnoMasketss.Models
{
    public class Roles
    {
        public int RolId { get; set; }
        public string? Nombre { get; set; }
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
