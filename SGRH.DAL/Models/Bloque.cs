using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class Bloque : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public List<Piso> Pisos { get; set; } = new();
    }
}
