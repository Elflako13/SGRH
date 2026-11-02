using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

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

        public Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario) => usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario);

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario) => usuarioRepository.AddAsync(usuario);

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario) => usuarioRepository.UpdateAsync(usuario);

        public Task<OperationResult<bool>> RemoveAsync(UsuarioRemove usuario) => usuarioRepository.RemoveAsync(usuario);

        public async Task<OperationResult<UsuarioModel>> LoginAsync(string nombreUsuario, string clave)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(clave))
                return Failure<UsuarioModel>("Debe indicar el usuario y la clave.");

            var resultado = await usuarioRepository.ValidarCredencialesAsync(nombreUsuario.Trim(), clave);
            if (!resultado.Success)
                return resultado;

            return Success(resultado.Data!, $"Bienvenido(a), {resultado.Data!.NombreCompleto}.");
        }

        public async Task<OperationResult<bool>> TienePermisoAsync(int usuarioId, string accion)
        {
            var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
            if (!usuario.Success)
                return Failure<bool>(usuario.Message);

            bool permitido = Usuario.PuedeRealizar(usuario.Data!.Rol, accion);
            return permitido
                ? Success(true, "Acción permitida.")
                : Failure<bool>($"El rol {usuario.Data.Rol} no tiene permiso para {accion} (RNF-001).");
        }
    }
}
