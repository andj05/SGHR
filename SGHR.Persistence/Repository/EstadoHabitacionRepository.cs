using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;

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

        // Obtener todos los estados de habitación
        public async Task<IEnumerable<EstadoHabitacion>> GetAllAsync()
        {
            try
            {
                return await _context.EstadoHabitacion
                    .Where(e => e.Estado == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los estados de habitación.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var estadoHabitacion = await _context.EstadoHabitacion.FindAsync(id);
                if (estadoHabitacion == null)
                {
                    result.Success = false;
                    result.Message = "Estado de habitación no encontrado.";
                }
                else
                {
                    result.Success = true;
                    result.Data = estadoHabitacion;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el estado de habitación con ID {id}.");
                result.Success = false;
                result.Message = $"Error al obtener el estado de habitación: {ex.Message}";
            }
            return result;
        }
       

        // Verificar si un estado de habitación existe
        public async Task<OperationResult> ExistsAsync(int IdEstadoHabitacion)
        {
            var result = new OperationResult();
            if (IdEstadoHabitacion <= 0)
            {
                result.Success = false;
                result.Message = "El ID del estado de habitación es inválido.";
                return result;
            }

            try
            {
                bool exists = await _context.EstadoHabitacion
                    .AnyAsync(e => e.Id == IdEstadoHabitacion);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del estado de habitación con ID {IdEstadoHabitacion}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del estado de habitación: {ex.Message}";
            }

            return result;
        }

        // Guardar un estado de habitación
        public async Task<OperationResult> SaveEntityAsync(int idestadoHabitacion)
        {
            var estadoHabitacion = await _context.Set<EstadoHabitacion>().FindAsync(idestadoHabitacion);
            if (estadoHabitacion == null)
            {
                return new OperationResult { Success = false, Message = "La categoria no fue encontrado." };
            }
            return await SaveEntityAsync(estadoHabitacion);
        }

        // Actualizar un estado de habitación
        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var result = new OperationResult();
            try
            {
                // Buscar el Estado de Habitación por su ID
                var existingEstado = await _context.Set<EstadoHabitacion>().FindAsync(estadoHabitacion.Id);

                // Verificar si no se encuentra el Estado de Habitación
                if (existingEstado == null)
                {
                    return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ha sido eliminado." };
                }

                // Actualizar los datos modificables
                existingEstado.Descripcion = estadoHabitacion.Descripcion ?? existingEstado.Descripcion;
                existingEstado.Estado = estadoHabitacion.Estado;
                existingEstado.ModifyDate = DateTime.Now;
                existingEstado.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                // Guardar los cambios en la base de datos
                _context.Update(existingEstado);
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

        // Eliminar un estado de habitación permanentemente
        public async Task<OperationResult> DeleteEntityAsync(int IdEstadoHabitacion)
        {
            try
            {
                var estadoHabitacion = await _context.EstadoHabitacion.FindAsync(IdEstadoHabitacion);
                if (estadoHabitacion == null)
                {
                    return new OperationResult { Success = false, Message = "Estado de habitación no encontrado." };
                }

                _context.EstadoHabitacion.Remove(estadoHabitacion);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Estado de habitación eliminado correctamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado de habitación.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el estado de habitación: {ex.Message}" };
            }
        }

        // Validar estado de habitación antes de guardarlo o actualizarlo
        private OperationResult ValidateEstadoHabitacion(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
            {
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(estadoHabitacion.Descripcion) || estadoHabitacion.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del estado de habitación es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}