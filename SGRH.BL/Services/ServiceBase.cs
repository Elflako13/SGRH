using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Result;
using System.Diagnostics;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    /// <summary>
    /// Base de los servicios de la capa de negocio. Concentra en un solo lugar (DRY):
    /// - el registro de eventos (logging) de cada operación: inicio, resultado y errores,
    /// - el manejo de excepciones (tolerancia a fallos): ningún error inesperado sale del servicio;
    ///   se registra con su tipo y TraceId y se devuelve un OperationResult con un mensaje claro.
    /// </summary>
    public abstract class ServiceBase
    {
        protected readonly ILogger logger;

        protected ServiceBase(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Ejecuta una operación del servicio con logging y manejo de excepciones.</summary>
        /// <param name="operacion">Descripción corta de la operación, por ejemplo "registrar hotel".</param>
        /// <param name="accion">Lógica de la operación.</param>
        /// <param name="usuarioId">Usuario que realiza la operación, si se conoce.</param>
        protected async Task<OperationResult<T>> EjecutarAsync<T>(string operacion, Func<Task<OperationResult<T>>> accion, int? usuarioId = null)
        {
            string traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");

            try
            {
                logger.LogInformation("Iniciando operación de {Operacion}. TraceId: {TraceId}", operacion, traceId);

                var result = await accion();

                if (result.Success)
                    logger.LogInformation("Operación de {Operacion} completada. Usuario: {UsuarioId}, Fecha: {Fecha}, TraceId: {TraceId}",
                                          operacion, usuarioId, DateTime.Now, traceId);
                else
                    logger.LogWarning("Operación de {Operacion} no completada: {Mensaje}. Usuario: {UsuarioId}, TraceId: {TraceId}",
                                      operacion, result.Message, usuarioId, traceId);

                return result;
            }
            catch (ReglaNegocioException ex)
            {
                // Regla de negocio incumplida: no es un error del sistema, se informa al usuario.
                logger.LogWarning("Regla de negocio incumplida al {Operacion}: {Mensaje}. TraceId: {TraceId}", operacion, ex.Message, traceId);
                return Failure<T>(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al {Operacion}. Tipo: {ExceptionType}. TraceId: {TraceId}", operacion, ex.GetType().Name, traceId);
                return Failure<T>($"Ocurrió un error inesperado al {operacion}. Intente de nuevo; si el problema continúa, contacte al administrador.");
            }
        }

        /// <summary>
        /// Igual que la anterior, pero primero ejecuta las validaciones del servicio.
        /// Si alguna falla, no se llama al repositorio y se devuelve el mensaje de la validación.
        /// </summary>
        protected Task<OperationResult<T>> EjecutarAsync<T>(string operacion, Func<string?> validar, Func<Task<OperationResult<T>>> accion, int? usuarioId = null)
        {
            return EjecutarAsync(operacion, async () =>
            {
                var error = validar();
                return error is not null ? Failure<T>(error) : await accion();
            }, usuarioId);
        }

        /// <summary>Validación común: el objeto recibido es obligatorio.</summary>
        protected static string? Requerido(object? valor, string nombre) =>
            valor is null ? $"El objeto {nombre} es requerido." : null;

        /// <summary>Validación común: el identificador debe ser mayor que cero.</summary>
        protected static string? ValidarId(int id, string entidad) =>
            id <= 0 ? $"El identificador de {entidad} debe ser mayor que cero." : null;

        /// <summary>Devuelve la primera validación que falle o null si todas pasan.</summary>
        protected static string? PrimerError(params string?[] validaciones) =>
            validaciones.FirstOrDefault(v => v is not null);
    }
}
