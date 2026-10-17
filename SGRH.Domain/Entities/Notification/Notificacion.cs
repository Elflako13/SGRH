using SGRH.Domain.Base;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Domain.Exceptions;

namespace SGRH.Domain.Entities.Notification
{
    /// <summary>
    /// Aviso al huésped sobre su reserva (RN-014). Tiene su propio estado, por eso un fallo
    /// de envío no afecta a la reserva (RN-015).
    /// </summary>
    public sealed class Notificacion : AuditEntity
    {
        public const int MaximoIntentos = 3;

        // Constructor vacío requerido por Entity Framework Core.
        private Notificacion()
        {
        }

        public Notificacion(int reservaId, TipoNotificacion tipo, CanalNotificacion canal, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ReglaNegocioException("La notificación debe tener un mensaje.");

            ReservaId = reservaId;
            Tipo = tipo;
            Canal = canal;
            Mensaje = mensaje;
            Estado = EstadoNotificacion.Pendiente;
        }

        public int ReservaId { get; private set; }
        public Reserva? Reserva { get; private set; }
        public TipoNotificacion Tipo { get; private set; }
        public CanalNotificacion Canal { get; private set; }
        public string Mensaje { get; private set; } = string.Empty;
        public EstadoNotificacion Estado { get; private set; }
        public int Intentos { get; private set; }
        public DateTime? FechaEnvio { get; private set; }
        public string? Error { get; private set; }

        public void MarcarEnviada()
        {
            Intentos++;
            Estado = EstadoNotificacion.Enviada;
            FechaEnvio = DateTime.Now;
            Error = null;
            MarcarModificacion();
        }

        /// <summary>RN-015: se registra el fallo, pero la reserva no se toca.</summary>
        public void MarcarFallida(string error)
        {
            Intentos++;
            Estado = EstadoNotificacion.Fallida;
            Error = error;
            MarcarModificacion();
        }

        /// <summary>CU-016: solo se reintentan las fallidas que no pasaron el máximo de intentos.</summary>
        public bool PuedeReintentar()
        {
            return Estado == EstadoNotificacion.Fallida && Intentos < MaximoIntentos;
        }
    }
}
