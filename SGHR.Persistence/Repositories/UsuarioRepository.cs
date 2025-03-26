using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UsuarioRepository(SGHRContext context,
                                ILoggerManager logger,
                                MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        /// <summary>
        /// Valida la entidad Usuario para operaciones de creación o actualización.
        /// Se agregan validaciones para el correo y la contraseña.
        /// </summary>
        /// <param name="usuario">Entidad a validar.</param>
        /// <param name="isUpdate">Indica si la validación es para una actualización (se valida que el ID sea mayor a 0).</param>
        /// <returns>OperationResult con el resultado de la validación.</returns>
        private OperationResult ValidateUsuario(Usuario usuario, bool isUpdate = false)
        {
            var result = new OperationResult();

            if (usuario == null)
            {
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(result.Message);
                return result;
            }

            if (isUpdate && usuario.Id <= 0)
            {
                result.Success = false;
                result.Message = $"{_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]} Valor recibido: {usuario.Id}";
                _logger.LogWarn(result.Message);
                return result;
            }

            // Validación de correo obligatorio.
            if (string.IsNullOrWhiteSpace(usuario.Correo))
            {
                result.Success = false;
                result.Message = "El campo 'Correo' es obligatorio.";
                _logger.LogWarn(result.Message);
                return result;
            }

            // Validar el formato del correo utilizando una expresión regular.
            var emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            if (!emailRegex.IsMatch(usuario.Correo))
            {
                result.Success = false;
                result.Message = "El formato del correo electrónico es inválido.";
                _logger.LogWarn(result.Message);
                return result;
            }

            // Validación de contraseña obligatoria y su longitud mínima.
            if (string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 6)
            {
                result.Success = false;
                result.Message = "La contraseña es obligatoria y debe tener al menos 6 caracteres.";
                _logger.LogWarn(result.Message);
                return result;
            }

            // Aquí podrías agregar más validaciones, por ejemplo:
            // - Verificar que el correo no exista ya en la base de datos (si la lógica lo requiere).
            // - Validar otros campos obligatorios o reglas de negocio específicas.

            result.Success = true;
            return result;
        }

        public override async Task<List<Usuario>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Usuario>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<Usuario> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarn($"{_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]} Valor recibido: {id}");
                return null;
            }

            try
            {
                var usuario = await _context.Set<Usuario>().FindAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarn($"{_messageMapper.ErrorMessages["EntityBase"]["NotFound"]} ID: {id}");
                    return null;
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Usuario, bool>> filter)
        {
            var result = new OperationResult();
            try
            {
                if (filter == null)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                    return result;
                }

                _logger.LogInfo($"Aplicando filtro: {filter}");
                var filteredEntities = await base.GetFilteredAsync(filter);
                result.Success = true;
                result.Data = filteredEntities.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["DbException"];
            }
            return result;
        }

        public async Task<OperationResult> GetUsersByStatusAsync(int idEstadoUsuario)
        {
            try
            {
                if (idEstadoUsuario is not (0 or 1))
                {
                    string invalidState = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                    string msg = $"{invalidState} Estado de usuario inválido. Debe ser 0 o 1.";
                    _logger.LogWarn(msg);
                    return new OperationResult { Success = false, Message = msg };
                }

                bool estado = idEstadoUsuario == 1;
                var usuarios = await _context.Set<Usuario>()
                    .AsNoTracking()
                    .Where(u => u.Estado == estado)
                    .ToListAsync();

                _logger.LogInfo($"Se recuperaron {usuarios.Count} usuario(s) con el estado {idEstadoUsuario}");
                return new OperationResult { Success = true, Data = usuarios };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: Error al obtener usuario con estado {idEstadoUsuario}");
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["DbException"] + ": " + ex.Message
                };
            }
        }

        public override async Task<Usuario> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarn("El correo electrónico proporcionado es nulo o vacío.");
                    return null;
                }

                var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.Correo == email);
                if (usuario == null)
                {
                    _logger.LogWarn($"Usuario no encontrado con el correo {email}.");
                    return null;
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Usuario usuario)
        {
            // Validación del objeto Usuario antes de guardarlo.
            var validationResult = ValidateUsuario(usuario, isUpdate: false);
            if (!validationResult.Success)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"] + " - " + validationResult.Message
                };
            }

            try
            {
                usuario.FechaCreacion = DateTime.UtcNow;
                usuario.ModifyDate = DateTime.UtcNow;
                usuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                await _context.Set<Usuario>().AddAsync(usuario);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Data = usuario,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Usuario usuario)
        {
            var result = new OperationResult();

            // Validación de la entidad Usuario para actualización.
            var validationResult = ValidateUsuario(usuario, isUpdate: true);
            if (!validationResult.Success)
            {
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"] + " - " + validationResult.Message;
                return result;
            }

            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                // Actualizar los datos modificables con Entity Framework Core
                _context.Entry(existingUsuario).CurrentValues.SetValues(usuario);

                // Excluir propiedades que no deben ser actualizadas
                _context.Entry(existingUsuario).Property(x => x.FechaCreacion).IsModified = false;
                _context.Entry(existingUsuario).Property(x => x.CreationUser).IsModified = false;

                // Actualizar ModifyDate y ModifyUser
                existingUsuario.ModifyDate = DateTime.UtcNow;
                existingUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingUsuario;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"] + ": " + ex.Message;
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Usuario usuario)
        {
            // Validación básica del objeto
            if (usuario == null || usuario.Id <= 0)
            {
                string msg = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(msg);
                return new OperationResult
                {
                    Success = false,
                    Message = msg
                };
            }

            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                _context.Set<Usuario>().Remove(existingUsuario);
                await _context.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["DeleteSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"] + ": " + ex.Message
                };
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Usuario, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                return false;
            }

            try
            {
                return await _context.Set<Usuario>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(Usuario usuario)
        {
            // Validación básica del objeto
            if (usuario == null || usuario.Id <= 0)
            {
                string msg = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(msg);
                return new OperationResult
                {
                    Success = false,
                    Message = msg
                };
            }

            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                existingUsuario.Deleted = false;
                existingUsuario.ModifyDate = DateTime.UtcNow;
                existingUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado

                await _context.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["RestoreSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"] + ": " + ex.Message
                };
            }
        }
    }
}
