using SGRH.DAL.Models;

namespace SGRH.BL.Configuration
{
    public class NotificacionOptions
    {
        public const string Seccion = "NotificacionOptions";

        public CanalNotificacion CanalPorDefecto { get; set; } = CanalNotificacion.Correo;

        public string Remitente { get; set; } = "SGRH - Reservas";
    }
}
