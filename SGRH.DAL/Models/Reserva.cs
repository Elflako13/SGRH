using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
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

        public Reserva(Huesped huesped, Habitacion habitacion, RangoFechas fechas, int cantidadHuespedes)
        {
            // RN-017: toda reserva tiene un huésped registrado.
            Huesped = huesped ?? throw new ReglaNegocioException("La reserva debe tener un huésped registrado (RN-017).");
            HuespedId = huesped.Id;
            Codigo = GenerarCodigo();
            Estado = EstadoReserva.Pendiente;
            AsignarDatos(habitacion, fechas, cantidadHuespedes);
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
        public void Modificar(Habitacion habitacion, RangoFechas fechas, int cantidadHuespedes)
        {
            VerificarQueEsteActiva("modificar");
            AsignarDatos(habitacion, fechas, cantidadHuespedes);
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

        /// <summary>RN-013: precio total = noches x tarifa por noche de la habitación.</summary>
        public decimal CalcularPrecio(Habitacion habitacion)
        {
            return Fechas.Noches * habitacion.TarifaPorNoche;
        }

        private void AsignarDatos(Habitacion habitacion, RangoFechas fechas, int cantidadHuespedes)
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
            PrecioTotal = CalcularPrecio(habitacion);
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
