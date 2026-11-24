using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Bloque;

namespace SGRH.Api.Controllers
{
    public class BloqueController : BaseApiController
    {
        private readonly IBloqueService bloqueService;

        public BloqueController(IBloqueService bloqueService)
        {
            this.bloqueService = bloqueService;
        }

        [HttpGet("GetBloques")]
        public async Task<IActionResult> Get() => Respuesta(await bloqueService.GetAllAsync());

        [HttpGet("GetBloque")]
        public async Task<IActionResult> GetById(int bloqueId) => Respuesta(await bloqueService.GetByIdAsync(bloqueId));

        [HttpGet("GetBloquesByHotel")]
        public async Task<IActionResult> GetByHotel(int hotelId) => Respuesta(await bloqueService.GetByHotelAsync(hotelId));

        [HttpPost("SaveBloque")]
        public async Task<IActionResult> Save([FromBody] BloqueAdd model) => RespuestaOperacion(await bloqueService.AddAsync(model));

        [HttpPut("UpdateBloque")]
        public async Task<IActionResult> Update([FromBody] BloqueUpdate model) => RespuestaOperacion(await bloqueService.UpdateAsync(model));

        [HttpPut("RemoveBloque")]
        public async Task<IActionResult> Remove([FromBody] BloqueRemove model) => RespuestaOperacion(await bloqueService.RemoveAsync(model));

        [HttpPut("ActivarBloque")]
        public async Task<IActionResult> Activar(int bloqueId, int? usuarioModificacion) => RespuestaOperacion(await bloqueService.ActivarAsync(bloqueId, usuarioModificacion));
    }
}
