using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Infraestructure.Logging.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class TarifasRepository : BaseRepository<Tarifas>, ITarifasRepository
    {
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public TarifasRepository(SGHRContext context,
                                    ILoggerManager logger,
                                    MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateTarifaPersistence(Tarifas tarifa)
        {
            if (tarifa == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
            return new OperationResult { Success = true };
        }

        public override async Task<List<Tarifas>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Tarifas>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Tarifas"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<Tarifas> GetEntityByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Set<Tarifas>()
                    .FirstOrDefaultAsync(t => t.Id == id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return null;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Tarifas, bool>> filter)
        {
            var result = new OperationResult();
            try
            {
                var filteredEntities = await base.GetFilteredAsync(filter);
                result.Success = true;
                result.Data = filteredEntities.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Tarifas, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.Set<Tarifas>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Tarifas"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifaPersistence(tarifa);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                tarifa.FechaCreacion = DateTime.UtcNow;
                tarifa.ModifyDate = DateTime.UtcNow;
                tarifa.ModifyUser = 1;

                await _context.Set<Tarifas>().AddAsync(tarifa);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = tarifa
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

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifaPersistence(tarifa);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                var existingTarifa = await _context.Set<Tarifas>().FindAsync(tarifa.Id);
                if (existingTarifa == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingTarifa.Descripcion = tarifa.Descripcion;
                existingTarifa.Estado = tarifa.Estado;
                existingTarifa.PrecioPorNoche = tarifa.PrecioPorNoche;
                existingTarifa.ModifyDate = DateTime.Now;
                existingTarifa.ModifyUser = 1; // Usar ID de usuario real

                _context.Update(existingTarifa);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingTarifa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifaPersistence(tarifa);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            if (tarifa.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingTarifa = await _context.Tarifas.FindAsync(tarifa.Id);
                if (existingTarifa == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.Tarifas.Remove(existingTarifa);
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

        public override async Task<OperationResult> RestoreEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifaPersistence(tarifa);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                var existingTarifa = await _context.Set<Tarifas>()
                                                   .IgnoreQueryFilters()
                                                   .FirstOrDefaultAsync(t => t.Id == tarifa.Id);

                if (existingTarifa == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingTarifa.Deleted = false;
                existingTarifa.ModifyDate = DateTime.Now;
                existingTarifa.ModifyUser = 1; // Usar ID de usuario real

                _context.Update(existingTarifa);
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
    }
}