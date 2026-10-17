using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Enums;
using SGRH.Domain.Exceptions;
using SGRH.Domain.Services;
using SGRH.Domain.ValueObjects;

namespace SGRH.Domain.Entities.Reservation
{
    /// <summary>
    /// Estadía de un huésped en una habitación. Controla su ciclo de vida:
    /// Pendiente -> Confirmada -> Finalizada, o Cancelada.
    /// </summary>
    public sealed class Reserva : AuditEntity
    {
        // Constructor vacío requerido por Entity Framework Core.
        private Reserva()
        {
        }

        public Reserva(Huesped huesped, Habitacion habitacion, RangoFechas fechas,
                       int cantidadHuespedes, ICalculadoraPrecio calculadora)
        {
            // RN-017: toda reserva tiene un huésped registrado.
            Huesped = huesped ?? throw new ReglaNegocioException("La reserva debe tener un huésped registrado (RN-017).");
            HuespedId = huesped.Id;
            Codigo = GenerarCodigo();
            Estado = EstadoReserva.Pendiente;
            AsignarDatos(habitacion, fechas, cantidadHuespedes, calculadora);
        }

        public string Codigo { get; private set; } = string.Empty;

        public int HuespedId { get; private set; }
        public Huesped? Huesped { get; private set; }

        public int HabitacionId { get; private set; }
        public Habitacion? Habitacion { get; private set; }

        public RangoFechas Fechas { get; private set; } = null!;
        public int CantidadHuespedes { get; private set; }
        public decimal PrecioTotal { get; private set; }
        public EstadoReserva Estado { get; private set; }
        public string? MotivoCancelacion { get; private set; }

        /// <summary>Control de concurrencia: evita que dos usuarios guarden la misma reserva a la vez.</summary>
        public byte[]? RowVersion { get; set; }

        public List<HistorialReserva> Historial { get; set; } = new();

        /// <summary>RN-012: solo las reservas Pendiente o Confirmada bloquean la habitación.</summary>
        public bool EstaActiva()
        {
            return Estado == EstadoReserva.Pendiente || Estado == EstadoReserva.Confirmada;
        }

        /// <summary>RN-010 y RN-012: una reserva cancelada o finalizada nunca se solapa.</summary>
        public bool SeSolapaCon(RangoFechas otrasFechas)
        {
            return EstaActiva() && Fechas.SeSolapaCon(otrasFechas);
        }

        public void Confirmar()
        {
            if (Estado != EstadoReserva.Pendiente)
                throw new ReglaNegocioException("Solo se pueden confirmar reservas pendientes (RN-016).");

            Estado = EstadoReserva.Confirmada;
            MarcarModificacion();
        }

        /// <summary>RN-016: solo se modifican reservas activas, volviendo a validar todo.</summary>
        public void Modificar(Habitacion habitacion, RangoFechas fechas, int cantidadHuespedes, ICalculadoraPrecio calculadora)
        {
            VerificarQueEsteActiva("modificar");
            AsignarDatos(habitacion, fechas, cantidadHuespedes, calculadora);
            MarcarModificacion();
        }

        /// <summary>RN-012 y RN-016: cancela la reserva y libera la habitación.</summary>
        public void Cancelar(string motivo)
        {
            VerificarQueEsteActiva("cancelar");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ReglaNegocioException("Debe indicar el motivo de la cancelación.");

            Estado = EstadoReserva.Cancelada;
            MotivoCancelacion = motivo.Trim();
            MarcarModificacion();
        }

        public void Finalizar()
        {
            if (Estado != EstadoReserva.Confirmada)
                throw new ReglaNegocioException("Solo se pueden finalizar reservas confirmadas.");

            Estado = EstadoReserva.Finalizada;
            MarcarModificacion();
        }

        /// <summary>RN-013: el precio lo calcula la estrategia recibida (por defecto noches x tarifa).</summary>
        public void AplicarPrecio(ICalculadoraPrecio calculadora, Habitacion habitacion)
        {
            PrecioTotal = calculadora.CalcularTotal(habitacion, Fechas);
        }

        private void AsignarDatos(Habitacion habitacion, RangoFechas fechas, int cantidadHuespedes, ICalculadoraPrecio calculadora)
        {
            if (habitacion == null)
                throw new ReglaNegocioException("Debe seleccionar una habitación.");

            // RN-007
            if (!habitacion.EsReservable())
                throw new ReglaNegocioException($"La habitación {habitacion.Numero} no está disponible para reservar (RN-007).");

            // RN-011
            if (!habitacion.PuedeAlojar(cantidadHuespedes))
                throw new ReglaNegocioException($"La habitación {habitacion.Numero} admite máximo {habitacion.Capacidad} huésped(es) (RN-011).");

            Habitacion = habitacion;
            HabitacionId = habitacion.Id;
            Fechas = fechas ?? throw new ReglaNegocioException("Debe indicar las fechas de la reserva.");
            CantidadHuespedes = cantidadHuespedes;
            AplicarPrecio(calculadora, habitacion);
        }

        private void VerificarQueEsteActiva(string accion)
        {
            if (!EstaActiva())
                throw new ReglaNegocioException($"No se puede {accion} una reserva en estado {Estado} (RN-016).");
        }

        private static string GenerarCodigo()
        {
            return "RES-" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        }
    }
}
