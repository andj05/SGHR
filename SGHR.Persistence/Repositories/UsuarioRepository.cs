using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<UsuarioRepository> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioRepository(SGHRContext context,
                                ILogger<UsuarioRepository> logger,
                                IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Usuario>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes.");
                throw;
            }
        }

        public async Task<OperationResult> GetUsersByStatusAsync(int idEstadoUsuario)
        {
            var result = new OperationResult();
            try
            {
                bool estado = idEstadoUsuario == 1;
                var usuarios = await _context.Set<Usuario>()
                                             .Where(u => u.Estado == estado)
                                             .ToListAsync();
                result.Data = usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuarios con estado {idEstadoUsuario}.");
                result.Success = false;
                result.Message = $"Error al obtener usuarios por estado: {ex.Message}";
            }
            return result;
        }
        public async Task<OperationResult> GetEntityByIdAsync(int idUsuario)
        {
            var result = new OperationResult();
            try
            {
                var usuario = await _context.Set<Usuario>().FindAsync(idUsuario);
                if (usuario != null)
                {
                    result.Data = usuario;
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Usuario no encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el usuario con id {idUsuario}.");
                result.Success = false;
                result.Message = $"Error al obtener el usuario: {ex.Message}";
            }
            return result;
        }
        public async Task<OperationResult> GetUsersByFilterAsync(Expression<Func<Usuario, bool>> filter)
        {
            if (filter == null)
                return new OperationResult { Success = false, Message = "El filtro no puede ser nulo." };

            var result = new OperationResult();
            try
            {
                var usuarios = await _context.Set<Usuario>().Where(filter).ToListAsync();
                result.Data = usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios por filtro.");
                result.Success = false;
                result.Message = $"Error al obtener usuarios por filtro: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> SaveEntityAsync(int idUsuario)
        {
            var usario = await _context.Set<Usuario>().FindAsync(idUsuario);
            if (usario == null)
            {
                return new OperationResult { Success = false, Message = "El usario no fue encontrado." };
            }
            return await SaveEntityAsync(usario);
        }

        public async Task<OperationResult> UpdateEntityAsync(Usuario usuario)
        {
            var result = new OperationResult();
            try
            {
                var existingUsuario = await _context.Set<Usuario>().FindAsync(usuario.Id);
                if (existingUsuario == null)
                {
                    return new OperationResult { Success = false, Message = "Usuario no encontrado." };
                }

                // Actualizar los datos modificables
                existingUsuario.NombreCompleto = usuario.NombreCompleto ?? existingUsuario.NombreCompleto;
                existingUsuario.Correo = usuario.Correo ?? existingUsuario.Correo;
                existingUsuario.IdRolUsuario = usuario.IdRolUsuario > 0 ? usuario.IdRolUsuario : existingUsuario.IdRolUsuario;
                existingUsuario.Clave = usuario.Clave ?? existingUsuario.Clave;
                existingUsuario.ModifyDate = DateTime.Now;
                existingUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                // Guardar cambios
                _context.Update(existingUsuario);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingUsuario;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario.");
                result.Success = false;
                result.Message = $"Error al actualizar el usuario: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> DeleteEntityAsync(int idUsuario)
        {
            if (idUsuario <= 0)
                return new OperationResult { Success = false, Message = "El ID del usuario es inválido." };
            try
            {
                var usuario = await _context.Set<Usuario>().FindAsync(idUsuario);
                if (usuario == null)
                    return new OperationResult { Success = false, Message = "Usuario no encontrado." };
                _context.Set<Usuario>().Remove(usuario);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Usuario eliminado exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el usuario con id {idUsuario}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el usuario: {ex.Message}" };
            }
        }

        public async Task<OperationResult> ExistsAsync(int idUsuario)
        {
            var result = new OperationResult();
            if (idUsuario <= 0)
            {
                result.Success = false;
                result.Message = "El ID del usuario es inválido.";
                return result;
            }

            try
            {
                bool exists = await _context.Set<Usuario>().AnyAsync(u => u.Id == idUsuario);
                result.Success = exists;
                result.Message = exists ? "Usuario encontrado." : "Usuario no encontrado.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del usuario con id {idUsuario}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del usuario: {ex.Message}";
            }
            return result;
        }
    }
}
