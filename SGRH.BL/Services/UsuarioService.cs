using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository;
        }

        public Task<OperationResult<List<UsuarioModel>>> GetAllAsync() => usuarioRepository.GetAllAsync();

        public Task<OperationResult<UsuarioModel>> GetByIdAsync(int id) => usuarioRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario) => usuarioRepository.AddAsync(usuario);

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario) => usuarioRepository.UpdateAsync(usuario);

        public Task<OperationResult<bool>> RemoveAsync(UsuarioRemove usuario) => usuarioRepository.RemoveAsync(usuario);

        public Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario) => usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);
    }
}
