using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SGRH.BL.Tests.Base
{
    /// <summary>Logger que guarda en memoria lo que registran los servicios, para comprobarlo en las pruebas.</summary>
    public sealed class LoggerDePrueba<T> : ILogger<T>
    {
        public List<(LogLevel Nivel, string Mensaje, Exception? Excepcion)> Entradas { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Entradas.Add((logLevel, formatter(state, exception), exception));
    }

    /// <summary>Atajo para pasar un logger vacío a los servicios cuando la prueba no revisa el log.</summary>
    public static class Log
    {
        public static ILogger<T> De<T>() => NullLogger<T>.Instance;
    }
}
