using SGRH.Domain.Entities.Security;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Security
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    }
}
