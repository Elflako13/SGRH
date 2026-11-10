namespace SGRH.DAL.Models.Objects.Hotel
{
    public class HotelBase
    {
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public int Estrellas { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public int? CadenaHoteleraId { get; set; }
    }
}
