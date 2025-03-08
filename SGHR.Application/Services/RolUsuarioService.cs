using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;


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
            var operationResult = new OperationResult();
            try
            {
                var roles = await _rolUsuarioRepository.GetAllAsync();
                operationResult.Data = roles;
                operationResult.Success = true;
                operationResult.Message = _messageMapper.SuccessMessages["GenericSuccess"];
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
                var rolUsuario = new RolUsuario
                {
                    FechaCreacion = DateTime.UtcNow,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

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

            rolUsuario.Descripcion = dto.Descripcion ?? rolUsuario.Descripcion;
            rolUsuario.Estado = dto.Estado ?? rolUsuario.Estado;
            rolUsuario.ModifyDate = DateTime.Now;
            rolUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado

            var result = await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
            return result;
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

            rolUsuario.Deleted = true;
            rolUsuario.DeletedUser = 1; // En producción, obtener el usuario autenticado.
            rolUsuario.ModifyDate = DateTime.Now;

            var result = await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
            return result;
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

            rolUsuario.Deleted = false;
            rolUsuario.ModifyDate = DateTime.Now;
            rolUsuario.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
            return result;
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