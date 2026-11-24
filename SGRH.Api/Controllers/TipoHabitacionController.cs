using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.TipoHabitacion;

namespace SGRH.Api.Controllers
{
    public class TipoHabitacionController : BaseApiController
    {
        private readonly ITipoHabitacionService tipoHabitacionService;

        public TipoHabitacionController(ITipoHabitacionService tipoHabitacionService)
        {
            this.tipoHabitacionService = tipoHabitacionService;
        }

        [HttpGet("GetTiposHabitacion")]
        public async Task<IActionResult> Get() => Respuesta(await tipoHabitacionService.GetAllAsync());

        [HttpGet("GetTipoHabitacion")]
        public async Task<IActionResult> GetById(int tipoHabitacionId) => Respuesta(await tipoHabitacionService.GetByIdAsync(tipoHabitacionId));


        [HttpPost("SaveTipoHabitacion")]
        public async Task<IActionResult> Save([FromBody] TipoHabitacionAdd model) => RespuestaOperacion(await tipoHabitacionService.AddAsync(model));

        [HttpPut("UpdateTipoHabitacion")]
        public async Task<IActionResult> Update([FromBody] TipoHabitacionUpdate model) => RespuestaOperacion(await tipoHabitacionService.UpdateAsync(model));

        [HttpPut("RemoveTipoHabitacion")]
        public async Task<IActionResult> Remove([FromBody] TipoHabitacionRemove model) => RespuestaOperacion(await tipoHabitacionService.RemoveAsync(model));

        [HttpPut("ActivarTipoHabitacion")]
        public async Task<IActionResult> Activar(int tipoHabitacionId, int? usuarioModificacion) => RespuestaOperacion(await tipoHabitacionService.ActivarAsync(tipoHabitacionId, usuarioModificacion));
    }
}
