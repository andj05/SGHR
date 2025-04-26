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
    public class ServiciosRepository : BaseRepository<Servicios>, IServiciosRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ServiciosRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public ServiciosRepository(SGHRContext context,
                                   ILogger<ServiciosRepository> logger,
                                   IConfiguration configuration,
                                   MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateServicioPersistence(Servicios servicio)
        {
            if (servicio == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
            return new OperationResult { Success = true };
        }

        public override async Task<List<Servicios>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Servicios>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Servicios"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<Servicios> GetEntityByIdAsync(int id)
        {
            try
            {
                var servicio = await _context.Set<Servicios>()
                                             .FirstOrDefaultAsync(s => s.Id == id);

                return servicio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return null;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Servicios, bool>> filter)
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

        public override async Task<bool> ExistsAsync(Expression<Func<Servicios, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.Set<Servicios>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Servicios"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicioPersistence(servicio);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                servicio.FechaCreacion = DateTime.UtcNow;
                servicio.ModifyDate = DateTime.UtcNow;
                servicio.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                await _context.Set<Servicios>().AddAsync(servicio);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = servicio
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

        public override async Task<OperationResult> UpdateEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicioPersistence(servicio);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                var existingServicio = await _context.Set<Servicios>().FindAsync(servicio.Id);
                if (existingServicio == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingServicio.Nombre = servicio.Nombre;
                existingServicio.Descripcion = servicio.Descripcion;
                existingServicio.Estado = servicio.Estado;
                existingServicio.ModifyDate = DateTime.UtcNow;
                existingServicio.ModifyUser = 1;

                _context.Update(existingServicio);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingServicio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicioPersistence(servicio);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            if (servicio.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingServicio = await _context.Servicios.FindAsync(servicio.Id);
                if (existingServicio == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.Servicios.Remove(existingServicio);
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

        public override async Task<OperationResult> RestoreEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicioPersistence(servicio);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                // Primero buscamos el servicio ignorando los filtros (para poder encontrar los eliminados)
                var existingServicio = await _context.Set<Servicios>()
                                                  .IgnoreQueryFilters()
                                                  .FirstOrDefaultAsync(s => s.Id == servicio.Id);

                if (existingServicio == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingServicio.Deleted = false;
                existingServicio.ModifyDate = DateTime.UtcNow;
                existingServicio.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Update(existingServicio);
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