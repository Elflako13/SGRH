using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;

namespace SGRH.BL.Tests.Base
{
    /// <summary>Opciones de negocio para las pruebas (los mismos valores de appsettings.json).</summary>
    public static class Opciones
    {
        public static IOptions<ReservaOptions> Reserva(int maximoNoches = 30, int diasAnticipacion = 365)
            => Options.Create(new ReservaOptions { MaximoNoches = maximoNoches, DiasMaximoAnticipacion = diasAnticipacion });

        public static IOptions<NotificacionOptions> Notificacion()
            => Options.Create(new NotificacionOptions { Remitente = "Hotel Prueba" });
    }
}
