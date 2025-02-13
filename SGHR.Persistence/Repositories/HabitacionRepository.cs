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

        public async Task<List<Habitacion>> ObtenerTodasLasHabitacionesAsync()
        {
            try
            {
                return await _context.Habitaciones.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todas las habitaciones: {ex.Message}");
                return new List<Habitacion>();
            }
        }

        public async Task<Habitacion> ObtenerHabitacionPorIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de habitación inválido.");
                throw new ArgumentException("El ID de habitación debe ser mayor que cero.");
            }

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
            {
                _logger.LogWarning($"Habitación con ID {id} no encontrada.");
            }
            return habitacion;
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion)
        {
            try
            {
                return await _context.Habitaciones
                    .Where(h => h.IdEstadoHabitacion == idEstadoHabitacion)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener habitaciones por estado: {ex.Message}");
                return new List<Habitacion>();
            }
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorFilterAsync(Expression<Func<Habitacion, bool>> filter)
        {
            try
            {
                return await _context.Habitaciones.Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener habitaciones con filtro: {ex.Message}");
                return new List<Habitacion>();
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
        {
            var result = new OperationResult();
            try
            {
                if (habitacion == null)
                {
                    result.Success = false;
                    result.Message = "La habitación no puede ser nula.";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(habitacion.Numero))
                {
                    result.Success = false;
                    result.Message = "El número de la habitación es obligatorio.";
                    return result;
                }

                await _context.Habitaciones.AddAsync(habitacion);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar habitación: {ex.Message}");
                result.Success = false;
                result.Message = "Error al guardar la habitación.";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
        {
            var result = new OperationResult();
            try
            {
                if (habitacion == null || habitacion.IdHabitacion <= 0)
                {
                    result.Success = false;
                    result.Message = "Datos de habitación inválidos.";
                    return result;
                }

                _context.Habitaciones.Update(habitacion);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar habitación: {ex.Message}");
                result.Success = false;
                result.Message = "Error al actualizar la habitación.";
            }
            return result;
        }

        public async Task<bool> ExisteHabitacionAsync(int id)
        {
            return await _context.Habitaciones.AnyAsync(h => h.IdHabitacion == id);
        }

        public async Task<OperationResult> GuardarHabitacionAsync(Habitacion habitacion)
        {
            return await SaveEntityAsync(habitacion);
        }

        public async Task<OperationResult> ActualizarHabitacionAsync(Habitacion habitacion)
        {
            return await UpdateEntityAsync(habitacion);
        }
    }
}
