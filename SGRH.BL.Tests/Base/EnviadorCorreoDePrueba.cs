using SGRH.BL.Correo;

namespace SGRH.BL.Tests.Base
{
    public class EnviadorCorreoDePrueba : IEnviadorCorreo
    {
        private readonly bool fallar;

        public EnviadorCorreoDePrueba(bool habilitado = true, bool fallar = false)
        {
            Habilitado = habilitado;
            this.fallar = fallar;
        }

        public bool Habilitado { get; }

        public List<(string Destinatario, string Asunto, string Cuerpo)> Enviados { get; } = new();

        public Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            if (fallar)
                throw new InvalidOperationException("Servidor de correo no disponible.");

            Enviados.Add((destinatario, asunto, cuerpoHtml));
            return Task.CompletedTask;
        }
    }
}
