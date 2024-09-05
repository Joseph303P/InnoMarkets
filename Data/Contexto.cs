namespace InnoMarkets.Data
{
    public class Contexto
    {

        public string conexion { get; }

        public Contexto(string valor)
        {
            conexion = valor!;
        }
    }
}