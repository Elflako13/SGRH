using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class Habitacion : AuditEntity
    {
        private Habitacion()
        {
        }

        public Habitacion(string numero, int tipoHabitacionId, int capacidad, decimal tarifaPorNoche)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ReglaNegocioException("El número de habitación es obligatorio.");

            Numero = numero.Trim();
            TipoHabitacionId = tipoHabitacionId;
            CambiarCapacidad(capacidad);
            ActualizarTarifa(tarifaPorNoche);
            Estado = EstadoHabitacion.Disponible;
        }

        public Habitacion(string numero, TipoHabitacion tipo, int capacidad, decimal tarifaPorNoche)
            : this(numero, tipo.Id, capacidad, tarifaPorNoche)
        {
            TipoHabitacion = tipo;
        }

        public string Numero { get; private set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public TipoHabitacion? TipoHabitacion { get; set; }
        public int Capacidad { get; private set; }
        public decimal TarifaPorNoche { get; private set; }
        public EstadoHabitacion Estado { get; private set; }

        public int PisoId { get; set; }
        public Piso? Piso { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public bool EsReservable()
        {
            return Estatus
                && (Estado == EstadoHabitacion.Disponible || Estado == EstadoHabitacion.Limpieza);
        }

        public bool PuedeAlojar(int cantidadHuespedes)
        {
            return cantidadHuespedes >= 1 && cantidadHuespedes <= Capacidad;
        }

        public void CambiarEstado(EstadoHabitacion nuevoEstado)
        {
            if (nuevoEstado == Estado)
                throw new ReglaNegocioException($"La habitación {Numero} ya está en estado {Estado}.");

            Estado = nuevoEstado;
            Estatus = nuevoEstado != EstadoHabitacion.Inactiva;
            MarcarModificacion();
        }

        public void ActualizarTarifa(decimal tarifaPorNoche)
        {
            if (tarifaPorNoche <= 0)
                throw new ReglaNegocioException("La tarifa por noche debe ser mayor que cero.");

            TarifaPorNoche = tarifaPorNoche;
            MarcarModificacion();
        }

        public void CambiarCapacidad(int capacidad)
        {
            if (capacidad <= 0)
                throw new ReglaNegocioException("La capacidad debe ser mayor que cero.");

            Capacidad = capacidad;
            MarcarModificacion();
        }
    }
}
