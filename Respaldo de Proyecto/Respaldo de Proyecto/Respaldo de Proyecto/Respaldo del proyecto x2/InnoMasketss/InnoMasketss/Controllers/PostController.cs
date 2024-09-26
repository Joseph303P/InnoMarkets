using InnoMasketss.Data;
using InnoMasketss.Models.ViewModels;
using InnoMasketss.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using InnoMasketss.Data.Servicios;

namespace InnoMasketss.Controllers
{
    public class PostController : Controller
    {
        private readonly Contexto _contexto;
        private readonly PostServicio _postServicio;

        public PostController(Contexto con)
        {
            {
                _contexto = con;
                _postServicio = new PostServicio(con);
            }
        }
        //Restrincion de acceso a los usuario roles no autorizados
        //Vista para crear un nuevo post
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Post post)
        {
            try
            {
                using (var connection = new SqlConnection(_contexto.Conexion))
                {
                    connection.Open();
                    using (var command = new SqlCommand("IngresarPost", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Titulo", post.Titulo);
                        command.Parameters.AddWithValue("@Contenido", post.Contenido);
                        command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                        DateTime fc = DateTime.UtcNow;
                        command.Parameters.AddWithValue("@FechaCreacion", fc);
                        command.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Update(int id)
        {
            try
            {
                var post = _postServicio.ObtenerPostPorId(id);
                return View(post);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        //Funciona para actualizar un post identificandolo por id
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Update(Post post)
        {
            try
            {
                using (var connection = new SqlConnection(_contexto.Conexion))
                {
                    connection.Open();
                    using (var command = new SqlCommand("ActualizarPost", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PostId", post.PostId);
                        command.Parameters.AddWithValue("@Titulo", post.Titulo);
                        command.Parameters.AddWithValue("@Contenido", post.Contenido);
                        command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                        command.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        //Este controlador elimina post identificandolo por su id
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_contexto.Conexion))
                {
                    connection.Open();
                    using (var command = new SqlCommand("EliminarPost", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PostId", id);
                        command.ExecuteNonQuery();
                    }

                }

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        //Controlador que nos permite llamar a la publicacion mediante su idenficador
        //se muestra la informacion detallada de un pos mas comentarios asociados
        public IActionResult Details(int id)
        {
            try
            {
                var post = _postServicio.ObtenerPostPorId(id);
                var Comentario = _postServicio.ObtenerComentariosPorPostId(id);
                Comentario = _postServicio.ObtenerComentariosHijos(Comentario);
                Comentario = _postServicio.ObtenerComentariosNietos(Comentario);

                //
                var models = new PostDetalleViewModels
                {
                    Post = post,
                    ComentariosPrincipales = Comentario.Where(c => c.ComentarioPrincipalId == null && c.ComentarioTerciariosId == null).ToList(),
                    ComentariosHijos = Comentario.Where(c => c.ComentarioPrincipalId! == null && c.ComentarioTerciariosId == null).ToList(),
                    ComentariosNietos = Comentario.Where(c => c.ComentarioTerciariosId != null).ToList(),
                    PostRecientes = _postServicio.ObtenerPosts().Take(10).ToList()
                };

                return View(models);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        //Controlador que permite comentar en las publicaciones
        [HttpPost]                                                                 //Posible error
        public IActionResult AgregarComentario(int postId, string comentario, int? comentariopadreid)
        {
            try
            {
                //Una condicion para que el comentario no sea nulo
                if (string.IsNullOrWhiteSpace(comentario))
                {
                    ViewBag.Error = "El comentario no puede estar vacio.";
                    return RedirectToAction("Details", "Post", new { id = postId });
                }

                //Validar que haya un usuario autenticado
                int? userId = null;
                var userIdClaim = User.FindFirst("UsuariosId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;

                DateTime fechaPublicacion = DateTime.UtcNow;

                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("AgregarComentarios", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Contenido", SqlDbType.VarChar).Value = comentario;
                        cmd.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = fechaPublicacion;
                        cmd.Parameters.Add("PostId", SqlDbType.Int).Value = postId;
                        cmd.Parameters.Add("UsuariosId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("ComentarioPrincipalId", SqlDbType.Int).Value = comentariopadreid ?? (object)DBNull.Value;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }

                return RedirectToAction("Details", "Post", new { id = postId });

            }
            catch (System.Exception e)
            {

                ViewBag.Error = e.Message;
                return RedirectToAction("Details", "Post", new { id = postId });
            }
        }
    }
}
