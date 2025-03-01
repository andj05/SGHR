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

        public async Task<IEnumerable<RolUsuario>> GetAllAsync()
        {
            try
            {
                return await _context.RolUsuario
                    .Where(r => r.Estado == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles de usuario.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var rolUsuario = await _context.RolUsuario.FindAsync(id);
                if (rolUsuario == null)
                {
                    result.Success = false;
                    result.Message = "Rol de usuario no encontrado.";
                }
                else
                {
                    result.Success = true;
                    result.Data = rolUsuario;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el rol de usuario con ID {id}.");
                result.Success = false;
                result.Message = $"Error al obtener el rol de usuario: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ExistsAsync(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "El ID del rol de usuario es inválido.";
                return result;
            }

            try
            {
                bool exists = await _context.RolUsuario.AnyAsync(r => r.Id == id);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del rol de usuario con ID {id}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del rol de usuario: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> SaveEntityAsync(int idrolUsuario)
        {
            var rolUsuario = await _context.Set<RolUsuario>().FindAsync(idrolUsuario);
            if (rolUsuario == null)
            {
                return new OperationResult { Success = false, Message = "El rol de usuario no fue encontrado." };
            }
            return await SaveEntityAsync(rolUsuario);
        }


        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario rolUsuario)
        {
            var result = new OperationResult();
            try
            {
                var existingRolUsuario = await _context.Set<RolUsuario>().FindAsync(rolUsuario.Id);
                if (existingRolUsuario == null)
                {
                    return new OperationResult { Success = false, Message = "Rol de Usuario no encontrado." };
                }

                // Actualizar los datos modificables
                existingRolUsuario.Descripcion = rolUsuario.Descripcion ?? existingRolUsuario.Descripcion;
                existingRolUsuario.Estado = rolUsuario.Estado;
                existingRolUsuario.ModifyDate = DateTime.Now;
                existingRolUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                // Guardar cambios
                _context.Update(existingRolUsuario);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingRolUsuario;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Rol de Usuarioo.");
                result.Success = false;
                result.Message = $"Error al actualizar el Rol de Usuario: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> DeleteEntityAsync(int id)
        {
            try
            {
                var rolUsuario = await _context.RolUsuario.FindAsync(id);
                if (rolUsuario == null)
                {
                    return new OperationResult { Success = false, Message = "Rol de usuario no encontrado." };
                }

                _context.RolUsuario.Remove(rolUsuario);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Rol de usuario eliminado permanentemente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol de usuario.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el rol de usuario: {ex.Message}" };
            }
        }

        private OperationResult ValidateRolUsuario(RolUsuario rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion) || rolUsuario.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del rol es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}
