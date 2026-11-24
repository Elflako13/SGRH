namespace SGRH.BL.Correo
{
    public interface IEnviadorCorreo
    {
        bool Habilitado { get; }

        Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml);
    }
}
