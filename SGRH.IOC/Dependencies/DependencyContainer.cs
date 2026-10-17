using Microsoft.Extensions.DependencyInjection;
using SGRH.Domain.Services;

namespace SGRH.IOC.Dependencies
{
    /// <summary>
    /// Punto de composición: el único lugar que conoce las clases concretas (DIP).
    /// La Api y la Web solo llaman a AddSgrhDependencies().
    /// </summary>
    public static class DependencyContainer
    {
        public static IServiceCollection AddSgrhDependencies(this IServiceCollection services)
        {
            // Dominio
            services.AddSingleton<ICalculadoraPrecio, CalculadoraPrecioPorNoche>();

            // Persistencia: aquí se registrarán los repositorios de SGRH.Persistence
            // (IHotelRepository, IReservaRepository, ...) cuando se implementen.

            // Aplicación: aquí se registrarán los servicios de SGRH.Application.

            // Infraestructura: aquí se registrará INotificador (correo, consola, ...).

            return services;
        }
    }
}
