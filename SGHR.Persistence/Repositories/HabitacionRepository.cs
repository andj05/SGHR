using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public HabitacionRepository(
            SGHRContext context,
            ILoggerManager logger,
            MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateHabitacionPersistence(Habitacion habitacion)
        {
            if (habitacion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"]
                };
            }
            return new OperationResult { Success = true };
        }

        public override async Task<List<Habitacion>> GetAllAsync()
        {
            return await _context.Set<Habitacion>().ToListAsync();
        }

        public override async Task<Habitacion> GetEntityByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Set<Habitacion>()
                    .FirstOrDefaultAsync(h => h.Id == id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return null;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Habitacion, bool>> filter)
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

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
        {
            var result = ValidateHabitacionPersistence(habitacion);
            if (result.Success != true)
                return result;

            try
            {
                await base.SaveEntityAsync(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
        {
            var result = ValidateHabitacionPersistence(habitacion);
            if (result.Success != true)
                return result;

            try
            {
                await base.UpdateEntityAsync(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Habitacion habitacion)
        {
            var validationResult = ValidateHabitacionPersistence(habitacion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                habitacion.Deleted = true;
                habitacion.ModifyDate = DateTime.Now;
                await base.UpdateEntityAsync(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"];
            }
            return result;
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Habitacion, bool>> filter)
        {
            try
            {
                return await base.ExistsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return false;
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(int id)
        {
            try
            {
                
                var habitacion = await _context.Set<Habitacion>()
                                               .IgnoreQueryFilters()
                                               .FirstOrDefaultAsync(h => h.Id == id);

                
                var result = await base.RestoreEntityAsync(id);

                
                if (result.Success == true)
                {
                    result.Message = _messageMapper.SuccessMessages["RestoreSuccess"];
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Generic"]["GenericError"]
                };
            }
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorNumeroAsync(string numero)
        {
            try
            {
                return await _context.Habitacion
                    .Where(h => h.Numero == numero)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Habitacion>();
            }
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorPisoIdAsync(int idPiso)
        {
            try
            {
                return await _context.Habitacion
                    .Where(h => h.IdPiso == idPiso)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Habitacion>();
            }
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorCategoriaIdAsync(int idCategoria)
        {
            try
            {
                return await _context.Habitacion
                    .Where(h => h.IdCategoria == idCategoria)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Habitacion>();
            }
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion)
        {
            try
            {
                return await _context.Habitacion
                    .Where(h => h.IdEstadoHabitacion == idEstadoHabitacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Habitacion>();
            }
        }
    }
}