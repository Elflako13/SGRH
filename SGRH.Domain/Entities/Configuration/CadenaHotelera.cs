using SGRH.Domain.Base;

namespace SGRH.Domain.Entities.Configuration
{
    /// <summary>Organización dueña de uno o varios hoteles.</summary>
    public sealed class CadenaHotelera : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? RNC { get; set; }

        public List<Hotel> Hoteles { get; set; } = new();

        public void AgregarHotel(Hotel hotel)
        {
            hotel.CadenaHoteleraId = Id;
            hotel.CadenaHotelera = this;
            Hoteles.Add(hotel);
            MarcarModificacion();
        }
    }
}
