namespace InnoMasketss.Data
{
    public class Contexto
    {
        public string Conexion { get; }

        //Toma una cadena de conexion en su constructor y la almacena en una propiedad conexion
        public Contexto(string valor)
        {
            Conexion = valor!;
        }
    }
}
