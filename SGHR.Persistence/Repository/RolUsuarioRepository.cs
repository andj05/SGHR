using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Configurations;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repository
{
    public class RolUsuarioRepository : BaseRepository<RolUsuario>, IRolUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<RolUsuarioRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public RolUsuarioRepository(SGHRContext context,
                                    ILogger<RolUsuarioRepository> logger,
                                    IConfiguration configuration,
                                    MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        public override async Task<List<RolUsuario>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolUsuario>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["RolUsuario"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<RolUsuario> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                return null;
            }

            try
            {
                var rolUsuario = await _context.Set<RolUsuario>().FindAsync(id);
                if (rolUsuario == null)
                {
                    _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    return null;
                }
                return rolUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["RolUsuario"]["GetByIdError"]);
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<RolUsuario, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.Set<RolUsuario>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["RolUsuario"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario rolUsuario)
        {
            var validationResult = ValidateRolUsuario(rolUsuario);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                rolUsuario.FechaCreacion = DateTime.UtcNow;
                rolUsuario.ModifyDate = DateTime.UtcNow;
                rolUsuario.ModifyUser = 1;

                await _context.Set<RolUsuario>().AddAsync(rolUsuario);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = rolUsuario
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

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario rolUsuario)
        {
            var validation = ValidateRolUsuario(rolUsuario);
            if (validation.Success != true)
                return validation;

            var result = new OperationResult();
            try
            {
                var existingRolUsuario = await _context.Set<RolUsuario>().FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingRolUsuario.Descripcion = rolUsuario.Descripcion;
                existingRolUsuario.Estado = rolUsuario.Estado;
                existingRolUsuario.ModifyDate = DateTime.UtcNow;
                existingRolUsuario.ModifyUser = 1;// En producción, obtener el usuario autenticado.

                _context.Update(existingRolUsuario);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingRolUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(RolUsuario rolUsuario)
        {
            if (rolUsuario == null || rolUsuario.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingRolUsuario = await _context.RolUsuario.FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.RolUsuario.Remove(existingRolUsuario);
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
                    Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]
                };
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(RolUsuario rolUsuario)
        {
            try
            {
                var existingRolUsuario = await _context.Set<RolUsuario>().FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingRolUsuario.Deleted = false;
                existingRolUsuario.ModifyDate = DateTime.UtcNow;
                existingRolUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

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
                    Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]
                };
            }
        }

        private OperationResult ValidateRolUsuario(dynamic rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"]
                };
            }

            if (rolUsuario.Descripcion != null && rolUsuario.Descripcion.Length > 50)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["InvalidDescription"]
                };
            }

            if (rolUsuario.CreationUser <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["InvalidCreationUser"]
                };
            }

            return new OperationResult { Success = true };
        }
    }
}