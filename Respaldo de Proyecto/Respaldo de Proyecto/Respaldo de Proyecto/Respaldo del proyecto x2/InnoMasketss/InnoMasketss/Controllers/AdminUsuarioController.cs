using InnoMasketss.Data;
using InnoMasketss.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Data;
using InnoMasketss.Data.Servicios;
using X.PagedList;

namespace InnoMasketss.Controllers
{
    public class AdminUsuarioController : Controller
    {
        private readonly Contexto _contexto;

        private readonly UsuarioServicio _usuarioServicio;

        public AdminUsuarioController(Contexto contexto)
        {
            _contexto = contexto;
            _usuarioServicio = new UsuarioServicio(contexto);
        }

        public IActionResult Index(string buscar, int? pagina)
        {
            try
            {
                var usuarios = _usuarioServicio.ListarUsuarios();
                if (!String.IsNullOrEmpty(buscar))

                    //Buscar mediante el correo o Nombre de usuario
                    usuarios=usuarios.Where(u=>u.Correo!=null && u.Correo.Contains(buscar) || u.NombreUsuario !=null && u.NombreUsuario.Contains(buscar)).ToList();
                usuarios= usuarios.OrderBy(u => u.NombreUsuario).ToList();

                List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
                {
                    //Se mostrara su rol ya sea administrador o usuario
                    Value = r.RolId.ToString(),
                    Text = r.Nombre
                }).ToList();
                ViewBag.Roles = roles;

                int pageSize = 10;
                int pageNumber = (pagina ?? 1);  //si no re asigna ningun numerode pagina se establecera a la pagina 1
                var usuariospaginados = usuarios.ToPagedList(pageNumber, pageSize);

                return View(usuariospaginados);  //Datos ya actualizados
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Create()
        {
            try
            {
                //Lista desplegable de roles
                List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
                {
                    Value = r.RolId.ToString(),
                    Text = r.Nombre
                }).ToList();
                ViewBag.Roles = roles;
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("RegistrarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
                        cmd.Parameters.AddWithValue("@Contrasena", hashedPassword);
                        cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                        cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
                        cmd.Parameters.AddWithValue("@Token", usuario.Token);
                        cmd.Parameters.AddWithValue("@FechaExpiracion", usuario.FechaExpiracion);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        public IActionResult Edit(int id)
        {
            try
            {
                var usuario = _usuarioServicio.ObtenerUsuarioId(id);
                if (usuario == null)
                    return NotFound();

                List<SelectListItem> roles = _usuarioServicio.ListarRoles().Select(r => new SelectListItem
                {
                    Value = r.RolId.ToString(),
                    Text = r.Nombre
                }).ToList();
                ViewBag.Roles = roles;

                return View(usuario);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuariosId) //Si no coincide el id con ningun registro enviara mensaje de "No Enconttrado"
                return NotFound();
            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("ActualizarUsuarios", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuariosId", usuario.UsuariosId);
                        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                }
                return RedirectToAction("Index");

            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var usuario = _usuarioServicio.ObtenerUsuarioId(id);
                if (usuario == null)
                    return NotFound();

                return View(usuario);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();

            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("EliminarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UsuariosId", id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View(_usuarioServicio.ObtenerUsuarioId(id));
            }
        }
    }
}
