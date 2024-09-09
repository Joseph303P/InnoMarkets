using System.Data.SqlClient;
using InnoMarkets.Data;
using InnoMarkets.Data.Servicios;
using InnoMarkets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Xml.Linq;

namespace InnoMarkets.Controllers;

public class UsuarioController : Controller
{
    private readonly  Contexto _contexto;
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
        int  userId = 0;
        var userIdClain = User.FindFirst("UsuariosId");
        if (userIdClain != null && int.TryParse(userIdClain.Value, out int parsedUserId))
        userId = parsedUserId;

        Usuario usuario = _usuarioServicio.ObtenerUsuarioId(userId);
        return View();
    }

    [HttpPost]
    public ActionResult ActualizarPerfil(Usuario model)
    {
        using (SqlConnection con = new SqlConnection(_contexto.Conexion))
        {
            using (SqlCommand cmd = new ("ActualizarPerfil", con))
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
    
    [HttpPost]

    public ActionResult EliminarCuenta()
    {
        int  userId = 0;
        var userIdClain = User.FindFirst("UsuariosId");
        if (userIdClain != null && int.TryParse(userIdClain.Value, out int parsedUserId))
        userId = parsedUserId;

        using (SqlConnection con = new(_contexto.Conexion))
        {
            using (SqlCommand cmd = new("EliminarUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UsariosId", userId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //Cierre de la sesion
            return RedirectToAction("Index", "Home");
        }
    }
}