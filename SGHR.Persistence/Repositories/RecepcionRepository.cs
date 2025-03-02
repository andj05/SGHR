using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Base;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;


namespace SGHR.Persistence.Repositories
{
    public class RecepcionRepository : BaseRepository<Recepcion>, IRecepcionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<RecepcionRepository> _logger;
        private readonly IConfiguration _configuration;

        public RecepcionRepository(SGHRContext context,
                                   ILogger<RecepcionRepository> logger,
                                   IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<List<Recepcion>> GetAllAsync()
        {
            return await _context.Recepcion
                                 .Where(c => c.Estado == true)
                                 .ToListAsync();
        }

        public override async Task<Recepcion> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de recepción inválido.");
                throw new ArgumentException("El ID de recepción debe ser mayor que cero.");
            }

            return await base.GetEntityByIdAsync(id);
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva)
        {
            try
            {
                return await _context.Recepcion
                    .Where(r => r.IdEstadoReserva == idEstadoReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por estado de reserva: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Recepcion, bool>> filter)
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
                _logger.LogError(ex, "Error al obtener recepciones con filtro: {Filter}", filter);
                result.Success = false;
                result.Message = "Error al obtener recepciones con filtro.";
            }
            return result;
        }

        public override async Task<OperationResult> SaveEntityAsync(Recepcion recepcion)
        {
            var validationResult = ValidateRecepcion(recepcion);
            if (!validationResult.Success.HasValue || !validationResult.Success.Value)
            {
                return validationResult;
            }

            return await base.SaveEntityAsync(recepcion);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Recepcion recepcion)
        {
            var validationResult = ValidateRecepcion(recepcion);
            if (!validationResult.Success.HasValue || !validationResult.Success.Value)
            {
                return validationResult;
            }

            return await base.UpdateEntityAsync(recepcion);
        }

        public override async Task<OperationResult> DeleteEntityAsync(Recepcion recepcion)
        {
            var result = new OperationResult();
            try
            {
                if (recepcion.IdEstadoReserva == null || recepcion.IdEstadoReserva == 2)
                {
                    result.Success = false;
                    result.Message = "No se puede eliminar una recepcion nula o en progreso.";
                    return result;
                }
                return await base.DeleteEntityAsync(recepcion);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrió un error eliminando la recepción: {ex.Message}";
            }
            return result;
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente)
        {
            return await _context.Recepcion
                .Where(r => r.IdEstadoReserva == idCliente)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion)
        {
            return await _context.Recepcion
                .Where(r => r.IdEstadoReserva == idHabitacion)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Recepcion
                .Where(r => r.FechaEntrada >= fechaInicio && r.FechaEntrada <= fechaFin)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Recepcion
                .Where(r => r.FechaSalidaConfirmacion >= fechaInicio && r.FechaSalidaConfirmacion <= fechaFin)
                .ToListAsync();
        }
        public override async Task<bool> ExistsAsync(Expression<Func<Recepcion, bool>> filter)
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
        private OperationResult ValidateRecepcion(Recepcion recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult { Success = false, Message = "La recepcion no puede ser nula." };
            }
            if (recepcion.FechaEntrada == default)
            {
                return new OperationResult { Success = false, Message = "La fecha de entrada es obligatoria." };
            }
            if (recepcion.IdCliente.HasValue && recepcion.IdCliente <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del cliente debe ser mayor que cero o nulo." };
            }
            if (recepcion.IdHabitacion.HasValue && recepcion.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la habitacion debe ser mayor que cero o nulo." };
            }
            if (recepcion.IdEstadoReserva.HasValue && recepcion.IdEstadoReserva <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del estado de la reserva debe ser mayor que cero o nulo." };
            }
            if (recepcion.Observacion != null && recepcion.Observacion.Length > 500)
            {
                return new OperationResult { Success = false, Message = "La observacion de la recepcion debe tener un máximo de 500 caracteres." };
            }
            if (recepcion.Deleted == true)
            {
                return new OperationResult { Success = false, Message = "La recepcion ha sido borrada." };
            }
            return new OperationResult { Success = true };
        }
    }
}
