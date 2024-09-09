using System.ComponentModel;
using System.Reflection;

namespace InnoMarkets.Data.Enums;

//Obtiene la Descripcion de la categoria a trevez de un atributo descripcion
public class CategoriaEnumHelper
{
    public static string ObtenerDescripcion(CategoriaEnum categoria)
    {
        FieldInfo field = categoria.GetType().GetField(categoria.ToString());

        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute != null ? attribute.Description : categoria.ToString();
    }
}