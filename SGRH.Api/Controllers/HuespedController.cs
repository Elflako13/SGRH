using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Huesped;

namespace SGRH.Api.Controllers
{
    public class HuespedController : BaseApiController
    {
        private readonly IHuespedService huespedService;

        public HuespedController(IHuespedService huespedService)
        {
            this.huespedService = huespedService;
        }

        [HttpGet("GetHuespedes")]
        public async Task<IActionResult> Get() => Respuesta(await huespedService.GetAllAsync());

        [HttpGet("GetHuesped")]
        public async Task<IActionResult> GetById(int huespedId) => Respuesta(await huespedService.GetByIdAsync(huespedId));


        [HttpGet("GetHuespedByDocumento")]
        public async Task<IActionResult> GetByDocumento(string documento) => Respuesta(await huespedService.GetByDocumentoAsync(documento));

        [HttpPost("SaveHuesped")]
        public async Task<IActionResult> Save([FromBody] HuespedAdd model) => RespuestaOperacion(await huespedService.AddAsync(model));

        [HttpPut("UpdateHuesped")]
        public async Task<IActionResult> Update([FromBody] HuespedUpdate model) => RespuestaOperacion(await huespedService.UpdateAsync(model));

        [HttpPut("RemoveHuesped")]
        public async Task<IActionResult> Remove([FromBody] HuespedRemove model) => RespuestaOperacion(await huespedService.RemoveAsync(model));

    }
}
