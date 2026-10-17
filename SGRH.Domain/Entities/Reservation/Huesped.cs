using SGRH.Domain.Base;
using SGRH.Domain.Exceptions;

namespace SGRH.Domain.Entities.Reservation
{
    /// <summary>Cliente que reserva. El documento es único (RN-017).</summary>
    public sealed class Huesped : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }

        public List<Reserva> Reservas { get; set; } = new();

        public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

        public void ActualizarContacto(string? correo, string? telefono)
        {
            if (string.IsNullOrWhiteSpace(correo) && string.IsNullOrWhiteSpace(telefono))
                throw new ReglaNegocioException("El huésped debe tener al menos un correo o un teléfono.");

            Correo = correo?.Trim();
            Telefono = telefono?.Trim();
            MarcarModificacion();
        }

        /// <summary>Se usa antes de notificar (CU-015, flujo alterno 1a).</summary>
        public bool TieneContactoValido()
        {
            bool correoValido = !string.IsNullOrWhiteSpace(Correo) && Correo.Contains('@');
            bool telefonoValido = !string.IsNullOrWhiteSpace(Telefono);
            return correoValido || telefonoValido;
        }
    }
}
