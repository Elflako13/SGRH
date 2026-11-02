using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.BL.Configuration;

namespace SGRH.BL
{
    /// <summary>
    /// Punto de entrada de la configuración de la aplicación: lee las opciones de negocio
    /// de appsettings.json y registra todas las dependencias (servicios y repositorios).
    /// La Web solo llama a builder.Services.AddApplication(builder.Configuration).
    /// </summary>
    public static class ApplicationSetup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ReservaOptions>(configuration.GetSection(ReservaOptions.Seccion));
            services.Configure<NotificacionOptions>(configuration.GetSection(NotificacionOptions.Seccion));

            services.AddBusinessLayer(configuration);

            return services;
        }
    }
}
