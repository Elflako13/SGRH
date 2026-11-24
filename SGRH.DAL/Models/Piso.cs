using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class Piso : AuditEntity
    {
        public int Numero { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public int? BloqueId { get; set; }
        public Bloque? Bloque { get; set; }

        public List<Habitacion> Habitaciones { get; set; } = new();

        public void AgregarHabitacion(Habitacion habitacion)
        {
            if (!Estatus)
                throw new ReglaNegocioException($"El piso {Numero} está inactivo y no puede recibir habitaciones (RN-006).");

            bool repetida = Hotel != null
                ? Hotel.ExisteNumeroHabitacion(habitacion.Numero)
                : Habitaciones.Any(h => h.Numero.Equals(habitacion.Numero, StringComparison.OrdinalIgnoreCase));

            if (repetida)
                throw new ReglaNegocioException($"Ya existe una habitación con el número {habitacion.Numero} en este hotel (RN-005).");

            habitacion.PisoId = Id;
            habitacion.HotelId = HotelId;
            habitacion.Hotel = Hotel;
            habitacion.Piso = this;
            Habitaciones.Add(habitacion);
            MarcarModificacion();
        }
    }
}
