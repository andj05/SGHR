using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repository
{
    public class EstadoHabitacionRepository : BaseRepository<EstadoHabitacion>, IEstadoHabitacionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IConfiguration _configuration;

        public EstadoHabitacionRepository(SGHRContext context,
                                          ILogger<EstadoHabitacionRepository> logger,
                                          IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // Obtener todos los estados de habitación
        public override async Task<List<EstadoHabitacion>> GetAllAsync()
        {
            try
            {
                return await _context.Set<EstadoHabitacion>().Where(h => !h.Deleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Estado de La Habitacion.");
                throw;
            }
        }

        // Obtener entidad por ID
        public override async Task<EstadoHabitacion> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID del estado de habitación inválido: {id}");
                return null;
            }

            try
            {
                var estadoHabitacion = await _context.EstadoHabitacion.FindAsync(id);
                if (estadoHabitacion == null || estadoHabitacion.Estado == false)
                {
                    _logger.LogWarning($"El estado de habitación con ID {id} no encontrado o está desactivado.");
                    return null;
                }
                return estadoHabitacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el estado de habitación con ID {id}.");
                throw;
            }
        }

        // Verificar existencia
        public override async Task<bool> ExistsAsync(Expression<Func<EstadoHabitacion, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.EstadoHabitacion.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del estado de habitación.");
                throw;
            }
        }

        // Guardar entidad
        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validation = ValidateEstadoHabitacion(estadoHabitacion);
            if (!validation.Success != null)
                return validation;

            var result = new OperationResult();
            try
            {
                estadoHabitacion.FechaCreacion = DateTime.Now;
                estadoHabitacion.ModifyDate = DateTime.Now;
                estadoHabitacion.ModifyUser = 1;

                await _context.Set<EstadoHabitacion>().AddAsync(estadoHabitacion);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = estadoHabitacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el Estado Habitacion.");
                result.Success = false;
                result.Message = $"Error al guardar el Estado Habitacion: {ex.Message}";
            }
            return result;
        }

        // Actualizar entidad
        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var result = new OperationResult();
            try
            {
                var validationResult = ValidateEstadoHabitacion(estadoHabitacion);
                if (!validationResult.Success !=null)
                {
                    return validationResult;
                }

                var existingEstado = await _context.EstadoHabitacion.FindAsync(estadoHabitacion.Id);
                if (existingEstado == null || existingEstado.Estado == false)
                {
                    return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ha sido eliminado." };
                }

                existingEstado.Descripcion = estadoHabitacion.Descripcion ?? existingEstado.Descripcion;
                existingEstado.Estado = estadoHabitacion.Estado;
                existingEstado.ModifyDate = DateTime.Now;
                existingEstado.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Entry(existingEstado).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingEstado;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de habitación.");
                result.Success = false;
                result.Message = $"Error al actualizar el estado de habitación: {ex.Message}";
            }
            return result;
        }

        // Eliminar entidad
        public override async Task<OperationResult> DeleteEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null || estadoHabitacion.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID del Esatado Habitacion es inválido." };

            try
            {
                var existingEstadoHabitacion = await _context.Set<Piso>().FindAsync(estadoHabitacion.Id);
                if (existingEstadoHabitacion == null)
                    return new OperationResult { Success = false, Message = "Esatado Habitacion  no encontrado." };

                _context.Set<Piso>().Remove(existingEstadoHabitacion);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Esatado Habitacion eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el Estado Habitacion con ID {estadoHabitacion.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el piso: {ex.Message}" };
            }
        }

        // Validación del estado de habitación
        private OperationResult ValidateEstadoHabitacion(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
            {
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };
            }

            if (estadoHabitacion.Descripcion != null && estadoHabitacion.Descripcion.Length > 50)
            {
                return new OperationResult { Success = false, Message = "La descripción del estado de habitación debe tener un máximo de 50 caracteres." };
            }

            if (estadoHabitacion.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
