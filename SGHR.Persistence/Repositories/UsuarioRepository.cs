using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Base;
using SGHR.Persistence.Contex;
using SGHR.Persistence.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<UsuarioRepository> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioRepository(
            SGHRContext context,
            ILogger<UsuarioRepository> logger,
            IConfiguration configuration)
            : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> ObtenerTodosLosUsuariosAsync()
        {
            var result = new OperationResult();
            try
            {
                var usuarios = await _context.Set<Usuario>().ToListAsync();
                result.Data = usuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                result.Success = false;
                result.Message = $"Error al obtener usuarios: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ObtenerUsuariosPorEstadoIdAsync(int idEstadoUsuario)
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

        public async Task<OperationResult> ObtenerUsuariosPorFilterAsync(Expression<Func<Usuario, bool>> filter)
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

        public async Task<OperationResult> GuardarUsuariosAsync(Usuario usario)
        {
            if (usario == null)
                return new OperationResult { Success = false, Message = "El usuario no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(usario.NombreCompleto))
                return new OperationResult { Success = false, Message = "El nombre completo es obligatorio." };

            if (string.IsNullOrWhiteSpace(usario.Correo))
                return new OperationResult { Success = false, Message = "El correo es obligatorio." };

            if (string.IsNullOrWhiteSpace(usario.Clave))
                return new OperationResult { Success = false, Message = "La clave es obligatoria." };

            try
            {
                return await SaveEntityAsync(usario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el usuario.");
                return new OperationResult { Success = false, Message = $"Error al guardar el usuario: {ex.Message}" };
            }
        }

        public async Task<OperationResult> ActualizarUsuariosAsync(Usuario usario)
        {
            if (usario == null)
                return new OperationResult { Success = false, Message = "El usuario no puede ser nulo." };

            if (usario.IdUsuario <= 0)
                return new OperationResult { Success = false, Message = "El ID del usuario es inválido." };

            try
            {
                return await UpdateEntityAsync(usario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario.");
                return new OperationResult { Success = false, Message = $"Error al actualizar el usuario: {ex.Message}" };
            }
        }

        public async Task<bool> ExisteUsuarioAsync(int id)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _context.Set<Usuario>().AnyAsync(u => u.IdUsuario == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del usuario con id {id}.");
                return false;
            }
        }
    }
}
