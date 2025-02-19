using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;


namespace SGHR.Persistence.Repository
{
    public class TarifasRepository : BaseRepository<Tarifas>, ITarifasRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<TarifasRepository> _logger;
        private readonly IConfiguration _configuration;

        public TarifasRepository(SGHRContext context,
                                 ILogger<TarifasRepository> logger,
                                 IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync()
        {
            return await _context.Tarifas
                .Where(t => t.FechaFin >= DateOnly.FromDateTime(DateTime.Now))
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarifas?>> ObtenerTarifasPorHabitacionAsync(int idHabitacion)
        {
            return await _context.Tarifas
                .Where(t => t.IdHabitacion == idHabitacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarifas>> ObtenerTarifasPorRangoDeFechasAsync(DateOnly fechaInicio, DateOnly fechaFin)
        {
            return await _context.Tarifas
                .Where(t => t.FechaInicio >= fechaInicio && t.FechaFin <= fechaFin)
                .ToListAsync();
        }

        public async Task<bool> ActualizarEstadoTarifaAsync(int idTarifa, bool estado)
        {
            var tarifa = await _context.Tarifas.FindAsync(idTarifa);
            if (tarifa != null)
            {
                tarifa.FechaFin = estado ? DateOnly.FromDateTime(DateTime.Now).AddMonths(1) : DateOnly.MinValue;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Tarifas?> ObtenerTarifaActualPorHabitacionAsync(int idHabitacion)
        {
            return await _context.Tarifas
                .Where(t => t.IdHabitacion == idHabitacion && t.FechaInicio <= DateOnly.FromDateTime(DateTime.Now) && t.FechaFin >= DateOnly.FromDateTime(DateTime.Now))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin)
        {
            var tarifaExistente = await _context.Tarifas
                .Where(t => t.IdHabitacion == idHabitacion && t.FechaInicio <= fechaFin && t.FechaFin >= fechaInicio)
                .AnyAsync();
            return !tarifaExistente;
        }

        public async Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento)
        {
            var tarifa = await _context.Tarifas.FindAsync(idTarifa);
            if (tarifa != null)
            {
                tarifa.Descuento = nuevoDescuento;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public new async Task<List<OperationResult>> GetAllAsync()
        {
            var tarifas = await base.GetAllAsync();
            return tarifas.Select(t => new OperationResult { Success = true, Data = t }).ToList();
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "La tarifa no puede ser nula." };

            if (entity.PrecioPorNoche <= 0)
                return new OperationResult { Success = false, Message = "El precio por noche debe ser mayor a 0." };

            try
            {
                _context.Tarifas.Add(entity);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Data = entity };
            }
            catch (DbUpdateException ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Ocurrió un error guardando los datos: {Message}", innerExceptionMessage);
                return new OperationResult { Success = false, Message = $"Ocurrió un error guardando los datos: {innerExceptionMessage}" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado: {Message}", ex.Message);
                return new OperationResult { Success = false, Message = $"Ocurrió un error inesperado: {ex.Message}" };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "La tarifa no puede ser nula." };

            if (entity.PrecioPorNoche <= 0)
                return new OperationResult { Success = false, Message = "El precio por noche debe ser mayor a 0." };

            return await base.UpdateEntityAsync(entity);
        }
    }
}
