using Microsoft.Extensions.Logging;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    /// <summary>Usuarios, inicio de sesión y permisos por rol (RNF-001).</summary>
    public class UsuarioService : ServiceBase, IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger) : base(logger)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public Task<OperationResult<List<UsuarioModel>>> GetAllAsync() =>
            EjecutarAsync("consultar usuarios", () => usuarioRepository.GetAllAsync());

        public Task<OperationResult<UsuarioModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar usuario", () => ValidarId(id, "usuario"), () => usuarioRepository.GetByIdAsync(id));

        public Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario) =>
            EjecutarAsync("buscar usuario",
                          () => string.IsNullOrWhiteSpace(nombreUsuario) ? "Debe indicar el nombre de usuario." : null,
                          () => usuarioRepository.GetByNombreUsuarioAsync(nombreUsuario));

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario) =>
            EjecutarAsync("registrar usuario", () => Requerido(usuario, nameof(UsuarioAdd)),
                          () => usuarioRepository.AddAsync(usuario), usuario?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario) =>
            EjecutarAsync("actualizar usuario",
                          () => Requerido(usuario, nameof(UsuarioUpdate)) ?? ValidarId(usuario.Id, "usuario"),
                          () => usuarioRepository.UpdateAsync(usuario), usuario?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(UsuarioRemove usuario) =>
            EjecutarAsync("inactivar usuario",
                          () => Requerido(usuario, nameof(UsuarioRemove)) ?? ValidarId(usuario.Id, "usuario"),
                          () => usuarioRepository.RemoveAsync(usuario), usuario?.UsuarioModificacion);

        /// <summary>Inicio de sesión. El intento fallido queda registrado en el log (auditoría).</summary>
        public Task<OperationResult<UsuarioModel>> LoginAsync(string nombreUsuario, string clave) =>
            EjecutarAsync("iniciar sesión",
                          () => string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(clave) ? "Debe indicar el usuario y la clave." : null,
                          async () =>
                          {
                              var resultado = await usuarioRepository.ValidarCredencialesAsync(nombreUsuario.Trim(), clave);
                              if (!resultado.Success)
                              {
                                  logger.LogWarning("Intento de inicio de sesión fallido para el usuario {NombreUsuario}.", nombreUsuario.Trim());
                                  return resultado;
                              }

                              return Success(resultado.Data!, $"Bienvenido(a), {resultado.Data!.NombreCompleto}.");
                          });

        public Task<OperationResult<bool>> TienePermisoAsync(int usuarioId, string accion) =>
            EjecutarAsync("verificar permiso",
                          () => ValidarId(usuarioId, "usuario") ?? (string.IsNullOrWhiteSpace(accion) ? "Debe indicar la acción." : null),
                          async () =>
                          {
                              var usuario = await usuarioRepository.GetByIdAsync(usuarioId);
                              if (!usuario.Success)
                                  return Failure<bool>(usuario.Message);

                              bool permitido = Usuario.PuedeRealizar(usuario.Data!.Rol, accion);
                              if (!permitido)
                                  logger.LogWarning("Acceso denegado: el usuario {UsuarioId} con rol {Rol} intentó {Accion}.", usuarioId, usuario.Data.Rol, accion);

                              return permitido
                                  ? Success(true, "Acción permitida.")
                                  : Failure<bool>($"El rol {usuario.Data.Rol} no tiene permiso para {accion} (RNF-001).");
                          }, usuarioId);
    }
}
