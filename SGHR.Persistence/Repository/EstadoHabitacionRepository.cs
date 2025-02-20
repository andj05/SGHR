
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

        public async Task<EstadoHabitacion?> ObtenerEstadoPorDescripcionAsync(string descripcion)
        {
            return await _context.EstadoHabitacion
                .FirstOrDefaultAsync(e => e.Descripcion == descripcion);
        }

        public async Task<bool> ActualizarEstadoAsync(int idEstado, bool estado)
        {
            var estadoHabitacion = await _context.EstadoHabitacion.FindAsync(idEstado);
            if (estadoHabitacion != null)
            {
                estadoHabitacion.Estado = estado; 
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExisteEstadoHabitacionAsync(int idEstado)
        {
            return await _context.EstadoHabitacion
                .AnyAsync(e => e.IdEstadoHabitacion == idEstado);
        }

        public async Task<bool> ActualizarDescripcionEstadoAsync(int idEstado, string nuevaDescripcion)
        {
            var estadoHabitacion = await _context.EstadoHabitacion.FindAsync(idEstado);
            if (estadoHabitacion != null)
            {
                estadoHabitacion.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public override async Task<OperationResult> SaveEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(estadoHabitacion.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción no puede ser vacía." };
            return await base.SaveEntityAsync(estadoHabitacion);
        }

        public override async Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion)
        {
            if (estadoHabitacion == null)
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(estadoHabitacion.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción no puede ser vacía." };
            return await base.UpdateEntityAsync(estadoHabitacion);
        }
    }
}
