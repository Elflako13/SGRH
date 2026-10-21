using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.Domain.Services;
using SGRH.Persistence.Context;

namespace SGRH.IOC.Dependencies
{
    /// <summary>
    /// Punto de composición: el único lugar que conoce las clases concretas (DIP).
    /// La Api y la Web solo llaman a AddSgrhDependencies(builder.Configuration).
    /// </summary>
    public static class DependencyContainer
    {
        public static IServiceCollection AddSgrhDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Base de datos (cadena de conexión "SGRHDb" en appsettings.json)
            services.AddDbContext<SGRHContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SGRHDb")));

            // Dominio
            services.AddSingleton<ICalculadoraPrecio, CalculadoraPrecioPorNoche>();

            // Repositorios por módulo (schema)

            return services;
        }
    }
}
