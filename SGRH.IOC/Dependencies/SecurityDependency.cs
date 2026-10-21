using Microsoft.Extensions.DependencyInjection;
using SGRH.Persistence.Interfaces.Security;
using SGRH.Persistence.Repositories.Security;

namespace SGRH.IOC.Dependencies
{
    public static class SecurityDependency
    {
        public static IServiceCollection AddSecurityDependency(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            return services;
        }
    }
}
