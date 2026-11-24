using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface IUsuarioService
    {
        Task<OperationResult<List<UsuarioModel>>> GetAllAsync();
        Task<OperationResult<UsuarioModel>> GetByIdAsync(int id);
        Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario);
        Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario);
        Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario);
        Task<OperationResult<bool>> RemoveAsync(UsuarioRemove usuario);

        Task<OperationResult<UsuarioModel>> LoginAsync(string nombreUsuario, string clave);
        Task<OperationResult<UsuarioModel>> RegistrarClienteAsync(ClienteRegistro registro);

        Task<OperationResult<bool>> TienePermisoAsync(int usuarioId, string accion);
    }
}
