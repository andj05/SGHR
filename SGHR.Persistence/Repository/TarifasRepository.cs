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

        public async Task<OperationResult> SaveEntityAsync(int idtarifas)
        {
            var tarifas = await _context.Set<Tarifas>().FindAsync(idtarifas);
            if (tarifas == null)
            {
                return new OperationResult { Success = false, Message = "El rol de usuario no fue encontrado." };
            }
            return await SaveEntityAsync(tarifas);
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


        public async Task<OperationResult> DeleteEntityAsync(int id)
        {
            try
            {
                var tarifas = await _context.Tarifas.FindAsync(id);
                if (tarifas == null)
                {
                    return new OperationResult { Success = false, Message = "Rol de usuario no encontrado." };
                }

                _context.Tarifas.Remove(tarifas);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Rol de usuario eliminado permanentemente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol de usuario.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el rol de usuario: {ex.Message}" };
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