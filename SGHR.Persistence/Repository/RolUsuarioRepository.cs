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

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync()
        {
            return await _context.RolUsuarios.ToListAsync();
        }

        public async Task<IEnumerable<RolUsuario>> ObtenerRolesActivosAsync()
        {
            return await _context.RolUsuarios
                .Where(r => r.Estado == true) // Asumiendo que el campo 'Estado' determina si el rol está activo
                .ToListAsync();
        }

        public async Task<RolUsuario?> ObtenerRolPorDescripcionAsync(string descripcion)
        {
            return await _context.RolUsuarios
                .Where(r => r.Descripcion == descripcion)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ActualizarEstadoRolAsync(int idRolUsuario, bool estado)
        {
            var rolUsuario = await _context.RolUsuarios.FindAsync(idRolUsuario);
            if (rolUsuario != null)
            {
                rolUsuario.Estado = estado;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExisteRolUsuarioAsync(int idRolUsuario)
        {
            return await _context.RolUsuarios
                .AnyAsync(r => r.IdRolUsuario == idRolUsuario);
        }

        public async Task<bool> ActualizarDescripcionRolAsync(int idRolUsuario, string nuevaDescripcion)
        {
            var rolUsuario = await _context.RolUsuarios.FindAsync(idRolUsuario);
            if (rolUsuario != null)
            {
                rolUsuario.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Otros métodos de la clase (como SaveEntityAsync y UpdateEntityAsync)

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del rol no puede estar vacía." };

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del rol no puede estar vacía." };

            return await base.UpdateEntityAsync(entity);
        }
    }
}

