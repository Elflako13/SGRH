namespace SGRH.DAL.Models.Objects.Bloque
{
    public class BloqueModel
    {
        public int BloqueId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int HotelId { get; set; }
        public string Hotel { get; set; } = string.Empty;
        public int CantidadPisos { get; set; }
        public bool Estatus { get; set; }
    }
}
