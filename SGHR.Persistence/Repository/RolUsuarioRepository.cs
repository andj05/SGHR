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
    public class RolUsuarioRepository : BaseRepository<RolUsuario>, IRolUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<RolUsuarioRepository> _logger;
        private readonly IConfiguration _configuration;

        public RolUsuarioRepository(SGHRContext context,
                                     ILogger<RolUsuarioRepository> logger,
                                     IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync()
        {
            return await _context.RolUsuario
                .Where(r => r.Estado == true) // Correccion que me hizo el maestro "Filtrado Mejor (Ahora solo filtra los RolUsarios Activos) "
                .ToListAsync();
        }

        public async Task<RolUsuario?> ObtenerRolPorDescripcionAsync(string descripcion)
        {
            return await _context.RolUsuario
                .Where(r => r.Descripcion == descripcion)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExisteRolUsuarioAsync(int idRolUsuario)
        {
            return await _context.RolUsuario
                .AnyAsync(r => r.IdRolUsuario == idRolUsuario);
        }

        public async Task<OperationResult> EliminarRolAsync(int idRolUsuario)
        {
            var rolUsuario = await _context.RolUsuario.FindAsync(idRolUsuario);
            if (rolUsuario != null)
            {
                _context.RolUsuario.Remove(rolUsuario);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Rol eliminado exitosamente." };
            }
            return new OperationResult { Success = false, Message = "Rol no encontrado." };
        }

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario rolUsuario)
        {
            var validationResult = ValidateRolUsuario(rolUsuario);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.SaveEntityAsync(rolUsuario);
        }

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario rolUsuario)
        {
            var validationResult = ValidateRolUsuario(rolUsuario);
            if (validationResult.Success.HasValue && !validationResult.Success.Value)
            {
                return validationResult;
            }
            return await base.UpdateEntityAsync(rolUsuario);
        }
        private OperationResult ValidateRolUsuario(RolUsuario rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion) || rolUsuario.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del rol es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}