using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.BL.Services;
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
            services.AddScoped<ICadenaHoteleraService, CadenaHoteleraService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<IHabitacionService, HabitacionService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IHuespedService, HuespedService>();
            services.AddScoped<IReservaService, ReservaService>();
            services.AddScoped<INotificacionService, NotificacionService>();

            return services;
        }
    }
}
