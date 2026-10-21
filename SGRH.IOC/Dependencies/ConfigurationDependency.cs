using Microsoft.Extensions.DependencyInjection;
using SGRH.Persistence.Interfaces.Configuration;
using SGRH.Persistence.Repositories.Configuration;

namespace SGRH.IOC.Dependencies
{
    public static class ConfigurationDependency
    {
        public static IServiceCollection AddConfigurationDependency(this IServiceCollection services)
        {
            services.AddScoped<ICadenaHoteleraRepository, CadenaHoteleraRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IPisoRepository, PisoRepository>();
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();
            return services;
        }
    }
}
