namespace SGRH.DAL.Models.Objects.Notificacion
{
    public class NotificacionModel
    {
        public int NotificacionId { get; set; }
        public int ReservaId { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public CanalNotificacion Canal { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public EstadoNotificacion Estado { get; set; }
        public int Intentos { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? Error { get; set; }
    }
}
