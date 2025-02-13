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

        public async Task<List<Recepcion>> ObtenerTodasLasRecepcionesAsync()
        {
            try
            {
                return await _context.Recepciones.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todas las recepciones: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<Recepcion> ObtenerRecepcionPorIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de recepción inválido.");
                throw new ArgumentException("El ID de recepción debe ser mayor que cero.");
            }

            var recepcion = await _context.Recepciones.FindAsync(id);
            if (recepcion == null)
            {
                _logger.LogWarning($"Recepción con ID {id} no encontrada.");
            }
            return recepcion;
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva)
        {
            try
            {
                return await _context.Recepciones
                    .Where(r => r.IdEstadoReserva == idEstadoReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por estado de reserva: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFilterAsync(Expression<Func<Recepcion, bool>> filter)
        {
            try
            {
                return await _context.Recepciones.Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones con filtro: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<OperationResult> GuardarRecepcionAsync(Recepcion recepcion)
        {
            var result = new OperationResult();
            try
            {
                if (recepcion == null)
                {
                    result.Success = false;
                    result.Message = "La recepción no puede ser nula.";
                    return result;
                }

                if (recepcion.FechaEntrada == default)
                {
                    result.Success = false;
                    result.Message = "La fecha de entrada es obligatoria.";
                    return result;
                }

                await _context.Recepciones.AddAsync(recepcion);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar recepción: {ex.Message}");
                result.Success = false;
                result.Message = "Error al guardar la recepción.";
            }
            return result;
        }

        public async Task<OperationResult> ActualizarRecepcionAsync(Recepcion recepcion)
        {
            var result = new OperationResult();
            try
            {
                if (recepcion == null || recepcion.IdRecepcion <= 0)
                {
                    result.Success = false;
                    result.Message = "Datos de recepción inválidos.";
                    return result;
                }

                _context.Recepciones.Update(recepcion);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar recepción: {ex.Message}");
                result.Success = false;
                result.Message = "Error al actualizar la recepción.";
            }
            return result;
        }

        public async Task<OperationResult> BorrarRecepcionAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _context.Recepciones.FindAsync(id);
                if (recepcion == null)
                {
                    result.Success = false;
                    result.Message = "Recepción no encontrada.";
                    return result;
                }

                _context.Recepciones.Remove(recepcion);
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

        public async Task<bool> ExisteRecepcionAsync(int id)
        {
            return await _context.Recepciones.AnyAsync(r => r.IdRecepcion == id);
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente)
        {
            return await _context.Recepciones
                .Where(r => r.IdEstadoReserva == idCliente)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion)
        {
            return await _context.Recepciones
                .Where(r => r.IdEstadoReserva == idHabitacion)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Recepciones
                .Where(r => r.FechaEntrada >= fechaInicio && r.FechaEntrada <= fechaFin)
                .ToListAsync();
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Recepciones
                .Where(r => r.FechaSalidaConfirmacion >= fechaInicio && r.FechaSalidaConfirmacion <= fechaFin)
                .ToListAsync();
        }
    }
}
