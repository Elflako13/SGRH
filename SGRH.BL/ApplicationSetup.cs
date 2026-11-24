using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.BL.Configuration;

namespace SGRH.BL
{
    public static class ApplicationSetup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ReservaOptions>(configuration.GetSection(ReservaOptions.Seccion));
            services.Configure<NotificacionOptions>(configuration.GetSection(NotificacionOptions.Seccion));
            services.Configure<CorreoOptions>(configuration.GetSection(CorreoOptions.Seccion));

            services.AddBusinessLayer(configuration);

            return services;
        }
    }
}
