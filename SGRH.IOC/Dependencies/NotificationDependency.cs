using Microsoft.Extensions.DependencyInjection;
using SGRH.Persistence.Interfaces.Notification;
using SGRH.Persistence.Repositories.Notification;

namespace SGRH.IOC.Dependencies
{
    public static class NotificationDependency
    {
        public static IServiceCollection AddNotificationDependency(this IServiceCollection services)
        {
            services.AddScoped<INotificacionRepository, NotificacionRepository>();
            return services;
        }
    }
}
