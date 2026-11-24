using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Piso;

namespace SGRH.Api.Controllers
{
    public class PisoController : BaseApiController
    {
        private readonly IPisoService pisoService;

        public PisoController(IPisoService pisoService)
        {
            this.pisoService = pisoService;
        }

        [HttpGet("GetPisos")]
        public async Task<IActionResult> Get() => Respuesta(await pisoService.GetAllAsync());

        [HttpGet("GetPiso")]
        public async Task<IActionResult> GetById(int pisoId) => Respuesta(await pisoService.GetByIdAsync(pisoId));

        [HttpGet("GetPisosByHotel")]
        public async Task<IActionResult> GetByHotel(int hotelId) => Respuesta(await pisoService.GetByHotelAsync(hotelId));

        [HttpPost("SavePiso")]
        public async Task<IActionResult> Save([FromBody] PisoAdd model) => RespuestaOperacion(await pisoService.AddAsync(model));

        [HttpPut("UpdatePiso")]
        public async Task<IActionResult> Update([FromBody] PisoUpdate model) => RespuestaOperacion(await pisoService.UpdateAsync(model));

        [HttpPut("RemovePiso")]
        public async Task<IActionResult> Remove([FromBody] PisoRemove model) => RespuestaOperacion(await pisoService.RemoveAsync(model));

        [HttpPut("ActivarPiso")]
        public async Task<IActionResult> Activar(int pisoId, int? usuarioModificacion) => RespuestaOperacion(await pisoService.ActivarAsync(pisoId, usuarioModificacion));
    }
}
