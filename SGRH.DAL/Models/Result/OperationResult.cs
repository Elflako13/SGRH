namespace SGRH.DAL.Models.Result
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public static class OperationResult
    {
        public static OperationResult<T> Success<T>(T data, string message = "Operación realizada correctamente.")
        {
            return new OperationResult<T> { Success = true, Data = data, Message = message };
        }

        public static OperationResult<T> Failure<T>(string message)
        {
            return new OperationResult<T> { Success = false, Message = message };
        }
    }
}
