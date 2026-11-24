using Microsoft.AspNetCore.Mvc;
using SGRH.Api.Models;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Usuario;

namespace SGRH.Api.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly IUsuarioService usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        [HttpGet("GetUsuarios")]
        public async Task<IActionResult> Get() => Respuesta(await usuarioService.GetAllAsync());

        [HttpGet("GetUsuario")]
        public async Task<IActionResult> GetById(int usuarioId) => Respuesta(await usuarioService.GetByIdAsync(usuarioId));

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model) => Respuesta(await usuarioService.LoginAsync(model.Usuario, model.Clave));

        [HttpPost("RegistrarCliente")]
        public async Task<IActionResult> RegistrarCliente([FromBody] ClienteRegistro model) => RespuestaOperacion(await usuarioService.RegistrarClienteAsync(model));

        [HttpPost("SaveUsuario")]
        public async Task<IActionResult> Save([FromBody] UsuarioAdd model) => RespuestaOperacion(await usuarioService.AddAsync(model));

        [HttpPut("UpdateUsuario")]
        public async Task<IActionResult> Update([FromBody] UsuarioUpdate model) => RespuestaOperacion(await usuarioService.UpdateAsync(model));

        [HttpPut("RemoveUsuario")]
        public async Task<IActionResult> Remove([FromBody] UsuarioRemove model) => RespuestaOperacion(await usuarioService.RemoveAsync(model));
    }
}
