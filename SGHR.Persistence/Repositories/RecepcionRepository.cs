using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Base;
using SGHR.Persistence.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Persistence.Repositories
{
    public class RecepcionRepository : BaseRepository<Recepcion>, IRecepcionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public RecepcionRepository( SGHRContext context,
                                    ILoggerManager logger,
                                    MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // confirma que la entindad no sea nula
        private OperationResult ValidateRecepcionPersistence(Recepcion recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"]
                };
            }
            return new OperationResult { Success = true };
        }

        public override async Task<List<Recepcion>> GetAllAsync()
        {
            return await _context.Set<Recepcion>().ToListAsync();
        }

        public override async Task<Recepcion> GetEntityByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Set<Recepcion>()
                    .FirstOrDefaultAsync(r => r.Id == id);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return null;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Recepcion, bool>> filter)
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

        public override async Task<OperationResult> SaveEntityAsync(Recepcion recepcion)
        {
            var result = ValidateRecepcionPersistence(recepcion);
            if (result.Success != true)
                return result;

            try
            {
                await base.SaveEntityAsync(recepcion);
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

        public override async Task<OperationResult> UpdateEntityAsync(Recepcion recepcion)
        {
            var result = ValidateRecepcionPersistence(recepcion);
            if (result.Success != true)
                return result;

            try
            {
                await base.UpdateEntityAsync(recepcion);
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

        public override async Task<OperationResult> DeleteEntityAsync(Recepcion recepcion)
        {
            var validationResult = ValidateRecepcionPersistence(recepcion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                recepcion.Deleted = true;
                recepcion.ModifyDate = DateTime.Now;
                await base.UpdateEntityAsync(recepcion);
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

        public override async Task<bool> ExistsAsync(Expression<Func<Recepcion, bool>> filter)
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
                var recepcion = await _context.Set<Recepcion>()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(r => r.Id == id);

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


        public async Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente)
        {
            try
            {
                return await _context.Set<Recepcion>()
                    .Where(r => r.IdCliente.HasValue && r.IdCliente == idCliente)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion)
        {
            try
            {
                return await _context.Set<Recepcion>()
                    .Where(r => r.IdHabitacion.HasValue && r.IdHabitacion == idHabitacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva)
        {
            try
            {
                return await _context.Set<Recepcion>()
                    .Where(r => r.IdEstadoReserva.HasValue && r.IdEstadoReserva == idEstadoReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorPrecioInicialAsync(decimal precioInicial)
        {
            try
            {
                return await _context.Set<Recepcion>()
                    .Where(r => r.PrecioInicial.HasValue &&
                                Math.Round(r.PrecioInicial.Value, 2) == Math.Round(precioInicial, 2))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }
    }
}
