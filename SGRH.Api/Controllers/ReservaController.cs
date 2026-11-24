using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Reserva;

namespace SGRH.Api.Controllers
{
    public class ReservaController : BaseApiController
    {
        private readonly IReservaService reservaService;

        public ReservaController(IReservaService reservaService)
        {
            this.reservaService = reservaService;
        }

        [HttpGet("GetReservas")]
        public async Task<IActionResult> Get() => Respuesta(await reservaService.GetAllAsync());

        [HttpGet("GetReserva")]
        public async Task<IActionResult> GetById(int reservaId) => Respuesta(await reservaService.GetByIdAsync(reservaId));

        [HttpGet("GetReservasByHuesped")]
        public async Task<IActionResult> GetByHuesped(int huespedId) => Respuesta(await reservaService.GetByHuespedAsync(huespedId));

        [HttpGet("GetHistorial")]
        public async Task<IActionResult> GetHistorial(int reservaId) => Respuesta(await reservaService.GetHistorialAsync(reservaId));

        [HttpPost("SaveReserva")]
        public async Task<IActionResult> Save([FromBody] ReservaAdd model) => RespuestaOperacion(await reservaService.AddAsync(model));

        [HttpPut("UpdateReserva")]
        public async Task<IActionResult> Update([FromBody] ReservaUpdate model) => RespuestaOperacion(await reservaService.UpdateAsync(model));

        [HttpPut("ConfirmarReserva")]
        public async Task<IActionResult> Confirmar(int reservaId, int? usuarioModificacion) =>
            RespuestaOperacion(await reservaService.ConfirmarAsync(reservaId, usuarioModificacion));

        [HttpPut("CancelarReserva")]
        public async Task<IActionResult> Cancelar([FromBody] ReservaRemove model) => RespuestaOperacion(await reservaService.RemoveAsync(model));
    }
}
