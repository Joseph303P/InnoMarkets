using System.Net;
using System.Net.Mail;

namespace InnoMarkets.Data;

public class Email
{
    //Se utiliza para enviar un correo electronico al usuario
    public void Enviar(string correo, string token)
    {
        Correo(correo, token);
    }

    //Configura el correo electronico y enviarlo usando el protocolo smtp
    void Correo(string correo_receptor, string token)
    {
        string correo_emisor = "@hotmail.com";
        string Clave_emisor = "";

        MailAddress receptor = new(correo_receptor);
        MailAddress emisor = new(correo_emisor);

        MailMessage email = new(emisor, receptor);
        email.Subject = "InnoMarkets: Activacion de cuenta";
        email.Body = @"<!DOCTYPE html>
                      <html>
                      <head>
                        <title>Activacion de cuenta</title>
                        </head>
                        <body>
                            <h2>Activacionde cuenta</h2>
                            <p>Para activar su cuenta, haga clic en el siguiente enlace:</p>
                            <a href= 'http://localhost:5231/Cuenta/Token?valor=" + token + ">Activar cuenta</a></body></html>";
        email.IsBodyHtml = true;

        SmtpClient smtp = new();
        smtp.Host = "smtp.office365.com";
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential(correo_emisor, Clave_emisor);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.EnableSsl = true;

        try
        {
            smtp.Send(email);
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
}