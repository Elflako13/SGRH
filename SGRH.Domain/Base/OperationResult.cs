namespace SGRH.Domain.Base
{
    /// <summary>
    /// Resultado estándar de una operación. Evita lanzar excepciones para errores esperados
    /// y le da a todas las capas la misma forma de responder.
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }

        public static OperationResult Ok(object? data = null, string? message = null)
        {
            return new OperationResult { Success = true, Data = data, Message = message };
        }

        public static OperationResult Fail(string message)
        {
            return new OperationResult { Success = false, Message = message };
        }
    }
}
