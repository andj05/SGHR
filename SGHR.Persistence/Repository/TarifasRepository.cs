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

        public async Task<IEnumerable<Tarifas>> GetAllAsync()
        {
            try
            {
                return await _context.Tarifas
                    .Where(t => t.Estado)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las tarifas.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int idTarifa)
        {
            var result = new OperationResult();
            try
            {
                var tarifa = await _context.Tarifas.FindAsync(idTarifa);
                if (tarifa == null)
                {
                    result.Success = false;
                    result.Message = "Tarifa no encontrada.";
                }
                else
                {
                    result.Success = true;
                    result.Data = tarifa;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la tarifa con ID {idTarifa}.");
                result.Success = false;
                result.Message = $"Error al obtener la tarifa: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ExistsAsync(int idTarifa)
        {
            var result = new OperationResult();
            if (idTarifa <= 0)
            {
                result.Success = false;
                result.Message = "El ID de la tarifa es inválido.";
                return result;
            }
            try
            {
                bool exists = await _context.Tarifas.AnyAsync(t => t.Id == idTarifa);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia de la tarifa con ID {idTarifa}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia de la tarifa: {ex.Message}";
            }
            return result;
        }


        public async Task<OperationResult> AplicarDescuentoTarifaEntityAsync(int idTarifa, decimal nuevoDescuento)
        {
            var result = new OperationResult();
            try
            {
                var tarifa = await _context.Tarifas.FindAsync(idTarifa);
                if (tarifa == null)
                {
                    return new OperationResult { Success = false, Message = "Tarifa no encontrada." };
                }

                tarifa.Descuento = nuevoDescuento;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Descuento aplicado correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al aplicar el descuento en la tarifa con ID {idTarifa}.");
                result.Success = false;
                result.Message = $"Error al aplicar el descuento: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifas(tarifa);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                await _context.Tarifas.AddAsync(tarifa);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Tarifa guardada correctamente.", Data = tarifa };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la tarifa.");
                return new OperationResult { Success = false, Message = $"Error al guardar la tarifa: {ex.Message}" };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifa)
        {
            var validationResult = ValidateTarifas(tarifa);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                var existingTarifa = await _context.Tarifas.FindAsync(tarifa.Id);
                if (existingTarifa == null)
                {
                    return new OperationResult { Success = false, Message = "Tarifa no encontrada." };
                }

                existingTarifa.FechaInicio = tarifa.FechaInicio;
                existingTarifa.FechaFin = tarifa.FechaFin;
                existingTarifa.PrecioPorNoche = tarifa.PrecioPorNoche;
                existingTarifa.Descuento = tarifa.Descuento;
                existingTarifa.Descripcion = tarifa.Descripcion;
                existingTarifa.IdHabitacion = tarifa.IdHabitacion;
                existingTarifa.Estado = tarifa.Estado;
                existingTarifa.ModifyDate = DateTime.Now;
                existingTarifa.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Tarifas.Update(existingTarifa);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Tarifa actualizada correctamente.", Data = existingTarifa };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la tarifa.");
                return new OperationResult { Success = false, Message = $"Error al actualizar la tarifa: {ex.Message}" };
            }
        }

        public async Task<OperationResult> DeleteEntityAsync(int idTarifa)
        {
            try
            {
                var tarifa = await _context.Tarifas.FindAsync(idTarifa);
                if (tarifa == null)
                {
                    return new OperationResult { Success = false, Message = "Tarifa no encontrada." };
                }

                _context.Tarifas.Remove(tarifa);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Tarifa eliminada permanentemente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa.");
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

            if (string.IsNullOrWhiteSpace(tarifa.Descripcion) || tarifa.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción de la tarifa es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            if (tarifa.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la habitación debe ser mayor que cero." };
            }

            if (tarifa.FechaInicio == default || tarifa.FechaFin == default)
            {
                return new OperationResult { Success = false, Message = "Las fechas de inicio y fin son obligatorias." };
            }

            return new OperationResult { Success = true };
        }
    }
}