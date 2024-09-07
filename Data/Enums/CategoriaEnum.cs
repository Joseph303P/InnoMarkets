using System.ComponentModel;


namespace InnoMarkets.Data.Enums;

public enum CategoriaEnum
{
    [Description("Noticia recientes")]
    Noticia,
    [Description("Opiniones de Profesionales")]
    Opinion,
    [Description("Consejos Financieros")]
    Finanzas,
    [Description("Consejos de profesionales")]
    Consejos,
    [Description("Una planificacion Eficientes")]
    Planificacion,
}