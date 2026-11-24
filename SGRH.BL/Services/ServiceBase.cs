using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Result;
using System.Diagnostics;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    public abstract class ServiceBase
    {
        protected readonly ILogger logger;

        protected ServiceBase(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                    logger.LogWarning("Operación de {Operacion} no completada: {Mensaje} (Usuario: {UsuarioId}, TraceId: {TraceId})",
                                      operacion, result.Message, usuarioId, traceId);

                return result;
            }
            catch (ReglaNegocioException ex)
            {
                logger.LogWarning("Regla de negocio incumplida al {Operacion}: {Mensaje}. TraceId: {TraceId}", operacion, ex.Message, traceId);
                return Failure<T>(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al {Operacion}. Tipo: {ExceptionType}. TraceId: {TraceId}", operacion, ex.GetType().Name, traceId);
                return Failure<T>($"Ocurrió un error inesperado al {operacion}. Intente de nuevo; si el problema continúa, contacte al administrador.");
            }
        }

        protected Task<OperationResult<T>> EjecutarAsync<T>(string operacion, Func<string?> validar, Func<Task<OperationResult<T>>> accion, int? usuarioId = null)
        {
            return EjecutarAsync(operacion, async () =>
            {
                var error = validar();
                return error is not null ? Failure<T>(error) : await accion();
            }, usuarioId);
        }

        protected static string? Requerido(object? valor, string nombre) =>
            valor is null ? $"El objeto {nombre} es requerido." : null;

        protected static string? ValidarId(int id, string entidad) =>
            id <= 0 ? $"El identificador de {entidad} debe ser mayor que cero." : null;

        protected static string? PrimerError(params string?[] validaciones) =>
            validaciones.FirstOrDefault(v => v is not null);
    }
}
