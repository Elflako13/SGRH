using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Habitacion;

namespace SGRH.Api.Controllers
{
    public class HabitacionController : BaseApiController
    {
        private readonly IHabitacionService habitacionService;

        public HabitacionController(IHabitacionService habitacionService)
        {
            this.habitacionService = habitacionService;
        }

        [HttpGet("GetHabitaciones")]
        public async Task<IActionResult> Get() => Respuesta(await habitacionService.GetAllAsync());

        [HttpGet("GetHabitacion")]
        public async Task<IActionResult> GetById(int habitacionId) => Respuesta(await habitacionService.GetByIdAsync(habitacionId));

        [HttpGet("GetHabitacionesByHotel")]
        public async Task<IActionResult> GetByHotel(int hotelId) => Respuesta(await habitacionService.GetByHotelAsync(hotelId));

        [HttpGet("GetDisponibles")]
        public async Task<IActionResult> GetDisponibles(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes) =>
            Respuesta(await habitacionService.GetDisponiblesAsync(hotelId, entrada, salida, cantidadHuespedes));

        [HttpPost("SaveHabitacion")]
        public async Task<IActionResult> Save([FromBody] HabitacionAdd model) => RespuestaOperacion(await habitacionService.AddAsync(model));

        [HttpPut("UpdateHabitacion")]
        public async Task<IActionResult> Update([FromBody] HabitacionUpdate model) => RespuestaOperacion(await habitacionService.UpdateAsync(model));

        [HttpPut("CambiarEstado")]
        public async Task<IActionResult> CambiarEstado(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion) =>
            RespuestaOperacion(await habitacionService.CambiarEstadoAsync(habitacionId, estado, usuarioModificacion));

        [HttpPut("RemoveHabitacion")]
        public async Task<IActionResult> Remove([FromBody] HabitacionRemove model) => RespuestaOperacion(await habitacionService.RemoveAsync(model));
    }
}
