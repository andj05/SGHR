using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using System.Linq.Expressions;

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

        public override async Task<List<Tarifas>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Tarifas>().Where(h => !h.Deleted).ToListAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las tarifas.");
                throw;
            }
        }

        public override async Task<Tarifas> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID de la Tarifa inválido: {id}");
                return null;
            }

            try
            {
                var usuario = await _context.Set<Tarifas>().FindAsync(id);
                if (usuario == null || usuario.Deleted)
                {
                    _logger.LogWarning($"La Tarifa con id {id} no encontrado o eliminado.");
                    return null;
                }
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener De la Tarifa con id {id}.");
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Tarifas, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.Set<Tarifas>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia de la tarifa.");
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
        {
            var validation = ValidateTarifas(tarifas);
            if (validation.Success != true)
                return validation;

            var result = new OperationResult();
            try
            {
               
                tarifas.ModifyDate = DateTime.Now;
                tarifas.ModifyUser = 1; 

                await _context.Set<Tarifas>().AddAsync(tarifas);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = tarifas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al Guardar al tarifa.");
                result.Success = false;
                result.Message = $"Error al Guardar al tarifa: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifa)
        {
            var result = new OperationResult();
            try
            {
                var existingTarifa = await _context.Set<Tarifas>().FindAsync(tarifa.Id);
                if (existingTarifa == null)
                {
                    return new OperationResult { Success = false, Message = "Tarifa no encontrada." };
                }

                var validationResult = ValidateTarifas(tarifa);
                if (!validationResult.Success != null)
                {
                    return validationResult;
                }

                // Actualizar los datos modificables
                existingTarifa.FechaInicio = tarifa.FechaInicio;
                existingTarifa.FechaFin = tarifa.FechaFin;
                existingTarifa.PrecioPorNoche = tarifa.PrecioPorNoche;
                existingTarifa.Descuento = tarifa.Descuento;
                existingTarifa.Descripcion = tarifa.Descripcion ?? existingTarifa.Descripcion;
                existingTarifa.IdHabitacion = tarifa.IdHabitacion;
                existingTarifa.Estado = tarifa.Estado;
                existingTarifa.Deleted = tarifa.Deleted;
                existingTarifa.DeletedUser = tarifa.DeletedUser;
                existingTarifa.ModifyDate = tarifa.ModifyDate;
                existingTarifa.ModifyUser = tarifa.ModifyUser;

                // Guardar cambios
                _context.Update(existingTarifa);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingTarifa;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la tarifa.");
                result.Success = false;
                result.Message = $"Error al actualizar la tarifa: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Tarifas tarifas)
        {
            if (tarifas == null || tarifas.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID de la Tarifa es inválido." };

            try
            {
                var existingTarifas= await _context.Set<Tarifas>().FindAsync(tarifas.Id);
                if (existingTarifas == null)
                    return new OperationResult { Success = false, Message = "Tarifa no encontrado." };

                _context.Set<Tarifas>().Remove(existingTarifas);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Tarifa eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la Tarifa con id {tarifas.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar la tarifa: {ex.Message}" };
            }
        }

        private OperationResult ValidateTarifas(Tarifas tarifa)
        {
            if (tarifa == null)
            {
                return new OperationResult { Success = false, Message = "La tarifa no puede ser nula." };
            }

            if (tarifa.PrecioPorNoche <= 0)
            {
                return new OperationResult { Success = false, Message = "El precio por noche debe ser mayor a 0." };
            }

            if (string.IsNullOrWhiteSpace(tarifa.Descripcion) || tarifa.Descripcion.Length > 255)
            {
                return new OperationResult { Success = false, Message = "La descripción de la tarifa es obligatoria y debe tener un máximo de 255 caracteres." };
            }

            if (tarifa.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la habitación debe ser mayor que cero." };
            }

            if (tarifa.FechaInicio == default || tarifa.FechaFin == default)
            {
                return new OperationResult { Success = false, Message = "Las fechas de inicio y fin son obligatorias." };
            }

            if (tarifa.Descuento < 0 || tarifa.Descuento > 100)
            {
                return new OperationResult { Success = false, Message = "El descuento debe estar entre 0 y 100." };
            }

            if (tarifa.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
