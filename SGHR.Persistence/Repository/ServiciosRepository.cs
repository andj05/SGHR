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
            return await _context.Servicios
                .Where(s => s.Estado == true)
                .ToListAsync();
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
                .AnyAsync(s => s.IdServicio == idServicio);
        }


        public override async Task<OperationResult> SaveEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicio(servicio);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.SaveEntityAsync(servicio);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Servicios servicio)
        {
            var validationResult = ValidateServicio(servicio);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.UpdateEntityAsync(servicio);
        }

        private OperationResult ValidateServicio(Servicios servicio)
        {
            if (servicio == null)
            {
                return new OperationResult { Success = false, Message = "El servicio no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Nombre) || servicio.Nombre.Length > 50)
            {
                return new OperationResult { Success = false, Message = "El nombre del servicio es obligatorio y debe tener un máximo de 50 caracteres." };
            }

            if (string.IsNullOrWhiteSpace(servicio.Descripcion) || servicio.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del servicio es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}