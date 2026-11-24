using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.CadenaHotelera;

namespace SGRH.Api.Controllers
{
    public class CadenaHoteleraController : BaseApiController
    {
        private readonly ICadenaHoteleraService cadenaHoteleraService;

        public CadenaHoteleraController(ICadenaHoteleraService cadenaHoteleraService)
        {
            this.cadenaHoteleraService = cadenaHoteleraService;
        }

        [HttpGet("GetCadenasHoteleras")]
        public async Task<IActionResult> Get() => Respuesta(await cadenaHoteleraService.GetAllAsync());

        [HttpGet("GetCadenaHotelera")]
        public async Task<IActionResult> GetById(int cadenaHoteleraId) => Respuesta(await cadenaHoteleraService.GetByIdAsync(cadenaHoteleraId));


        [HttpPost("SaveCadenaHotelera")]
        public async Task<IActionResult> Save([FromBody] CadenaHoteleraAdd model) => RespuestaOperacion(await cadenaHoteleraService.AddAsync(model));

        [HttpPut("UpdateCadenaHotelera")]
        public async Task<IActionResult> Update([FromBody] CadenaHoteleraUpdate model) => RespuestaOperacion(await cadenaHoteleraService.UpdateAsync(model));

        [HttpPut("RemoveCadenaHotelera")]
        public async Task<IActionResult> Remove([FromBody] CadenaHoteleraRemove model) => RespuestaOperacion(await cadenaHoteleraService.RemoveAsync(model));

    }
}
