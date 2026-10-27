namespace SGRH.DAL.Models.Objects.Notificacion
{
    public class NotificacionBase
    {
        public int ReservaId { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public CanalNotificacion Canal { get; set; }
        public string? Mensaje { get; set; }
    }
}
