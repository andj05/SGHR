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

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Servicios>> ObtenerTodosLosServiciosAsync()
        {
            return await _context.Servicios.ToListAsync();
        }

        public async Task<IEnumerable<Servicios>> ObtenerServiciosActivosAsync()
        {
            return await _context.Servicios
                .Where(s => s.Estado == true) // Suponiendo que 'Estado' es un campo que indica si el servicio está activo.
                .ToListAsync();
        }

        public async Task<Servicios?> ObtenerServicioPorNombreAsync(string nombre)
        {
            return await _context.Servicios
                .FirstOrDefaultAsync(s => s.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ActualizarEstadoServicioAsync(int idServicio, bool estado)
        {
            var servicio = await _context.Servicios.FindAsync(idServicio);
            if (servicio != null)
            {
                servicio.Estado = estado;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExisteServicioAsync(int idServicio)
        {
            return await _context.Servicios
                .AnyAsync(s => s.IdServicios == idServicio);
        }

        public async Task<bool> ActualizarDescripcionServicioAsync(int idServicio, string nuevaDescripcion)
        {
            var servicio = await _context.Servicios.FindAsync(idServicio);
            if (servicio != null)
            {
                servicio.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Métodos adicionales para SaveEntityAsync y UpdateEntityAsync si es necesario

        public override async Task<OperationResult> SaveEntityAsync(Servicios entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El servicio no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio." };

            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del servicio es obligatoria." };

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Servicios entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El servicio no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio." };

            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del servicio es obligatoria." };

            return await base.UpdateEntityAsync(entity);
        }
    }
}

