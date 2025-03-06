using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

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

        public override async Task<List<RolUsuario>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolUsuario>().Where(h => !h.Deleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Rol Usuario.");
                throw;
            }
        }

        public override async Task<RolUsuario> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID del rol de usuario inválido: {id}");
                return null;
            }

            try
            {
                var rolUsuario = await _context.Set<RolUsuario>().FindAsync(id);
                if (rolUsuario == null || rolUsuario.Deleted)
                {
                    _logger.LogWarning($"El rol de usuario con ID {id} no encontrado o eliminado.");
                    return null;
                }
                return rolUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el rol de usuario con ID {id}.");
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<RolUsuario, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.Set<RolUsuario>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del rol de usuario.");
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario rolUsuario)
        {
            var validation = ValidateRolUsuario(rolUsuario);
            if (!validation.Success != null)
                return validation;

            var result = new OperationResult();
            try
            {
                rolUsuario.FechaCreacion = DateTime.Now;
                rolUsuario.ModifyDate = DateTime.Now;
                rolUsuario.ModifyUser = 1;

                await _context.Set<RolUsuario>().AddAsync(rolUsuario);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = rolUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el rol de usuario.");
                result.Success = false;
                result.Message = $"Error al guardar el rol de usuario: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario rolUsuario)
        {
            var validationResult = ValidateRolUsuario(rolUsuario);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                var existingRolUsuario = await _context.Set<RolUsuario>().FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                {
                    return new OperationResult { Success = false, Message = "Rol de usuario no encontrado." };
                }

                // Actualizar los datos modificables
                existingRolUsuario.Descripcion = rolUsuario.Descripcion ?? existingRolUsuario.Descripcion;
                existingRolUsuario.Estado = rolUsuario.Estado;
                existingRolUsuario.ModifyDate = DateTime.Now;
                existingRolUsuario.ModifyUser = 1;

                // Guardar cambios
                _context.Entry(existingRolUsuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingRolUsuario;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol de usuario.");
                result.Success = false;
                result.Message = $"Error al actualizar el rol de usuario: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(RolUsuario rolUsuario)
        {
            if (rolUsuario == null || rolUsuario.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID del rol de usuario es inválido." };

            try
            {
                var existingRolUsuario = await _context.Set<RolUsuario>().FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                    return new OperationResult { Success = false, Message = "Rol de usuario no encontrado." };

                _context.Set<RolUsuario>().Remove(existingRolUsuario);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Rol de usuario eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el rol de usuario con ID {rolUsuario.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el rol de usuario: {ex.Message}" };
            }
        }

        private OperationResult ValidateRolUsuario(RolUsuario rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            }

            if (rolUsuario.Descripcion != null && rolUsuario.Descripcion.Length > 50)
            {
                return new OperationResult { Success = false, Message = "La descripción del rol debe tener un máximo de 50 caracteres." };
            }

            if (rolUsuario.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
