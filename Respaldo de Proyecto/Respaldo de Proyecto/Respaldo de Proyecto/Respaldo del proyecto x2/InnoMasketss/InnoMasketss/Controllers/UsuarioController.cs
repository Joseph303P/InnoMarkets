using InnoMasketss.Data.Servicios;
using InnoMasketss.Data;
using InnoMasketss.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace InnoMasketss.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Contexto _contexto;
        private readonly UsuarioServicio _usuarioServicio;

        public UsuarioController(Contexto contexto)
        {
            _contexto = contexto;
            _usuarioServicio = new UsuarioServicio(contexto);
        }

        //Metodo de perfil de usuario
        [Authorize]
        public ActionResult Perfil()
        {
            try
            {
                int userId = 0;
                var userIdClaim = User.FindFirst("UsuariosId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;

                Usuario usuario = _usuarioServicio.ObtenerUsuarioId(userId);
                return View(usuario);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult ActualizarPerfil(Usuario model)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("ActualizarPerfil", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuariosId", model.UsuariosId);
                        cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", model.Apellido);
                        cmd.Parameters.AddWithValue("@Correo", model.Correo);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Perfil");
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Perfil");
            }
            
        }

        [HttpPost]

        public ActionResult EliminarCuenta()
        {
            try
            {
                int userId = 0;
                var userIdClaim = User.FindFirst("UsuariosId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                    userId = parsedUserId;

                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("EliminarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuariosId", userId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //Cierre de la sesion
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Perfil");
            }
        }
    }
}
