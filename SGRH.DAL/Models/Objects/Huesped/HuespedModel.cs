namespace SGRH.DAL.Models.Objects.Huesped
{
    public class HuespedModel
    {
        public int HuespedId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
        public string Documento { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }
}
