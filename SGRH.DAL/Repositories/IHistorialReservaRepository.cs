using SGRH.DAL.Models.Objects.HistorialReserva;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    /// <summary>Bitácora de reservas (RF-021, RNF-007): solo se agregan y consultan registros.</summary>
    public interface IHistorialReservaRepository
    {
        Task<OperationResult<bool>> AddAsync(HistorialReservaAdd historial);
        Task<OperationResult<List<HistorialReservaModel>>> GetByReservaAsync(int reservaId);
    }
}
