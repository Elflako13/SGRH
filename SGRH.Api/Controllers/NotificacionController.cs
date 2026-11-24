using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.BL.Correo;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Notificacion;

namespace SGRH.Api.Controllers
{
    public class NotificacionController : BaseApiController
    {
        private readonly INotificacionService notificacionService;
        private readonly IEnviadorCorreo enviadorCorreo;
        private readonly string remitente;

        public NotificacionController(INotificacionService notificacionService, IEnviadorCorreo enviadorCorreo, IOptions<NotificacionOptions> notificacionOptions)
        {
            this.notificacionService = notificacionService;
            this.enviadorCorreo = enviadorCorreo;
            remitente = notificacionOptions.Value.Remitente;
        }

        [HttpGet("GetNotificacionesByReserva")]
        public async Task<IActionResult> GetByReserva(int reservaId) => Respuesta(await notificacionService.GetByReservaAsync(reservaId));

        [HttpGet("GetPendientesDeReintento")]
        public async Task<IActionResult> GetPendientes() => Respuesta(await notificacionService.GetPendientesDeReintentoAsync());

        [HttpPost("SaveNotificacion")]
        public async Task<IActionResult> Save([FromBody] NotificacionAdd model) => RespuestaOperacion(await notificacionService.AddAsync(model));

        [HttpPut("RegistrarEnvio")]
        public async Task<IActionResult> RegistrarEnvio(int notificacionId, bool enviada, string? error) =>
            RespuestaOperacion(await notificacionService.RegistrarResultadoEnvioAsync(notificacionId, enviada, error));

        [HttpPost("ReintentarFallidas")]
        public async Task<IActionResult> ReintentarFallidas() =>
            RespuestaOperacion(await notificacionService.ReintentarFallidasAsync(async notificacion =>
            {
                await enviadorCorreo.EnviarAsync(notificacion.Destinatario ?? string.Empty,
                                                 $"Notificación de su reserva · {remitente}",
                                                 PlantillaCorreo.Texto("Notificación de su reserva", notificacion.Mensaje, remitente));
                return true;
            }));
    }
}
