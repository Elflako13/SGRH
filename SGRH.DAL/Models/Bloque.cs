using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    /// <summary>
    /// Bloque (edificio, torre o ala) de un hotel. Agrupa pisos para ubicarlos mejor
    /// (por ejemplo: Torre A con los pisos 1 al 5). Un bloque pertenece a un solo hotel.
    /// </summary>
    public sealed class Bloque : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public List<Piso> Pisos { get; set; } = new();
    }
}
