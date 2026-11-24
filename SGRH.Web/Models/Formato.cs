using SGRH.DAL.Models;
using System.Text.RegularExpressions;

namespace SGRH.Web.Models
{
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

        public static string Iniciales(string nombreHotel)
        {
            var palabras = nombreHotel.Replace("Hotel ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(palabras.Take(2).Select(p => char.ToUpper(p[0])));
        }

        public static string Limpiar(string mensaje) =>
            Regex.Replace(mensaje, @"\s*\((?:CU|RN|RNF|RF)-\d{3}[^)]*\)", string.Empty);
    }
}
