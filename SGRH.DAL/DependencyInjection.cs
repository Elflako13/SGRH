using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.DAL.Repositories;
using SGRH.DAL.Data;

namespace SGRH.DAL
{
    /// <summary>Registra el contexto y los repositorios de la capa de acceso a datos.</summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Por defecto se usa SQL Server. Con "ProveedorBaseDatos": "Sqlite" se puede usar un archivo
            // SQLite (por ejemplo, para probar la aplicación en una PC sin SQL Server).
            string? cadena = configuration.GetConnectionString("SGRHDb");
            bool usarSqlite = string.Equals(configuration["ProveedorBaseDatos"], "Sqlite", StringComparison.OrdinalIgnoreCase);

            services.AddDbContext<SGRHDbContext>(options =>
            {
                if (usarSqlite)
                    options.UseSqlite(cadena);
                else
                    options.UseSqlServer(cadena);
            });

            // Repositorios
            services.AddScoped<ICadenaHoteleraRepository, CadenaHoteleraRepository>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IBloqueRepository, BloqueRepository>();
            services.AddScoped<IPisoRepository, PisoRepository>();
            services.AddScoped<ITipoHabitacionRepository, TipoHabitacionRepository>();
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IHuespedRepository, HuespedRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IHistorialReservaRepository, HistorialReservaRepository>();
            services.AddScoped<INotificacionRepository, NotificacionRepository>();

            return services;
        }
    }
}
