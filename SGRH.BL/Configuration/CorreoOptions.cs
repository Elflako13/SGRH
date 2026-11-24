namespace SGRH.BL.Configuration
{
    public class CorreoOptions
    {
        public const string Seccion = "CorreoOptions";

        public bool Habilitado { get; set; }

        public string Servidor { get; set; } = "smtp.gmail.com";

        public int Puerto { get; set; } = 587;

        public bool UsarSsl { get; set; } = true;

        public string? Usuario { get; set; }

        public string? Clave { get; set; }

        public string? CorreoRemitente { get; set; }

        public string NombreRemitente { get; set; } = "Caribe Hoteles";
    }
}
