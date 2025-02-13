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

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Piso>> ObtenerTodosLosPisosAsync()
        {
            return await _context.Pisos.ToListAsync();
        }

        public async Task<IEnumerable<Piso>> ObtenerPisosActivosAsync()
        {
            return await _context.Pisos
                .Where(p => p.Estado == true) // Suponiendo que "Estado" es un campo booleano que determina si está activo
                .ToListAsync();
        }

        public async Task<Piso?> ObtenerPisoPorDescripcionAsync(string descripcion)
        {
            return await _context.Pisos
                .FirstOrDefaultAsync(p => p.Descripcion == descripcion);
        }

        public async Task<bool> ActualizarEstadoPisoAsync(int idPiso, bool estado)
        {
            var piso = await _context.Pisos.FindAsync(idPiso);
            if (piso != null)
            {
                piso.Estado = estado;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistePisoAsync(int idPiso)
        {
            return await _context.Pisos
                .AnyAsync(p => p.IdPiso == idPiso);
        }

        public async Task<bool> ActualizarDescripcionPisoAsync(int idPiso, string nuevaDescripcion)
        {
            var piso = await _context.Pisos.FindAsync(idPiso);
            if (piso != null)
            {
                piso.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Métodos adicionales para manejar la entidad de Piso

        public override async Task<OperationResult> SaveEntityAsync(Piso entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El piso no puede ser nulo." };

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Piso entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El piso no puede ser nulo." };

            return await base.UpdateEntityAsync(entity);
        }
    }
}
