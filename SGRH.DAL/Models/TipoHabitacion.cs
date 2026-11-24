using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class TipoHabitacion : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public decimal TarifaBase { get; set; }

        public List<Habitacion> Habitaciones { get; set; } = new();
    }
}
