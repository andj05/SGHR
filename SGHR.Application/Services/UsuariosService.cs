using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Usuario;
using SGHR.Application.Intefaces;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

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
                var usuario = await _usuarioRepository.GetAllAsync();
                var activeUsuario = usuario
                    .Where(t => !t.Deleted)
                    .Select(UsuarioMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                operationResult.Data = activeUsuario;
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

        public async Task<OperationResult> GerAllDelete()
        {
            var operationResult = new OperationResult();
            try
            {
                var usuario = await _usuarioRepository.GetAllAsync();
                var deletedUsuario = usuario
                    .Where(t => t.Deleted)
                    .Select(UsuarioMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                operationResult.Data = deletedUsuario;
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
                if (usuario == null)
                {
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = usuario;
                    operationResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Login(LoginRequestDto loginRequest)
        {
            var operationResult = new OperationResult();
            try
            {
                var usuario = await _usuarioRepository.GetByEmailAsync(loginRequest.Correo);
                if (usuario == null || usuario.Deleted || usuario.Clave != loginRequest.Clave)
                {
                    operationResult.Success = false;
                    operationResult.Message = _messageMapper.ErrorMessages["User"]["InvalidCredentials"];
                    return operationResult;
                }

                operationResult.Success = true;
                operationResult.Message = _messageMapper.SuccessMessages["LoginSuccess"];
                operationResult.Data = UsuarioMapper.ToDto(usuario);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["LoginFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["LoginFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveUsuarioDto dto)
        {
            var validationResult = ValidateUsuario(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var usuario = UsuarioMapper.ToEntity(dto);
                return await _usuarioRepository.SaveEntityAsync(usuario);
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
            if (dto.IdUsuario <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
            if (usuario == null || usuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            usuario.UpdateFromDto(dto);
            return await _usuarioRepository.UpdateEntityAsync(usuario);
        }

        public async Task<OperationResult> Remove(RemoveUsuarioDto dto)
        {
            if (dto.IdUsuario <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.IdUsuario);
            if (usuario == null || usuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            usuario.RemoveFromDto(dto);
            return await _usuarioRepository.UpdateEntityAsync(usuario);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null || !usuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = usuario == null
                        ? _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                        : _messageMapper.ErrorMessages["EntityBase"]["AlreadyActive"]
                };

            usuario.RestoreFromDto(1);
            return await _usuarioRepository.UpdateEntityAsync(usuario);
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
