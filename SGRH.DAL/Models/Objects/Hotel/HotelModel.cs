namespace SGRH.DAL.Models.Objects.Hotel
{
    public class HotelModel
    {
        public int HotelId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public int? CadenaHoteleraId { get; set; }
    }
}
