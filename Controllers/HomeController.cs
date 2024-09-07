using InnoMarkets.Data;
using InnoMarkets.Data.Enums;
using InnoMarkets.Data.Servicios;
using InnoMarkets.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;
public class HomeController : Controller
{
    private readonly Contexto _contexto;
    private readonly PostServicio _postServicio;

    public HomeController (Contexto contexto)
    {
        _contexto = contexto;
        _postServicio = new PostServicio(contexto);
    }

    public IActionResult Index(string categoria, string buscar, int? pagina)
    {
        var post = new List<Post>();
        if (string.IsNullOrEmpty(categoria) && string.IsNullOrEmpty(buscar))
           post = _postServicio.ObtenerPosts();
        else if (!string.IsNullOrEmpty(categoria))
        {
            var categoriaEnum= Enum.Parse<CategoriaEnum>(categoria);
            post= _postServicio.ObtenerPostsCategoria(categoriaEnum);

            if (post.Count == 0)
            ViewBag.Error = $"No se encontraron publicaciones en la categoria {categoriaEnum}.";

          
        }
        else if(!string.IsNullOrEmpty(buscar))
        {
            post = _postServicio.ObtenerPostsTitulo(buscar);
            if (post.Count == 0)
            ViewBag.Error = $"Nose encontraron publicaciones en la categoria {buscar}.";


            
        }
        int pageSize = 6;
        int pageNumber = (pagina ?? 1);

        string descripcioncategoria = !string.IsNullOrEmpty(categoria) ? CategoriaEnumHelper.ObtenerDescripcion(Enum.Parse<CategoriaEnum>(categoria)) : "Todas las demas";
        ViewBag.CategoriaDescripcion = descripcioncategoria;

        return View(post.ToPagedList(pageNumber, pageSize));
        
    }
    
}
