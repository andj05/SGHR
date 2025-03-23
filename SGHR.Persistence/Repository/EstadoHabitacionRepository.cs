using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Configurations;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repository
{
    public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion>, IEstadoHabitacionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public EstadoHabitacionRepository(SGHRContext context,
                                          ILogger<EstadoHabitacionRepository> logger,
                                          IConfiguration configuration,
                                          MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateEstadoHabitacionPersistence(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
            return new OperationResult { Success = true };
        }

        // Obtener todos los estados de habitación
        public override async Task<List<EstadoHabitacion>> GetAllAsync()
        {
            try
            {
                return await _context.Set<EstadoHabitacion>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["EstadoHabitacion"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                return null;
            }

            try
            {
                var estadoHabitacion = await _context.Set<EstadoHabitacion>()
                                                     .IgnoreQueryFilters() 
                                                     .FirstOrDefaultAsync(e => e.Id == id);

                if (estadoHabitacion == null)
                {
                    _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    return null;
                }
                return estadoHabitacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["EstadoHabitacion"]["GetByIdError"]);
                throw;
            }
        }


        // Verificar existencia
        public override async Task<bool> ExistsAsync(Expression<Func<EstadoHabitacion, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.EstadoHabitacion.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["EstadoHabitacion"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacionPersistence(estadoHabitacion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                estadoHabitacion.FechaCreacion = DateTime.UtcNow;
                estadoHabitacion.ModifyDate = DateTime.UtcNow;
                estadoHabitacion.ModifyUser = 1;

                await _context.Set<EstadoHabitacion>().AddAsync(estadoHabitacion);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = estadoHabitacion
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

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacionPersistence(estadoHabitacion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                var existingEstado = await _context.Set<EstadoHabitacion>().FindAsync(estadoHabitacion.Id);
                if (existingEstado == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingEstado.Descripcion = estadoHabitacion.Descripcion;
                existingEstado.Estado = estadoHabitacion.Estado;
                existingEstado.ModifyDate = DateTime.UtcNow;
                existingEstado.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Update(existingEstado);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingEstado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        // Eliminar entidad
        public override async Task<OperationResult> DeleteEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacionPersistence(estadoHabitacion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            if (estadoHabitacion.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingEstadoHabitacion = await _context.EstadoHabitacion.FindAsync(estadoHabitacion.Id);
                if (existingEstadoHabitacion == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.EstadoHabitacion.Remove(existingEstadoHabitacion);
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

        public override async Task<OperationResult> RestoreEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacionPersistence(estadoHabitacion);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                var existingEstadoHabitacion = await _context.Set<EstadoHabitacion>()
                                                       .IgnoreQueryFilters()
                                                       .FirstOrDefaultAsync(e => e.Id == estadoHabitacion.Id);

                if (existingEstadoHabitacion == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingEstadoHabitacion.Deleted = false;
                existingEstadoHabitacion.ModifyDate = DateTime.UtcNow;
                existingEstadoHabitacion.ModifyUser = 1; 

                _context.Update(existingEstadoHabitacion);
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