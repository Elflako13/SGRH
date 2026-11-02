using SGRH.DAL.Models;

namespace SGRH.BL.Configuration
{
    /// <summary>Parámetros de las notificaciones al huésped (sección "NotificacionOptions").</summary>
    public class NotificacionOptions
    {
        public const string Seccion = "NotificacionOptions";

        /// <summary>Canal que se usa al generar una notificación.</summary>
        public CanalNotificacion CanalPorDefecto { get; set; } = CanalNotificacion.Correo;

        /// <summary>Nombre del hotel o cadena que firma los mensajes.</summary>
        public string Remitente { get; set; } = "SGRH - Reservas";
    }
}
