using SGRH.Domain.Base;
using SGRH.Domain.Exceptions;

namespace SGRH.Domain.Entities.Configuration
{
    /// <summary>Hotel de la cadena. Controla sus pisos (Hotel 1 -> N Pisos).</summary>
    public sealed class Hotel : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public int? CadenaHoteleraId { get; set; }
        public CadenaHotelera? CadenaHotelera { get; set; }

        public List<Piso> Pisos { get; set; } = new();

        /// <summary>RN-001 y RN-003: agrega un piso validando que no exista otro activo con el mismo número.</summary>
        public Piso AgregarPiso(int numero)
        {
            if (numero <= 0)
                throw new ReglaNegocioException("El número de piso debe ser mayor que cero.");

            if (Pisos.Any(p => p.Estatus && p.Numero == numero))
                throw new ReglaNegocioException($"Ya existe un piso activo con el número {numero} en este hotel (RN-003).");

            var piso = new Piso { Numero = numero, HotelId = Id, Hotel = this };
            Pisos.Add(piso);
            MarcarModificacion();
            return piso;
        }

        /// <summary>RN-005: el número de habitación es único dentro de todo el hotel.</summary>
        public bool ExisteNumeroHabitacion(string numero)
        {
            return Pisos.SelectMany(p => p.Habitaciones)
                        .Any(h => h.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));
        }

        public Habitacion? BuscarHabitacion(string numero)
        {
            return Pisos.SelectMany(p => p.Habitaciones)
                        .FirstOrDefault(h => h.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));
        }
    }
}
