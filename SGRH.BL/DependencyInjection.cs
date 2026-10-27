using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.DAL;

namespace SGRH.BL
{
    /// <summary>
    /// Registra la capa de negocio y, a través de ella, la de acceso a datos.
    /// La Web solo llama a AddBusinessLayer(builder.Configuration).
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccessLayer(configuration);

            // Servicios

            return services;
        }
    }
}
