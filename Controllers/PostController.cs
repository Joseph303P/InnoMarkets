using System.Data.SqlClient;
using System.Net;
using InnoMarkets.Data;
using InnoMarkets.Data.Servicios;
using InnoMarkets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Collections.ObjectModel;

namespace InnoMarkets.Controllers
{
    public class PostController : Controller
    {
        private readonly Contexto _contexto;
        private readonly PostServicio _postServicio;

        public PostController(Contexto con)
        {{
            _contexto = con;
            _postServicio = new PostServicio(con);
        }}

        [Authorize(Roles="Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles="Administrador")]
        public IActionResult Create(Post post)
        {
            using (var connection = new SqlConnection( _contexto.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("IngresarPost", connection))
                {
                    command.CommandType=CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Titulo", post.Titulo);
                    command.Parameters.AddWithValue("@Contenido", post.Contenido);
                    command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                    DateTime fc=DateTime.UtcNow;
                    command.Parameters.AddWithValue("@FechaCreacion", fc);
                    command.ExecuteNonQuery();
                }

            }

            return RedirectToAction("Index", "Home");
            
        }
        
        [Authorize(Roles="Administrador")]
        public IActionResult Update(int id)
        {
            var post = _postServicio.ObtenerPostPorId(id);
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles="Administrador")]
        public IActionResult Update(Post post)
        {
            using (var connection = new SqlConnection( _contexto.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ActualizarPost", connection))
                {
                    command.CommandType=CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", post.PostId);
                    command.Parameters.AddWithValue("@Titulo", post.Titulo);
                    command.Parameters.AddWithValue("@Contenido", post.Contenido);
                    command.Parameters.AddWithValue("@Categoria", post.Categoria.ToString());
                    command.ExecuteNonQuery();
                }

            }

            return RedirectToAction("Index", "Home");
            
        }

        [HttpPost]
        [Authorize(Roles="Administrador")]
        public IActionResult Delete(int id)
        {
            using (var connection = new SqlConnection( _contexto.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("EliminarPost", connection))
                {
                    command.CommandType=CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    command.ExecuteNonQuery();
                }

            }

            return RedirectToAction("Index", "Home");
            
        }

        public IActionResult Details(int id)
        {
            var post = _postServicio.ObtenerPostPorId(id);
            var Comentario = _postServicio.ObtenerComentariosPorPostId(id);
            Comentario = _postServicio.ObtenerComentariosHijos(Comentario);
            Comentario = _postServicio.ObtenerComentariosNietos(Comentario);   

            var models = new PostDetalleViewModels  
            {
                Post=post,
                ComentariosPrincipales=Comentario.Where(c=>c.ComentarioPrincipalId==null && c.ComentarioTerciariosId == null).ToList(),
                ComentariosHijos=Comentario.Where(c=>c.ComentarioPrincipalId! == null && c.ComentarioTerciariosId==null).ToList(),
                ComentariosNietos=Comentario.Where(c=>c.ComentarioTerciariosId!=null).ToList(),
                PostRecientes=_postServicio.ObtenerPosts().Take(10).ToList()
            };

            return View(models);       
        }

        [HttpPost]
        public IActionResult AgregarComentario(int postId, string Comentario, int? comentariopadreid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Comentario))
                {
                    ViewBag.Error = "El comentario no puede estar vacio.";
                    return RedirectToAction("Details", "Post", new { id = postId });
                }

                int? userId  = null ;
                var userIdClaim = User.FindFirst("IdUsuario");
                if (userIdClaim != null  && int.TryParse(userIdClaim.Value, out int parsedUserId))
                userId = parsedUserId;

                DateTime fechaPublicacion = DateTime.UtcNow;

                using (SqlConnection con = new (_contexto.Conexion))
                {
                    using (SqlCommand cmd = new ("AgregarComentario", con))
                    {
                        cmd.CommandType=CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Contenido", SqlDbType.VarChar).Value=Comentario;
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