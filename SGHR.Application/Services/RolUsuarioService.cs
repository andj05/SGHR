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
            var result = new OperationResult();
            try
            {
                var rolUsuarios = await _rolUsuarioRepository.GetAllAsync();
                var activerolUsuarios = rolUsuarios
                    .Where(t => !t.Deleted)
                    .Select(RolUsuarioMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
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

        public async Task<OperationResult> GetAllDelete()
        {
            var result = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetAllAsync();
                var activerolUsuario = rolUsuario
                    .Where(t => t.Deleted)
                    .Select(RolUsuarioMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activerolUsuario;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                result.Success = false;
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
            // Validaciones básicas del objeto
            var validationResult = ValidateRolUsuario(dto);
            if (validationResult.Success != true)
                return validationResult;

            // Validaciones de negocio específicas para guardar
            var businessValidationResult = await ValidateRolUsuarioBusinessRules(dto);
            if (businessValidationResult.Success != true)
                return businessValidationResult;

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

            // Validación básica del objeto
            var validationResult = ValidateRolUsuario(dto);
            if (validationResult.Success != true)
                return validationResult;

            // Validaciones de negocio específicas para actualizar
            var businessValidationResult = await ValidateRolUsuarioUpdateBusinessRules(dto, rolUsuario);
            if (businessValidationResult.Success != true)
                return businessValidationResult;

            try
            {
                // Fix: Implementar UpdateFromDto manualmente en lugar de usar un método de extensión
                rolUsuario.Descripcion = dto.Descripcion;
                rolUsuario.Estado = dto.Estado;

                // Llamada al repositorio para actualizar
                return await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = $"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex.Message}"
                };
            }
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

            // Validación de negocio antes de restaurar
            var restoreValidationResult = await ValidateRolUsuarioRestoreBusinessRules(rolUsuario);
            if (restoreValidationResult.Success != true)
                return restoreValidationResult;

            rolUsuario.Deleted = false;
            return await _rolUsuarioRepository.RestoreEntityAsync(rolUsuario);
        }

        private OperationResult ValidateRolUsuario(dynamic rolUsuario)
        {
            if (rolUsuario == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["NullRole"]
                };

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["EmptyDescription"]
                };

            if (rolUsuario.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["DescriptionTooLong"]
                };

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidateRolUsuarioBusinessRules(SaveRolUsuarioDto dto)
        {
            // Validar que no exista otro rol con la misma descripción
            var existingRoles = await _rolUsuarioRepository.GetAllAsync();
            if (existingRoles.Any(r => !r.Deleted &&
                r.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["DuplicateDescription"]
                };
            }

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidateRolUsuarioUpdateBusinessRules(UpdateRolUsuarioDto dto, RolUsuario existingRol)
        {
            // Validar que no exista otro rol con la misma descripción (excepto el mismo rol)
            var existingRoles = await _rolUsuarioRepository.GetAllAsync();
            if (existingRoles.Any(r => !r.Deleted &&
                r.Id != dto.IdRolUsuario &&
                r.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["DuplicateDescriptionUpdate"]
                };
            }

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidateRolUsuarioRestoreBusinessRules(RolUsuario rolUsuario)
        {
            // Verificar que no exista otro rol activo con el mismo nombre
            var existingRoles = await _rolUsuarioRepository.GetAllAsync();
            if (existingRoles.Any(r => !r.Deleted &&
                r.Id != rolUsuario.Id &&
                r.Descripcion.Equals(rolUsuario.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["RolUsuario"]["DuplicateDescriptionRestore"]
                };
            }

            return new OperationResult { Success = true };
        }
    }
}