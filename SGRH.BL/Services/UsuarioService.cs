using Microsoft.Extensions.Logging;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    public class UsuarioService : ServiceBase, IUsuarioService
    {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly IHuespedRepository huespedRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, IHuespedRepository huespedRepository, ILogger<UsuarioService> logger) : base(logger)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            this.huespedRepository = huespedRepository ?? throw new ArgumentNullException(nameof(huespedRepository));
        }

        public Task<OperationResult<UsuarioModel>> RegistrarClienteAsync(ClienteRegistro registro) =>
            EjecutarAsync("registrar cliente", () => Requerido(registro, nameof(ClienteRegistro)) ?? ValidarRegistro(registro), async () =>
            {
                string documento = registro.Documento!.Trim();
                string correo = registro.Correo!.Trim();

                if (await usuarioRepository.ExisteCorreoAsync(correo))
                    return Failure<UsuarioModel>("Ya existe una cuenta con ese correo.");

                var huesped = await huespedRepository.GetByDocumentoAsync(documento);
                if (huesped.Success)
                {
                    if (await usuarioRepository.ExisteParaHuespedAsync(huesped.Data!.HuespedId))
                        return Failure<UsuarioModel>("Ya existe una cuenta con ese documento.");
                }
                else
                {
                    var nuevoHuesped = await huespedRepository.AddAsync(new HuespedAdd
                    {
                        Nombre = registro.Nombre,
                        Apellido = registro.Apellido,
                        Documento = documento,
                        Correo = correo,
                        Telefono = registro.Telefono
                    });
                    if (!nuevoHuesped.Success)
                        return Failure<UsuarioModel>(nuevoHuesped.Message);

                    huesped = await huespedRepository.GetByDocumentoAsync(documento);
                    if (!huesped.Success)
                        return Failure<UsuarioModel>(huesped.Message);
                }

                var usuario = await usuarioRepository.AddAsync(new UsuarioAdd
                {
                    NombreUsuario = correo,
                    NombreCompleto = $"{registro.Nombre!.Trim()} {registro.Apellido!.Trim()}",
                    Correo = correo,
                    Rol = Rol.Cliente,
                    Clave = registro.Clave,
                    HuespedId = huesped.Data!.HuespedId
                });
                if (!usuario.Success)
                    return Failure<UsuarioModel>(usuario.Message);

                var creado = await usuarioRepository.GetByNombreUsuarioAsync(correo);
                return creado.Success
                    ? Success(creado.Data!, $"Bienvenido(a), {creado.Data!.NombreCompleto}. Su cuenta fue creada.")
                    : creado;
            });

        private static string? ValidarRegistro(ClienteRegistro registro)
        {
            if (string.IsNullOrWhiteSpace(registro.Nombre) || string.IsNullOrWhiteSpace(registro.Apellido))
                return "El nombre y el apellido son obligatorios.";

            if (string.IsNullOrWhiteSpace(registro.Documento))
                return "El documento es obligatorio.";

            if (string.IsNullOrWhiteSpace(registro.Correo) || !registro.Correo.Contains('@'))
                return "Debe indicar un correo válido.";

            if (string.IsNullOrWhiteSpace(registro.Clave) || registro.Clave.Length < 6)
                return "La contraseña debe tener al menos 6 caracteres.";

            return null;
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
