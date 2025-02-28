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
                    .Where(s => s.Estado)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los servicios.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int idServicio)
        {
            var result = new OperationResult();
            try
            {
                var servicio = await _context.Servicios.FindAsync(idServicio);
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
                _logger.LogError(ex, $"Error al obtener el servicio con ID {idServicio}.");
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


        public override async Task<OperationResult> SaveEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateEntity(servicio);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                await _context.Servicios.AddAsync(servicio);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Servicio guardado correctamente.", Data = servicio };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el servicio.");
                return new OperationResult { Success = false, Message = $"Error al guardar el servicio: {ex.Message}" };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateEntity(servicio);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                var existingServicio = await _context.Servicios.FindAsync(servicio.Id);
                if (existingServicio == null)
                {
                    return new OperationResult { Success = false, Message = "Servicio no encontrado." };
                }

                existingServicio.Nombre = servicio.Nombre ?? existingServicio.Nombre;
                existingServicio.Descripcion = servicio.Descripcion ?? existingServicio.Descripcion;
                existingServicio.Estado = servicio.Estado;
                existingServicio.ModifyDate = DateTime.Now;
                existingServicio.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Servicios.Update(existingServicio);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Servicio actualizado correctamente.", Data = existingServicio };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el servicio.");
                return new OperationResult { Success = false, Message = $"Error al actualizar el servicio: {ex.Message}" };
            }
        }

        public async Task<OperationResult> DeleteEntityAsync(int idServicio)
        {
            try
            {
                var servicio = await _context.Servicios.FindAsync(idServicio);
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

            if (string.IsNullOrWhiteSpace(servicio.Nombre) || servicio.Nombre.Length > 50)
            {
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio y debe tener un máximo de 50 caracteres." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Descripcion) || servicio.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del servicio es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}