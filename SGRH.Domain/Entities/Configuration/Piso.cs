using SGRH.Domain.Base;
using SGRH.Domain.Exceptions;

namespace SGRH.Domain.Entities.Configuration
{
    /// <summary>Piso de un hotel. Agrupa habitaciones (Piso 1 -> N Habitaciones).</summary>
    public sealed class Piso : AuditEntity
    {
        public int Numero { get; set; }

        // RN-002: un piso pertenece a un solo hotel.
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public List<Habitacion> Habitaciones { get; set; } = new();

        /// <summary>RN-004, RN-005 y RN-006.</summary>
        public void AgregarHabitacion(Habitacion habitacion)
        {
            // RN-006: un piso inactivo no recibe habitaciones nuevas.
            if (!Estatus)
                throw new ReglaNegocioException($"El piso {Numero} está inactivo y no puede recibir habitaciones (RN-006).");

            // RN-005: si el hotel está cargado, el número no se puede repetir en todo el hotel.
            bool repetida = Hotel != null
                ? Hotel.ExisteNumeroHabitacion(habitacion.Numero)
                : Habitaciones.Any(h => h.Numero.Equals(habitacion.Numero, StringComparison.OrdinalIgnoreCase));

            if (repetida)
                throw new ReglaNegocioException($"Ya existe una habitación con el número {habitacion.Numero} en este hotel (RN-005).");

            // RN-004: la habitación queda en este piso y, por extensión, en este hotel.
            habitacion.PisoId = Id;
            habitacion.HotelId = HotelId;
            habitacion.Piso = this;
            Habitaciones.Add(habitacion);
            MarcarModificacion();
        }
    }
}
