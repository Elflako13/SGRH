using SGRH.DAL.Models.Objects.HistorialReserva;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    public interface IHistorialReservaRepository
    {
        Task<OperationResult<bool>> AddAsync(HistorialReservaAdd historial);
        Task<OperationResult<List<HistorialReservaModel>>> GetByReservaAsync(int reservaId);
    }
}
