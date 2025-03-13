using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;


namespace SGHR.Application.Services
{
    public class RolUsuarioService : IRolUsuarioService
    {
        private readonly IRolUsuarioRepository _rolUsuarioRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public RolUsuarioService(IRolUsuarioRepository rolUsuarioRepository,
                                 MessageMapper messageMapper,
                                 ILoggerManager loggerManager)
        {
            _rolUsuarioRepository = rolUsuarioRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var rolUsuarios = await _rolUsuarioRepository.GetAllAsync();
                var activerolUsuarios = rolUsuarios
                    .Where(t => !t.Deleted)
                    .Select(RolUsuarioMapper.ToDto)
                    .OrderByDescending(h => h.ChangeData)
                    .ToList();
                result.Data = activerolUsuarios;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Success = false;
                result.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rolUsuario == null)
                {
                    operationResult.Success = false;
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                }
                else
                {
                    operationResult.Data = rolUsuario;
                    operationResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveRolUsuarioDto dto)
        {
            var validationResult = ValidateRolUsuario(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var rolUsuario = RolUsuarioMapper.ToEntity(dto);
                return await _rolUsuarioRepository.SaveEntityAsync(rolUsuario);
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

        public async Task<OperationResult> Update(UpdateRolUsuarioDto dto)
        {
            if (dto.IdRolUsuario <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
            if (rolUsuario == null || rolUsuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            rolUsuario.UpdateFromDto(dto);
            return await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
        }

        public async Task<OperationResult> Remove(RemoveRolUsuarioDto dto)
        {
            if (dto.IdRolUsuario <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
            if (rolUsuario == null || rolUsuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            rolUsuario.RemoveFromDto(dto);
            return await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (rolUsuario == null || !rolUsuario.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            rolUsuario.RestoreFromDto(1); 
            return await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
        }

        private OperationResult ValidateRolUsuario(dynamic rolUsuario)
        {
            if (rolUsuario == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["NullRolUsuario"]
                };

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion) || rolUsuario.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["InvalidDescription"]
                };

            return new OperationResult { Success = true };
        }
    }
}