using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Hotel;

namespace SGRH.Api.Controllers
{
    public class HotelController : BaseApiController
    {
        private readonly IHotelService hotelService;

        public HotelController(IHotelService hotelService)
        {
            this.hotelService = hotelService;
        }

        [HttpGet("GetHoteles")]
        public async Task<IActionResult> Get() => Respuesta(await hotelService.GetAllAsync());

        [HttpGet("GetHotel")]
        public async Task<IActionResult> GetById(int hotelId) => Respuesta(await hotelService.GetByIdAsync(hotelId));


        [HttpPost("SaveHotel")]
        public async Task<IActionResult> Save([FromBody] HotelAdd model) => RespuestaOperacion(await hotelService.AddAsync(model));

        [HttpPut("UpdateHotel")]
        public async Task<IActionResult> Update([FromBody] HotelUpdate model) => RespuestaOperacion(await hotelService.UpdateAsync(model));

        [HttpPut("RemoveHotel")]
        public async Task<IActionResult> Remove([FromBody] HotelRemove model) => RespuestaOperacion(await hotelService.RemoveAsync(model));

        [HttpPut("ActivarHotel")]
        public async Task<IActionResult> Activar(int hotelId, int? usuarioModificacion) => RespuestaOperacion(await hotelService.ActivarAsync(hotelId, usuarioModificacion));
    }
}
