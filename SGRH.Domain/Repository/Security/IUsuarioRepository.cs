using SGRH.Domain.Entities.Security;

namespace SGRH.Domain.Repository.Security
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    }
}
