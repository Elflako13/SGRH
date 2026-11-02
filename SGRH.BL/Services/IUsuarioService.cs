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

        /// <summary>Inicio de sesión del personal del hotel.</summary>
        Task<OperationResult<UsuarioModel>> LoginAsync(string nombreUsuario, string clave);

        /// <summary>RNF-001: ¿el usuario puede realizar esta acción?</summary>
        Task<OperationResult<bool>> TienePermisoAsync(int usuarioId, string accion);
    }
}
