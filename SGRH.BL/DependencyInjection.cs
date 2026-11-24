using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGRH.BL.Correo;
using SGRH.BL.Services;
using SGRH.DAL;

namespace SGRH.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccessLayer(configuration);

            services.AddScoped<IEnviadorCorreo, EnviadorCorreoSmtp>();

            services.AddScoped<ICadenaHoteleraService, CadenaHoteleraService>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IBloqueService, BloqueService>();
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<ITipoHabitacionService, TipoHabitacionService>();
            services.AddScoped<IHabitacionService, HabitacionService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IHuespedService, HuespedService>();
            services.AddScoped<IReservaService, ReservaService>();
            services.AddScoped<INotificacionService, NotificacionService>();

            return services;
        }
    }
}
