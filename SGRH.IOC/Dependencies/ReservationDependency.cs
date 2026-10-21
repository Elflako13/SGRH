using Microsoft.Extensions.DependencyInjection;
using SGRH.Persistence.Interfaces.Reservation;
using SGRH.Persistence.Repositories.Reservation;

namespace SGRH.IOC.Dependencies
{
    public static class ReservationDependency
    {
        public static IServiceCollection AddReservationDependency(this IServiceCollection services)
        {
            services.AddScoped<IHuespedRepository, HuespedRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IHistorialReservaRepository, HistorialReservaRepository>();
            return services;
        }
    }
}
