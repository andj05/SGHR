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
                    return new OperationResult { Success = false, Message = $"{invalidState} Estado de usuario inválido. Debe ser 0 o 1." };
                }

                bool estado = idEstadoUsuario == 1;
                var usuario = await _context.Set<Usuario>()
                    .AsNoTracking()
                    .Where(c => c.Estado == estado)
                    .ToListAsync();

                _logger.LogInfo($"Se recuperaron {usuario.Count} usuario con el estado {idEstadoUsuario}");

                return new OperationResult { Success = true, Data = usuario };
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
            if (usuario == null)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }

            try
            {
                usuario.FechaCreacion = DateTime.UtcNow;
                usuario.ModifyDate = DateTime.UtcNow;
                usuario.ModifyUser = 1;

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
            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                // Actualizar los datos modificables con Entity Framework Core
                _context.Entry(existingUsuario).CurrentValues.SetValues(usuario);

                // Excluir propiedades que no deben ser actualizadas
                _context.Entry(existingUsuario).Property(x => x.FechaCreacion).IsModified = false;
                _context.Entry(existingUsuario).Property(x => x.CreationUser).IsModified = false;

                // Actualizar ModifyDate y ModifyUser
                existingUsuario.ModifyDate = DateTime.UtcNow;
                existingUsuario.ModifyUser = 1; // Obtener el usuario autenticado en producción

                // Guardar cambios
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
            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
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
            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingUsuario.Deleted = false;
                existingUsuario.ModifyDate = DateTime.UtcNow;
                existingUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

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