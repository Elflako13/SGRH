namespace SGRH.Domain.Exceptions
{
    /// <summary>
    /// Se lanza cuando una operación incumple una regla de negocio (RN-xxx).
    /// </summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string message) : base(message)
        {
        }
    }
}
