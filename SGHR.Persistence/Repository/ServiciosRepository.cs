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
    public class ServiciosRepository : BaseRepository<Servicios>, IServiciosRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ServiciosRepository> _logger;
        private readonly IConfiguration _configuration;

        public ServiciosRepository(SGHRContext context,
                                   ILogger<ServiciosRepository> logger,
                                   IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<List<Servicios>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Servicios>().Where(h => !h.Deleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los servicios.");
                throw;
            }
        }

        public override async Task<Servicios> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID del servicio inválido: {id}");
                return null;
            }

            try
            {
                var servicio = await _context.Set<Servicios>().FindAsync(id);
                if (servicio == null || servicio.Deleted)
                {
                    _logger.LogWarning($"El servicio con ID {id} no encontrado o eliminado.");
                    return null;
                }
                return servicio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el servicio con ID {id}.");
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Servicios, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.Set<Servicios>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del servicio.");
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Servicios servicio)
        {
            var validation = ValidateServicios(servicio);
            if (!validation.Success !=null)
                return validation;

            var result = new OperationResult();
            try
            {
                servicio.FechaCreacion = DateTime.Now;
                servicio.ModifyDate = DateTime.Now;
                servicio.ModifyUser = 1;

                await _context.Set<Servicios>().AddAsync(servicio);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = servicio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el servicio.");
                result.Success = false;
                result.Message = $"Error al guardar el servicio: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Servicios servicio)
        {
            var result = new OperationResult();
            try
            {
                // Validación del servicio
                var validationResult = ValidateServicios(servicio);
                if (!validationResult.Success != null)
                {
                    return validationResult;
                }

                // Buscar el servicio existente en la base de datos
                var existingServicio = await _context.Set<Servicios>().FindAsync(servicio.Id);
                if (existingServicio == null)
                {
                    return new OperationResult { Success = false, Message = "Servicio no encontrado." };
                }

                // Actualizar los datos modificables
                existingServicio.Nombre = servicio.Nombre;
                existingServicio.Descripcion = servicio.Descripcion;
                existingServicio.Estado = servicio.Estado;
                existingServicio.ModifyDate = DateTime.Now;
                existingServicio.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                // Guardar los cambios
                _context.Entry(existingServicio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingServicio;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el servicio.");
                result.Success = false;
                result.Message = $"Error al actualizar el servicio: {ex.Message}";
            }
            return result;
        }


        public override async Task<OperationResult> DeleteEntityAsync(Servicios servicio)
        {
            if (servicio == null || servicio.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID del servicio es inválido." };

            try
            {
                var existingServicio = await _context.Set<Servicios>().FindAsync(servicio.Id);
                if (existingServicio == null)
                    return new OperationResult { Success = false, Message = "Servicio no encontrado." };

                _context.Set<Servicios>().Remove(existingServicio);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Servicio eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el servicio con ID {servicio.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el servicio: {ex.Message}" };
            }
        }

        private OperationResult ValidateServicios(Servicios servicio)
        {
            if (servicio == null)
            {
                return new OperationResult { Success = false, Message = "El servicio no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Nombre) || servicio.Nombre.Length > 100)
            {
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio y debe tener un máximo de 100 caracteres." };
            }

            if (servicio.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
