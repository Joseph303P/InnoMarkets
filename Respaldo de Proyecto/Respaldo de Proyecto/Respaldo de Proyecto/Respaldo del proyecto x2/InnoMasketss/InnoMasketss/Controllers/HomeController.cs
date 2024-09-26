using InnoMasketss.Data.Enums;
using InnoMasketss.Data.Servicios;
using InnoMasketss.Data;
using InnoMasketss.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace InnoMasketss.Controllers
{
    public class HomeController : Controller
    {
        private readonly Contexto _contexto;
        private readonly PostServicio _postServicio;

        public HomeController(Contexto contexto)
        {
            _contexto = contexto;
            _postServicio = new PostServicio(contexto);
        }

        public IActionResult Index(string categoria, string buscar, int? pagina)
        {
            try
            {
                var post = new List<Post>();
                if (string.IsNullOrEmpty(categoria) && string.IsNullOrEmpty(buscar))
                    post = _postServicio.ObtenerPosts();
                else if (!string.IsNullOrEmpty(categoria))
                {
                    // Que categoria nosea nula
                    var categoriaEnum = Enum.Parse<CategoriaEnum>(categoria);
                    post = _postServicio.ObtenerPostsCategoria(categoriaEnum);

                    if (post.Count == 0)
                        ViewBag.Error = $"No se encontraron publicaciones en la categoria {categoriaEnum}.";


                }
                else if (!string.IsNullOrEmpty(buscar))
                {
                    // Que buscar nosea nulo
                    //Buscar Por titulo
                    post = _postServicio.ObtenerPostsTitulo(buscar);
                    if (post.Count == 0)
                        ViewBag.Error = $"Nose encontraron publicaciones en la categoria {buscar}.";



                }
                //Muestra cantidad por pagina
                int pageSize = 6;
                int pageNumber = (pagina ?? 1);

                string descripcioncategoria = !string.IsNullOrEmpty(categoria) ? CategoriaEnumHelper.ObtenerDescripcion(Enum.Parse<CategoriaEnum>(categoria)) : "Todas las demas";
                ViewBag.CategoriaDescripcion = descripcioncategoria;

                return View(post.ToPagedList(pageNumber, pageSize));
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }
}
