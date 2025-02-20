using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<HabitacionRepository> _logger;
        private readonly IConfiguration _configuration;

        public HabitacionRepository(SGHRContext context,
                                    ILogger<HabitacionRepository> logger,
                                    IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        
        public override async Task<List<Habitacion>> GetAllAsync()
        {
            return await _context.Habitacion
                                 .Where(c => c.Estado == true)
                                 .ToListAsync();
        }

        public override async Task<Habitacion> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de habitación inválido.");
                throw new ArgumentException("El ID de habitación debe ser mayor que cero.");
            }

            return await base.GetEntityByIdAsync(id);
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
                _logger.LogError($"Error al obtener habitaciones por estado: {ex.Message}");
                return new List<Habitacion>();
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Habitacion, bool>> filter)
        {
            var result = new OperationResult();
            try
            {
                if (filter == null)
                {
                    _logger.LogWarning("El filtro no puede ser nulo.");
                    result.Success = false;
                    result.Message = "El filtro no puede ser nulo.";
                    return result;
                }

                _logger.LogInformation("Aplicando filtro: {Filter}", filter);

                var filteredEntities = await base.GetFilteredAsync(filter);
                result.Success = true;
                result.Data = filteredEntities.Data; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener habitaciones con filtro: {Filter}", filter);
                result.Success = false;
                result.Message = "Error al obtener habitaciones con filtro.";
            }
            return result;
        }

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
        {
            if (habitacion == null)
                return new OperationResult { Success = false, Message = "La habitacion no puede ser nula." };
            if (string.IsNullOrWhiteSpace(habitacion.Detalle))
                return new OperationResult { Success = false, Message = "El detalle de la habitacion no puede estar vacío." };

            return await base.SaveEntityAsync(habitacion);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
        {
            if (habitacion == null)
                return new OperationResult { Success = false, Message = "La habitacion no puede ser nula." };
            if (string.IsNullOrWhiteSpace(habitacion.Detalle))
                return new OperationResult { Success = false, Message = "El detalle de la habitacion no puede estar vacío." };

            return await base.UpdateEntityAsync(habitacion);
        }
        public override async Task<bool> ExistsAsync(Expression<Func<Habitacion, bool>> filter)
        {
            try
            {
                if (filter == null)
                {
                    _logger.LogWarning("El filtro no puede ser nulo.");
                    throw new ArgumentException("El filtro no puede ser nulo.");
                }

                _logger.LogInformation("Verificando existencia con filtro: {Filter}", filter);

                return await base.ExistsAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia con filtro: {Filter}", filter);
                throw;
            }
        }
    }
}
