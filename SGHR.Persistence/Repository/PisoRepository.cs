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
    public class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<PisoRepository> _logger;
        private readonly IConfiguration _configuration;

        public PisoRepository(SGHRContext context,
                              ILogger<PisoRepository> logger,
                              IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<List<Piso>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Piso>().Where(h => !h.Deleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pisos.");
                throw;
            }
        }

        public override async Task<Piso> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID del piso inválido: {id}");
                return null;
            }

            try
            {
                var piso = await _context.Set<Piso>().FindAsync(id);
                if (piso == null || piso.Deleted)
                {
                    _logger.LogWarning($"El piso con ID {id} no encontrado o eliminado.");
                    return null;
                }
                return piso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el piso con ID {id}.");
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Piso, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.Set<Piso>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del piso.");
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Piso piso)
        {
            var validation = ValidatePiso(piso);
            if (!validation.Success != null)
                return validation;

            var result = new OperationResult();
            try
            {
                piso.FechaCreacion = DateTime.Now;
                piso.ModifyDate = DateTime.Now;
                piso.ModifyUser = 1;

                await _context.Set<Piso>().AddAsync(piso);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = piso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el piso.");
                result.Success = false;
                result.Message = $"Error al guardar el piso: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
        {
            var result = new OperationResult();
            try
            {
                // Validación del piso
                var validationResult = ValidatePiso(piso);
                if (!validationResult.Success != null)
                {
                    return validationResult;
                }

                // Buscar el piso existente en la base de datos
                var existingPiso = await _context.Set<Piso>().FindAsync(piso.Id);
                if (existingPiso == null)
                {
                    return new OperationResult { Success = false, Message = "Piso no encontrado." };
                }

                // Actualizar los datos modificables
                existingPiso.Descripcion = piso.Descripcion ?? existingPiso.Descripcion;
                existingPiso.Estado = piso.Estado;
                existingPiso.ModifyDate = DateTime.Now;
                existingPiso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                // Guardar los cambios
                _context.Entry(existingPiso).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingPiso;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el piso.");
                result.Success = false;
                result.Message = $"Error al actualizar el piso: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Piso piso)
        {
            if (piso == null || piso.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID del piso es inválido." };

            try
            {
                var existingPiso = await _context.Set<Piso>().FindAsync(piso.Id);
                if (existingPiso == null)
                    return new OperationResult { Success = false, Message = "Piso no encontrado." };

                _context.Set<Piso>().Remove(existingPiso);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Piso eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el piso con ID {piso.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el piso: {ex.Message}" };
            }
        }

        private OperationResult ValidatePiso(Piso piso)
        {
            if (piso == null)
            {
                return new OperationResult { Success = false, Message = "El piso no puede ser nulo." };
            }

            if (piso.Descripcion != null && piso.Descripcion.Length > 50)
            {
                return new OperationResult { Success = false, Message = "La descripción del piso debe tener un máximo de 50 caracteres." };
            }

            if (piso.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
