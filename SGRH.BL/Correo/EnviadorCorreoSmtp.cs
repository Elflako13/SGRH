using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SGRH.BL.Configuration;

namespace SGRH.BL.Correo
{
    public class EnviadorCorreoSmtp : IEnviadorCorreo
    {
        private readonly CorreoOptions opciones;
        private readonly ILogger<EnviadorCorreoSmtp> logger;

        public EnviadorCorreoSmtp(IOptions<CorreoOptions> opciones, ILogger<EnviadorCorreoSmtp> logger)
        {
            this.opciones = opciones?.Value ?? throw new ArgumentNullException(nameof(opciones));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Habilitado => opciones.Habilitado
                                  && !string.IsNullOrWhiteSpace(opciones.Servidor)
                                  && !string.IsNullOrWhiteSpace(opciones.Usuario)
                                  && !string.IsNullOrWhiteSpace(opciones.Clave);

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            if (!Habilitado)
                throw new InvalidOperationException("El envío de correos no está configurado.");

            if (string.IsNullOrWhiteSpace(destinatario))
                throw new ArgumentException("Debe indicar el correo del destinatario.", nameof(destinatario));

            string usuario = opciones.Usuario!.Trim();
            string remitente = string.IsNullOrWhiteSpace(opciones.CorreoRemitente) ? usuario : opciones.CorreoRemitente.Trim();

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(opciones.NombreRemitente, remitente));
            mensaje.To.Add(MailboxAddress.Parse(destinatario.Trim()));
            mensaje.Subject = asunto;
            mensaje.Body = new BodyBuilder { HtmlBody = cuerpoHtml }.ToMessageBody();

            var seguridad = !opciones.UsarSsl ? SecureSocketOptions.None
                          : opciones.Puerto == 465 ? SecureSocketOptions.SslOnConnect
                          : SecureSocketOptions.StartTls;

            using var cliente = new SmtpClient { Timeout = 20000 };
            await cliente.ConnectAsync(opciones.Servidor, opciones.Puerto, seguridad);
            await cliente.AuthenticateAsync(usuario, opciones.Clave!.Replace(" ", string.Empty));
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);

            logger.LogInformation("Correo enviado a {Destinatario}: {Asunto}", destinatario, asunto);
        }
    }
}
