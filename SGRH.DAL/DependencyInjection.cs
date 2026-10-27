using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.DAL.Data;

namespace SGRH.DAL
{
    /// <summary>Registra el contexto y los repositorios de la capa de acceso a datos.</summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SGRHDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SGRHDb")));

            // Repositorios

            return services;
        }
    }
}
