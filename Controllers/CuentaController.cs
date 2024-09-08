using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using BCrypt.Net;
using InnoMarkets.Data;
using InnoMarkets.Data.Servicios;
using InnoMarkets.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace InnoMarkets.Controllers
{
    public class CuentaController : Controller
    {
        private readonly Contexto _contexto;

        private readonly UsuarioServicio _usuarioServicio;

        public CuentaController (Contexto con)
        {
            _contexto = con;
            _usuarioServicio = new UsuarioServicio(con);
        }

        public IActionResult Registrar()
        {
            return View();
        }
        
        [HttpPost, ValidateAntiForgeryToken]

        public IActionResult Registrar(Usuario model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(_contexto.Conexion))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("RegistrarUsuario", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Nomre", model.Nombre);
                            command.Parameters.AddWithValue("@Apellido", model.Apellido);
                            command.Parameters.AddWithValue("@Correo", model.Correo);
                            string hashedPasswork = BCrypt.Net.BCrypt.HashPassword(model.Contrasena);
                            command.Parameters.AddWithValue("@Contrasena", hashedPasswork);
                            command.Parameters.AddWithValue("@NombreUsuario", model.NombreUsuario);
                            DateTime fechaexpiracion = DateTime.UtcNow.AddMinutes(5);
                            command.Parameters.AddWithValue("@FechaExpiracion", fechaexpiracion);
                            var token = Guid.NewGuid();
                            command.Parameters.AddWithValue("@Token", token);
                            command .ExecuteNonQuery();

                            Email email = new();
                            if (model.Correo != null)
                            email.Enviar(model.Correo, token.ToString());
                        }
                    }
                    return RedirectToAction("Token");
                }
                catch (SqlException ex)
                {
                    
                    if (ex.Number == 2627)
                    ViewBag.Error = "El correo electorico y/o nombre de usuario ya existe!.";
                    else
                    ViewBag.Error = "Ocurrio al intentar ingresar al usuario.";
                    throw;
                }
            }
            return View();
        }

        public IActionResult Token()
        {
            string token = Request.Query["valor"];

            if (token != null)
            {
              try
              {
                using (SqlConnection con = new SqlConnection(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new SqlCommand("ActivarCuenta", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Token", token);
                        DateTime fechaExpiracion = DateTime.UtcNow;
                        cmd.Parameters.AddWithValue("@Fecha", fechaExpiracion);
                        con.Open();
                        var resultado = cmd.ExecuteScalar();

                        int activada = Convert.ToInt32(resultado);
                        
                        if (activada == 1)
                        ViewBag["mensaje"] = "Su cuenta ha sido validada exitosamente";
                        else
                        ViewBag["mensaje"] = "Su enlace de activacion ha expirado";

                        con.Close();
                        
                    }
                }
              }
              catch (System.Exception e)
              {
                
                ViewData["mensaje"] = e.Message;
                return View();
              }  
              
            }
            else
            {
                ViewData["mensaje"] = "Verifique su correo para activar su cuenta";
                return View();
            }
            return View();
        }

        public IActionResult Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if (c.Identity != null)
            {
              if (c.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");  
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModels model)
        {
            if (!ModelState.IsValid)
            return View(model);

            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("ValidarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Correo", model.Correo);
                        con.Open(); 
                        try
                        {
                            using (var dr=cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    bool passwordMatch=BCrypt.Net.BCrypt.Verify(model.Contrasena, dr["Contrasena"].ToString());
                                    if (passwordMatch)
                                    {
                                        DateTime fechaexpiracion = DateTime.UtcNow;

                                        if (!(bool)dr["Estado"] && dr["FechaExpiracion"].ToString()!=fechaexpiracion.ToString())
                                        {
                                            if (model.Correo != null)
                                            _usuarioServicio.ActualizarToken(model.Correo);

                                                ViewBag.Error = "Su cuenta no ha sido activada, se ha reenviado un correo de activacio, verique su bandeja";

                                            
                                        }
                                        else if(!(bool)dr["Estado"])
                                            ViewBag.Error= "Su cuenta no ha sido activada, verifique su bandeja de entrada";

                                        else
                                        {
                                            string nombreusuario = dr["NombreUsuario"].ToString();
                                            int idUsuario = (int)dr["UsuariosId"];

                                            if (nombreusuario != null)
                                            {
                                                var claims = new List<Claim>()
                                                {
                                                    new Claim(ClaimTypes.NameIdentifier, nombreusuario),
                                                    new Claim("IdUsuario", idUsuario.ToString())
                                                };

                                                int rolId = (int)dr["RolId"];
                                                string rolNombre = rolId == 1? "Administrador" : "Usuario";
                                                claims.Add(new Claim(ClaimTypes.Role, rolNombre));

                                                var identity= new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                                var propiedades = new AuthenticationProperties
                                                {
                                                    AllowRefresh = true,
                                                    IsPersistent = model.MantenerActivo,
                                                    ExpiresUtc = DateTimeOffset.UtcNow.Add(model.MantenerActivo ? TimeSpan.FromDays(1): TimeSpan.FromMinutes(5))

                                                };

                                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), propiedades);
                                                return RedirectToAction("Index", "Home");
                                            }
                                        }
                                    }
                                }
                                else
                                ViewBag.Error = "Correo no registrado";
                                dr.Close();
                            }
                        }
                        finally
                        {
                            if (cmd !=  null)
                            {
                                cmd.Dispose();
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                
                ViewBag.Error = ex.Message;
            }
            return View(model);
                
            
        }        

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
    
}