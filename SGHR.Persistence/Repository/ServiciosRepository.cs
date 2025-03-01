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

        public async Task<IEnumerable<Servicios>> GetAllAsync()
        {
            try
            {
                return await _context.Servicios
                    .Where(s => s.Estado == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los servicios.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                {
                    result.Success = false;
                    result.Message = "Servicio no encontrado.";
                }
                else
                {
                    result.Success = true;
                    result.Data = servicio;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el servicio con ID {id}.");
                result.Success = false;
                result.Message = $"Error al obtener el servicio: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ExistsAsync(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "El ID del servicio es inválido.";
                return result;
            }

            try
            {
                bool exists = await _context.Servicios.AnyAsync(s => s.Id == id);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del servicio con ID {id}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del servicio: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> SaveEntityAsync(Servicios servicio)
        {
            var result = new OperationResult();
            try
            {
                // Siempre se agregará un nuevo servicio (INSERT)
                _context.Servicios.Add(servicio);

                int affectedRows = await _context.SaveChangesAsync();

                if (affectedRows == 0)
                {
                    return new OperationResult { Success = false, Message = "No se pudo guardar el servicio." };
                }

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

                // Guardar cambios
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



        public async Task<OperationResult> DeleteEntityAsync(int id)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(id);
                if (servicio == null)
                {
                    return new OperationResult { Success = false, Message = "Servicio no encontrado." };
                }

                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Servicio eliminado permanentemente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el servicio.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el servicio: {ex.Message}" };
            }
        }

        private OperationResult ValidateEntity(Servicios servicio)
        {
            if (servicio == null)
            {
                return new OperationResult { Success = false, Message = "El servicio no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Nombre) || servicio.Nombre.Length > 200)
            {
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio y debe tener un máximo de 200 caracteres." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Descripcion))
            {
                return new OperationResult { Success = false, Message = "La descripción del servicio es obligatoria." };
            }

            return new OperationResult { Success = true };
        }
    }
}
