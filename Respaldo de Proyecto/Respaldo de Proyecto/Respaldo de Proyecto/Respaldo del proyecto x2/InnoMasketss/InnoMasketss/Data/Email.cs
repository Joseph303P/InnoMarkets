using System;
using System.Net;
using System.Net.Mail;

public class Email
{
    // Se utiliza para enviar un correo electrónico al usuario
    public void Enviar(string correo, string token)
    {
        Correo(correo, token);
    }

    // Configura el correo electrónico y lo envía usando el protocolo SMTP
    void Correo(string correo_receptor, string token)
    {
        string correo_emisor = "josephperez404@hotmail.com"; // Reemplaza con tu correo Hotmail
        string clave_emisor = "panita404"; // Reemplaza con tu contraseña

        MailAddress receptor = new(correo_receptor);
        MailAddress emisor = new(correo_emisor);

        MailMessage email = new(emisor, receptor);

        email.Subject = "InnoMasketss: Activación de cuenta";
        email.Body = @"<!DOCTYPE html>
                           <html>
                           <head>
                               <title>Activación de cuenta</title>
                           </head>
                           <body>
                               <h2>Activación de cuenta</h2>
                               <p>Para activar su cuenta, haga clic en el siguiente enlace:</p>
                               <a href='http://localhost:7131/Cuenta/Token?valor=" + token + "'>Activar cuenta</a></body></html>";

        email.IsBodyHtml = true;

        SmtpClient smtp = new();
        smtp.Host = "smtp.office365.com"; // SMTP de Hotmail
        smtp.Port = 587; // Usamos el puerto correcto para TLS
        smtp.EnableSsl = true; 
        smtp.Credentials = new NetworkCredential(correo_emisor, clave_emisor);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.Timeout = 20000; // Tiempo de espera

        try
        {
            smtp.Send(email);
        }
        catch (SmtpException)
        {
            throw;
        }
    }
}
