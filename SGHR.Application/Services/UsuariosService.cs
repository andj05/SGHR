using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Usuario;
using SGHR.Application.Intefaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;
namespace SGHR.Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuariosService> _logger;
        private readonly IConfiguration _configuration;

        public UsuariosService(IUsuarioRepository usuariosRepository,
                                ILogger<UsuariosService> logger,
                                IConfiguration configuration)
        {
            _usuarioRepository = usuariosRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                operationResult.Data = usuarios.Where(u => !u.Deleted).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios.");
                operationResult.Success = false;
                operationResult.Message = $"Error al obtener usuarios: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario.Data is Usuario usuarioData && usuarioData.Deleted)
                return new OperationResult { Success = false, Message = "Usuario eliminado." };

            return usuario;
        }

        public async Task<OperationResult> Save(SaveUsuarioDto dto)
        {
            var validationResult = ValidateUsuario(dto);
            if (!validationResult.Success != true)
                return validationResult;

            try
            {
                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    Correo = dto.Correo,
                    IdRolUsuario = dto.IdRolUsuario,
                    Clave = dto.Clave,
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

                return await _usuarioRepository.SaveEntityAsync(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el usuario.");
                return new OperationResult { Success = false, Message = $"Error al guardar el usuario: {ex.Message}" };
            }
        }

        public async Task<OperationResult> Update(UpdateUsuarioDto dto)
        {
            if (dto.IdUsuario <= 0)
                return new OperationResult { Success = false, Message = "ID de usuario inválido." };

            var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
            if (usuario.Data is not Usuario usuarioData || usuarioData.Deleted)
                return new OperationResult { Success = false, Message = "Usuario no encontrado o eliminado." };

            usuarioData.NombreCompleto = dto.NombreCompleto ?? usuarioData.NombreCompleto;
            usuarioData.Correo = dto.Correo ?? usuarioData.Correo;
            usuarioData.IdRolUsuario = dto.IdRolUsuario ?? usuarioData.IdRolUsuario;
            usuarioData.Clave = dto.Clave ?? usuarioData.Clave;
            usuarioData.ModifyDate = DateTime.Now;
            usuarioData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _usuarioRepository.UpdateEntityAsync(usuarioData);
        }

        public async Task<OperationResult> Remove(RemoveUsuarioDto dto)
        {
            if (dto.IdUsuario <= 0)
                return new OperationResult { Success = false, Message = "ID de cliente inválido." };

            var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
            if (usuario.Data is not Usuario usuarioData || usuarioData.Deleted)
                return new OperationResult { Success = false, Message = "Cliente no encontrado o ya eliminado." };

            usuarioData.Deleted = true;
            usuarioData.DeletedUser = 1;
            usuarioData.ModifyDate = DateTime.Now;

            return await _usuarioRepository.UpdateEntityAsync(usuarioData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario.Data is not Usuario usuarioData || !usuarioData.Deleted)
                return new OperationResult { Success = false, Message = "Usuario no encontrado o ya activo." };

            usuarioData.Deleted = false;
            usuarioData.ModifyDate = DateTime.Now;
            usuarioData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _usuarioRepository.UpdateEntityAsync(usuarioData);
        }

        private OperationResult ValidateUsuario(dynamic usuario)
        {
            if (usuario == null)
                return new OperationResult { Success = false, Message = "El usuario no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto) || usuario.NombreCompleto.Length > 50)
                return new OperationResult { Success = false, Message = "El nombre completo no puede estar vacío y debe tener un máximo de 50 caracteres." };

            if (string.IsNullOrWhiteSpace(usuario.Correo) || usuario.Correo.Length > 50)
                return new OperationResult { Success = false, Message = "El correo es obligatorio y debe tener un máximo de 50 caracteres." };

            if (usuario.IdRolUsuario <= 0)
                return new OperationResult { Success = false, Message = "El rol del usuario es obligatorio." };

            if (string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 6)
                return new OperationResult { Success = false, Message = "La clave es obligatoria y debe tener al menos 6 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}