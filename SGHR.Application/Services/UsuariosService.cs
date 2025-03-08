using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Usuario;
using SGHR.Application.Intefaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;
        private UsuarioRepository usuarioRepository;
        private MessageMapper messageMapper;
        private LoggerManager logger;

        public UsuariosService(IUsuarioRepository usuariosRepository,
                               MessageMapper messageMapper,
                               ILoggerManager loggerManager)
        {
            _usuarioRepository = usuariosRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                operationResult.Data = usuarios.Where(u => !u.Deleted).ToList();
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                // Si el usuario existe pero está eliminado, se devuelve el mensaje NotFound.
                if (usuario is Usuario usuarioData && usuarioData.Deleted)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }
                operationResult.Data = usuario;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveUsuarioDto dto)
        {
            var validationResult = ValidateUsuario(dto);
            if (validationResult.Success == true)
                return validationResult;

            try
            {
                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    Correo = dto.Correo,
                    IdRolUsuario = dto.IdRolUsuario,
                    Clave = dto.Clave,
                    CreationUser = 1 // En producción, obtener el usuario autenticado.
                };

                var result = await _usuarioRepository.SaveEntityAsync(usuario);
                return result;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = $"{_messageMapper.ErrorMessages["Operations"]["SaveFailed"]}: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> Update(UpdateUsuarioDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdUsuario <= 0)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                    };

                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
                if (usuario is not Usuario usuarioData || usuarioData.Deleted)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };

                usuarioData.NombreCompleto = dto.NombreCompleto ?? usuarioData.NombreCompleto;
                usuarioData.Correo = dto.Correo ?? usuarioData.Correo;
                usuarioData.IdRolUsuario = dto.IdRolUsuario ?? usuarioData.IdRolUsuario;
                usuarioData.Clave = dto.Clave ?? usuarioData.Clave;
                usuarioData.ModifyDate = DateTime.Now;
                usuarioData.ModifyUser = 1;

                var updateResult = await _usuarioRepository.UpdateEntityAsync(usuarioData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Success == true
                    ? updateResult.Message
                    : $"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {updateResult.Message}";
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveUsuarioDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdUsuario <= 0)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                    };

                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
                if (usuario is not Usuario usuarioData || usuarioData.Deleted)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };

                usuarioData.Deleted = true;
                usuarioData.DeletedUser = 1;
                usuarioData.ModifyDate = DateTime.Now;

                var updateResult = await _usuarioRepository.UpdateEntityAsync(usuarioData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Success == true
                    ? _messageMapper.SuccessMessages["DeleteSuccess"]
                    : $"{_messageMapper.ErrorMessages["Operations"]["DeleteFailed"]}: {updateResult.Message}";
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DeleteFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (usuario is not Usuario usuarioData || !usuarioData.Deleted)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };

                usuarioData.Deleted = false;
                usuarioData.ModifyDate = DateTime.Now;
                usuarioData.ModifyUser = 1;

                var updateResult = await _usuarioRepository.UpdateEntityAsync(usuarioData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Success == true
                    ? _messageMapper.SuccessMessages["RestoreSuccess"]
                    : $"{_messageMapper.ErrorMessages["Operations"]["RestoreFailed"]}: {updateResult.Message}";
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["RestoreFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        private OperationResult ValidateUsuario(dynamic usuario)
        {
            if (usuario == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"]
                };

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto) || usuario.NombreCompleto.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Client"]["MissingName"]
                };

            if (string.IsNullOrWhiteSpace(usuario.Correo) || usuario.Correo.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["User"]["MissingEmail"]
                };

            if (usuario.IdRolUsuario <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["User"]["InvalidRoleID"]
                };

            if (string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 6)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Client"]["MissingPassword"]
                };

            return new OperationResult { Success = true };
        }
    }
}
