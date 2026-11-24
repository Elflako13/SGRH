using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Reserva;
using System.Net;

namespace SGRH.BL.Correo
{
    public static class PlantillaCorreo
    {
        public static string Asunto(ReservaModel reserva, TipoNotificacion tipo, string remitente) =>
            $"{Titulo(tipo)} · {reserva.Codigo} · {remitente}";

        public static string Titulo(TipoNotificacion tipo) => tipo switch
        {
            TipoNotificacion.Creacion => "Reserva recibida",
            TipoNotificacion.Confirmacion => "Reserva confirmada",
            TipoNotificacion.Modificacion => "Reserva modificada",
            TipoNotificacion.Cancelacion => "Reserva cancelada",
            _ => "Actualización de su reserva"
        };

        public static string Reserva(ReservaModel reserva, TipoNotificacion tipo, string remitente)
        {
            string introduccion = tipo switch
            {
                TipoNotificacion.Creacion => "Recibimos su reserva. Estos son los detalles:",
                TipoNotificacion.Confirmacion => "Su reserva fue confirmada. Le esperamos en las fechas indicadas.",
                TipoNotificacion.Modificacion => "Su reserva fue modificada. Estos son los nuevos detalles:",
                TipoNotificacion.Cancelacion => "Su reserva fue cancelada.",
                _ => "Su reserva tuvo un cambio."
            };

            var filas = new List<(string, string)>
            {
                ("Código", reserva.Codigo),
                ("Hotel", reserva.Hotel),
                ("Habitación", $"{reserva.Habitacion} · {reserva.TipoHabitacion}"),
                ("Entrada", reserva.FechaEntrada.ToString("dd/MM/yyyy")),
                ("Salida", reserva.FechaSalida.ToString("dd/MM/yyyy")),
                ("Noches", reserva.Noches.ToString()),
                ("Huéspedes", reserva.CantidadHuespedes.ToString()),
                ("Total", $"RD$ {reserva.PrecioTotal:N2}"),
                ("Estado", reserva.Estado.ToString())
            };

            if (tipo == TipoNotificacion.Cancelacion && !string.IsNullOrWhiteSpace(reserva.MotivoCancelacion))
                filas.Add(("Motivo", reserva.MotivoCancelacion));

            string tabla = string.Concat(filas.Select(f =>
                $"<tr><td style=\"padding:8px 12px;color:#64748b;border-bottom:1px solid #e2e8f0\">{Html(f.Item1)}</td>" +
                $"<td style=\"padding:8px 12px;font-weight:600;color:#1e293b;border-bottom:1px solid #e2e8f0\">{Html(f.Item2)}</td></tr>"));

            return Envolver(Titulo(tipo), $"Hola {Html(reserva.Huesped)},", introduccion, tabla, remitente);
        }

        public static string Texto(string titulo, string mensaje, string remitente) =>
            Envolver(titulo, string.Empty, mensaje, string.Empty, remitente);

        private static string Envolver(string titulo, string saludo, string introduccion, string tabla, string remitente) =>
            "<div style=\"background:#f1f5f9;padding:24px;font-family:Segoe UI,Arial,sans-serif\">" +
            "<div style=\"max-width:560px;margin:0 auto;background:#ffffff;border-radius:10px;overflow:hidden\">" +
            $"<div style=\"background:#1f3864;color:#ffffff;padding:20px 24px;font-size:20px\">{Html(remitente)}" +
            "<div style=\"height:3px;width:48px;background:#c9a227;margin-top:10px\"></div></div>" +
            "<div style=\"padding:24px\">" +
            $"<h2 style=\"margin:0 0 12px;color:#1f3864\">{Html(titulo)}</h2>" +
            (string.IsNullOrEmpty(saludo) ? string.Empty : $"<p style=\"margin:0 0 8px;color:#1e293b\">{saludo}</p>") +
            $"<p style=\"margin:0 0 16px;color:#1e293b\">{Html(introduccion)}</p>" +
            (string.IsNullOrEmpty(tabla) ? string.Empty : $"<table style=\"width:100%;border-collapse:collapse;font-size:14px\">{tabla}</table>") +
            "<p style=\"margin:20px 0 0;color:#64748b;font-size:13px\">Este es un mensaje automático, por favor no lo responda.</p>" +
            "</div></div></div>";

        private static string Html(string? texto) => WebUtility.HtmlEncode(texto ?? string.Empty);
    }
}
