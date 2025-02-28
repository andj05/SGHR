using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;


namespace SGHR.Application.Services
{
    public class RolUsuarioService : IRolUsuarioService
    {
        private readonly IRolUsuarioRepository _rolUsuarioRepository;
        private readonly ILogger<RolUsuarioService> _logger;
        private readonly IConfiguration _configuration;

        public RolUsuarioService(IRolUsuarioRepository rolUsuarioRepository,
                                ILogger<RolUsuarioService> logger,
                                IConfiguration configuration)
        {
            _rolUsuarioRepository = rolUsuarioRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var roles = await _rolUsuarioRepository.GetAllAsync();
                operationResult.Data = roles.Where(r => !r.Deleted).ToList(); // Filtrar roles eliminados
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario.");
                operationResult.Message = "Error al obtener todos los roles de usuario.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rolUsuario.Data is RolUsuario rolData && rolData.Deleted)
                {
                    return new OperationResult { Success = false, Message = "Rol de usuario eliminado." };
                }

                operationResult.Data = rolUsuario.Data;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el rol de usuario con ID {id}.");
                operationResult.Message = "Error al obtener el rol de usuario por ID.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveRolUsuarioDto dto)
        {
            var validationResult = ValidateRolUsuario(dto);
            if (!validationResult.Success != null)
                return validationResult;

            try
            {
                var rolUsuario = new RolUsuario
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

                return await _rolUsuarioRepository.SaveEntityAsync(rolUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el rol de usuario.");
                return new OperationResult { Success = false, Message = "Error al guardar el rol de usuario." };
            }
        }

        public async Task<OperationResult> Update(UpdateRolUsuarioDto dto)
        {
            if (dto.IdRolUsuario <= 0)
                return new OperationResult { Success = false, Message = "ID de rol de usuario inválido." };

            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
            if (rolUsuario?.Data is not RolUsuario rolData || rolData.Deleted)
                return new OperationResult { Success = false, Message = "Rol de usuario no encontrado o eliminado." };

            rolData.Descripcion = dto.Descripcion ?? rolData.Descripcion;
            rolData.Estado = dto.Estado ?? rolData.Estado;
            rolData.ModifyDate = DateTime.Now;
            rolData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _rolUsuarioRepository.UpdateEntityAsync(rolData);
        }

        public async Task<OperationResult> Remove(RemoveRolUsuarioDto dto)
        {
            if (dto.IdRolUsuario <= 0)
                return new OperationResult { Success = false, Message = "ID de rol de usuario inválido." };

            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
            if (rolUsuario.Data is not RolUsuario rolData || rolData.Deleted)
                return new OperationResult { Success = false, Message = "Rol de usuario no encontrado o ya eliminado." };

            rolData.Deleted = true;
            rolData.DeletedUser = 1;
            rolData.ModifyDate = DateTime.Now;

            return await _rolUsuarioRepository.UpdateEntityAsync(rolData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (rolUsuario.Data is not RolUsuario rolData || !rolData.Deleted)
                return new OperationResult { Success = false, Message = "Rol de usuario no encontrado o ya activo." };

            rolData.Deleted = false;
            rolData.ModifyDate = DateTime.Now;
            rolData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _rolUsuarioRepository.UpdateEntityAsync(rolData);
        }

        public async Task<OperationResult> DeletePermanent(int id)
        {
            if (id <= 0)
                return new OperationResult { Success = false, Message = "ID de rol de usuario inválido." };

            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (rolUsuario.Data is not RolUsuario rolUsuarioData)
                return new OperationResult { Success = false, Message = "Rol de usuario no encontrado." };

            return await _rolUsuarioRepository.DeleteEntityAsync(id);
        }

        private OperationResult ValidateRolUsuario(dynamic rolUsuario)
        {
            if (rolUsuario == null)
                return new OperationResult { Success = false, Message = "El rol de usuario no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion) || rolUsuario.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = "La descripción no puede estar vacía y debe tener un máximo de 50 caracteres." };

            return new OperationResult { Success = true };
        }

    }
}