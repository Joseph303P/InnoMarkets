using System.ComponentModel;
using System.Reflection;

namespace InnoMarkets.Data.Enums;

public class CategoriaEnumHelper
{
    public static string ObtenerDescripcion(CategoriaEnum categoria)
    {
        FieldInfo field = categoria.GetType().GetField(categoria.ToString());

        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute != null ? attribute.Description : categoria.ToString();
    }
}