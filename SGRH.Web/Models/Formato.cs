using SGRH.DAL.Models;

namespace SGRH.Web.Models
{
    /// <summary>Textos y estilos que se repiten en varias vistas (DRY).</summary>
    public static class Formato
    {
        public static string Estado(EstadoHabitacion estado) => estado switch
        {
            EstadoHabitacion.Disponible => "Disponible",
            EstadoHabitacion.Limpieza => "Limpieza",
            EstadoHabitacion.Mantenimiento => "Mantenimiento",
            EstadoHabitacion.FueraDeServicio => "Fuera de servicio",
            EstadoHabitacion.Inactiva => "Inactiva",
            _ => estado.ToString()
        };

        public static string ClaseEstado(EstadoHabitacion estado) => estado switch
        {
            EstadoHabitacion.Disponible => "ok",
            EstadoHabitacion.Limpieza => "info",
            EstadoHabitacion.Mantenimiento => "warn",
            EstadoHabitacion.FueraDeServicio => "danger",
            _ => "muted"
        };

        public static string Moneda(decimal valor) => $"RD$ {valor:N2}";
    }
}
