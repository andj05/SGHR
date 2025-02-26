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

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync()
        {
            return await _context.Tarifas
                .Where(t => t.Estado == true)
                .ToListAsync();
        }

        public async Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin)
        {
            var tarifaExistente = await _context.Tarifas
                .Where(t => t.IdHabitacion == idHabitacion && t.FechaInicio <= fechaFin && t.FechaFin >= fechaInicio)
                .AnyAsync();
            return !tarifaExistente;
        }

        public async Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento)
        {
            var tarifa = await _context.Tarifas.FindAsync(idTarifa);
            if (tarifa != null)
            {
                tarifa.Descuento = nuevoDescuento;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public new async Task<List<OperationResult>> GetAllAsync()
        {
            var tarifas = await base.GetAllAsync();
            return tarifas.Select(t => new OperationResult { Success = true, Data = t }).ToList();
        }

        public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
        {
            var validationResult = ValidateTarifas(tarifas);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.SaveEntityAsync(tarifas);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
        {
            var validationResult = ValidateTarifas(tarifas);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.UpdateEntityAsync(tarifas);
        }

        private OperationResult ValidateTarifas(Tarifas tarifas)
        {
            if (tarifas == null)
            {
                return new OperationResult { Success = false, Message = "La tarifa no puede ser nula." };
            }

            if (tarifas.PrecioPorNoche <= 0)
            {
                return new OperationResult { Success = false, Message = "El precio por noche debe ser mayor a 0." };
            }

            if (string.IsNullOrWhiteSpace(tarifas.Descripcion) || tarifas.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción de la tarifa es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            if (tarifas.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la habitación debe ser mayor que cero." };
            }

            if (tarifas.FechaInicio == default || tarifas.FechaFin == default)
            {
                return new OperationResult { Success = false, Message = "Las fechas de inicio y fin son obligatorias." };
            }

            return new OperationResult { Success = true };
        }
    }
}