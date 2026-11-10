using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    /// <summary>
    /// Tipo de habitación que maneja la cadena (Sencilla, Doble, Suite...).
    /// Antes era una enumeración fija; ahora es un catálogo que el administrador mantiene.
    /// </summary>
    public sealed class TipoHabitacion : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        /// <summary>Tarifa sugerida al registrar una habitación de este tipo.</summary>
        public decimal TarifaBase { get; set; }

        public List<Habitacion> Habitaciones { get; set; } = new();
    }
}
