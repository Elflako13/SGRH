using SGRH.Domain.Base;
using SGRH.Domain.Enums;
using SGRH.Domain.Exceptions;

namespace SGRH.Domain.Entities.Configuration
{
    /// <summary>Habitación de un piso, con su capacidad, tarifa y estado operativo.</summary>
    public sealed class Habitacion : AuditEntity
    {
        // Constructor vacío requerido por Entity Framework Core.
        private Habitacion()
        {
        }

        public Habitacion(string numero, TipoHabitacion tipo, int capacidad, decimal tarifaPorNoche)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ReglaNegocioException("El número de habitación es obligatorio.");

            Numero = numero.Trim();
            Tipo = tipo;
            CambiarCapacidad(capacidad);
            ActualizarTarifa(tarifaPorNoche);
            Estado = EstadoHabitacion.Disponible;
        }

        public string Numero { get; private set; } = string.Empty;
        public TipoHabitacion Tipo { get; set; }
        public int Capacidad { get; private set; }
        public decimal TarifaPorNoche { get; private set; }
        public EstadoHabitacion Estado { get; private set; }

        public int PisoId { get; set; }
        public Piso? Piso { get; set; }

        // Se guarda también el hotel para poder crear el índice único (HotelId, Numero) - RN-005.
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        /// <summary>RN-007: solo se reservan habitaciones activas que no estén en mantenimiento ni fuera de servicio.</summary>
        public bool EsReservable()
        {
            return Estatus
                && (Estado == EstadoHabitacion.Disponible || Estado == EstadoHabitacion.Limpieza);
        }

        /// <summary>RN-011: la cantidad de huéspedes debe estar entre 1 y la capacidad.</summary>
        public bool PuedeAlojar(int cantidadHuespedes)
        {
            return cantidadHuespedes >= 1 && cantidadHuespedes <= Capacidad;
        }

        /// <summary>
        /// Cambia el estado operativo. La verificación de reservas futuras (RN-018)
        /// la hace la capa de aplicación antes de llamar a este método.
        /// </summary>
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
