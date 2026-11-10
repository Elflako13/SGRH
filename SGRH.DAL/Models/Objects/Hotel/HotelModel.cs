namespace SGRH.DAL.Models.Objects.Hotel
{
    public class HotelModel
    {
        public int HotelId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public int Estrellas { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public int? CadenaHoteleraId { get; set; }
        public int CantidadBloques { get; set; }
        public int CantidadPisos { get; set; }
        public int CantidadHabitaciones { get; set; }
        public bool Estatus { get; set; }
    }
}
