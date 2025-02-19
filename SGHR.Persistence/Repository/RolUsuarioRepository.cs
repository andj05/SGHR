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

        public async Task<bool> ActualizarEstadoRolAsync(int idRolUsuario, bool estado)
        {
            var rolUsuario = await _context.RolUsuario.FindAsync(idRolUsuario);
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
            return await _context.RolUsuario
                .AnyAsync(r => r.IdRolUsuario == idRolUsuario);
        }

        public async Task<bool> ActualizarDescripcionRolAsync(int idRolUsuario, string nuevaDescripcion)
        {
            var rolUsuario = await _context.RolUsuario.FindAsync(idRolUsuario);
            if (rolUsuario != null)
            {
                rolUsuario.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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

        // Implementación del método SaveEntityAsync
        public override async Task<OperationResult> SaveEntityAsync(RolUsuario entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del rol no puede estar vacía." };

            _context.RolUsuario.Add(entity);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Data = entity };
        }

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción del rol no puede estar vacía." };

            _context.RolUsuario.Update(entity);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Rol de usuario actualizado correctamente.", Data = entity };
        }
    }
}