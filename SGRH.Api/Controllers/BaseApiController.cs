using Microsoft.AspNetCore.Mvc;
using SGRH.DAL.Models.Result;
using System.Text.RegularExpressions;

namespace SGRH.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        private ILogger Logger => HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        protected IActionResult Respuesta<T>(OperationResult<T> result)
        {
            if (result.Success)
                return Ok(result.Data);

            RegistrarError(result.Message);
            return BadRequest(Limpiar(result.Message));
        }

        protected IActionResult RespuestaOperacion<T>(OperationResult<T> result)
        {
            result.Message = Limpiar(result.Message);
            if (result.Success)
                return Ok(result);

            RegistrarError(result.Message);
            return BadRequest(result);
        }

        private void RegistrarError(string mensaje) =>
            Logger.LogWarning("Error en {Metodo} {Ruta}: {Mensaje}", Request.Method, Request.Path, mensaje);

        private static string Limpiar(string mensaje) =>
            Regex.Replace(mensaje, @"\s*\((?:CU|RN|RNF|RF)-\d{3}[^)]*\)", string.Empty);
    }
}
