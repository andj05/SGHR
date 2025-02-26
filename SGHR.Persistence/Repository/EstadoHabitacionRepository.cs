using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
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

        // Implementación de los métodos de la interfaz
        public async Task<IEnumerable<EstadoHabitacion>> ObtenerTodosLosEstadosAsync()
        {
            return await _context.EstadoHabitacion
                .Where(e => e.Estado == true)
                .ToListAsync();
        }

        public async Task<bool> ExisteEstadoHabitacionAsync(int idEstado)
        {
            return await _context.EstadoHabitacion
                .AnyAsync(e => e.IdEstadoHabitacion == idEstado);
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacion(estadoHabitacion);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.SaveEntityAsync(estadoHabitacion);
        }

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            var validationResult = ValidateEstadoHabitacion(estadoHabitacion);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.UpdateEntityAsync(estadoHabitacion);
        }

        private OperationResult ValidateEstadoHabitacion(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
            {
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(estadoHabitacion.Descripcion) || estadoHabitacion.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del estado de habitación es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}


