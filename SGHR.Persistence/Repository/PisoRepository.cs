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
    public class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<PisoRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public PisoRepository(SGHRContext context,
                              ILogger<PisoRepository> logger,
                              IConfiguration configuration,
                              MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        public override async Task<List<Piso>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Piso>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Piso"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<Piso> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                return null;
            }

            try
            {
                var piso = await _context.Set<Piso>().FindAsync(id);
                if (piso == null)
                {
                    _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    return null;
                }
                return piso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Piso"]["GetByIdError"]);
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Piso, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.Set<Piso>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Piso"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Piso piso)
        {
            var validationResult = ValidatePiso(piso);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                piso.FechaCreacion = DateTime.UtcNow;
                piso.ModifyDate = DateTime.UtcNow;
                piso.ModifyUser = 1;

                await _context.Set<Piso>().AddAsync(piso);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = piso
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

        public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
        {
            var validation = ValidatePiso(piso);
            if (validation.Success != true)
                return validation;

            var result = new OperationResult();
            try
            {
                var existingPiso = await _context.Set<Piso>().FindAsync(piso.Id);
                if (existingPiso == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingPiso.Descripcion = piso.Descripcion ?? existingPiso.Descripcion;
                existingPiso.Estado = piso.Estado;
                existingPiso.ModifyDate = DateTime.UtcNow;
                existingPiso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Update(existingPiso);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingPiso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Piso piso)
        {
            if (piso == null || piso.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingPiso = await _context.Pisos.FindAsync(piso.Id);
                if (existingPiso == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.Pisos.Remove(existingPiso);
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

        public override async Task<OperationResult> RestoreEntityAsync(Piso piso)
        {
            try
            {
                var existingPiso = await _context.Set<Piso>().FindAsync(piso.Id);
                if (existingPiso == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingPiso.Deleted = false;
                existingPiso.ModifyDate = DateTime.UtcNow;
                existingPiso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

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

        private OperationResult ValidatePiso(Piso piso)
        {
            if (piso == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"]
                };
            }

            if (piso.Descripcion != null && piso.Descripcion.Length > 50)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Piso"]["InvalidDescription"]
                };
            }

            if (piso.CreationUser <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Piso"]["InvalidCreationUser"]
                };
            }

            return new OperationResult { Success = true };
        }
    }
}