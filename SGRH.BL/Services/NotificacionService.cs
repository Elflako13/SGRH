using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository notificacionRepository;

        public NotificacionService(INotificacionRepository notificacionRepository)
        {
            this.notificacionRepository = notificacionRepository;
        }

        public Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId) => notificacionRepository.GetByReservaAsync(reservaId);

        public Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync() => notificacionRepository.GetPendientesDeReintentoAsync();

        public Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion) => notificacionRepository.AddAsync(notificacion);

        public Task<OperationResult<bool>> MarcarEnviadaAsync(int notificacionId) => notificacionRepository.MarcarEnviadaAsync(notificacionId);

        public Task<OperationResult<bool>> MarcarFallidaAsync(int notificacionId, string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                error = "Error no especificado.";

            return notificacionRepository.MarcarFallidaAsync(notificacionId, error);
        }
    }
}
