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
            if (recepcion == null)
                return new OperationResult { Success = false, Message = "La recepcion no puede ser nula." };
            if (string.IsNullOrWhiteSpace(recepcion.Observacion))
                return new OperationResult { Success = false, Message = "La observacion de la recepcion no puede estar vacía." };

            return await base.SaveEntityAsync(recepcion);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Recepcion recepcion)
        {
            if (recepcion == null)
                return new OperationResult { Success = false, Message = "La recepcion no puede ser nula." };
            if (string.IsNullOrWhiteSpace(recepcion.Observacion))
                return new OperationResult { Success = false, Message = "La observacion de la recepcion no puede estar vacía." };

            return await base.UpdateEntityAsync(recepcion);
        }

        public async Task<OperationResult> BorrarRecepcionAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _context.Recepcion.FindAsync(id);
                if (recepcion == null)
                {
                    result.Success = false;
                    result.Message = "Recepción no encontrada.";
                    return result;
                }

                _context.Recepcion.Remove(recepcion);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar recepción: {ex.Message}");
                result.Success = false;
                result.Message = "Error al eliminar la recepción.";
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
    }
}
