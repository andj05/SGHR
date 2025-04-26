using SGHR.WebApi.Models;
using SGHR.WebApi.Models.RolUsuario;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class RolUsuarioService : IRolUsuarioService
    {
        private readonly IRepository<RolUsuarioApiModel> _rolUsuarioRepository;
        private readonly ILoggerManger<RolUsuarioService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public RolUsuarioService(IRepository<RolUsuarioApiModel> rolUsuarioRepository, ILoggerManger<RolUsuarioService> logger, IErrorMessageService errorMessageService)
        {
            _rolUsuarioRepository = rolUsuarioRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var rolUsuarios = await _rolUsuarioRepository.GetAllAsync();
                result.success = true;
                result.data = rolUsuarios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("EntityBase", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("EntityBase", "InvalidID");
                return result;
            }
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetByIdAsync(id);
                if (rolUsuario == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = rolUsuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(RolUsuarioApiModel dto)
        {
            var validationResult = ValidateRolUsuario(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var businessValidationResult = await ValidateRolUsuarioBusinessRules(dto);
            if (!businessValidationResult.success)
            {
                return businessValidationResult;
            }

            var result = new OperationResult();
            try
            {
                await _rolUsuarioRepository.AddAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed");
            }
            return result;
        }

        public async Task<OperationResult> Update(RolUsuarioApiModel dto)
        {
            var validationResult = ValidateRolUsuario(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var businessValidationResult = await ValidateRolUsuarioUpdateBusinessRules(dto);
            if (!businessValidationResult.success)
            {
                return businessValidationResult;
            }

            var result = new OperationResult();
            try
            {
                await _rolUsuarioRepository.UpdateAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "UpdateFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateFailed");
            }
            return result;
        }

        public async Task<OperationResult> Remove(RolUsuarioApiModel dto)
        {
            var result = new OperationResult();
            try
            {
                await _rolUsuarioRepository.DeleteAsync(dto.IdRolUsuario);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteFailed");
            }
            return result;
        }

        private OperationResult ValidateRolUsuario(dynamic rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("RolUsuario", "NullRole")
                };
            }

            if (string.IsNullOrWhiteSpace(rolUsuario.Descripcion))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("RolUsuario", "EmptyDescription")
                };
            }

            if (rolUsuario.Descripcion.Length > 50)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("RolUsuario", "DescriptionTooLong")
                };
            }

            return new OperationResult { success = true };
        }

        private async Task<OperationResult> ValidateRolUsuarioBusinessRules(RolUsuarioApiModel dto)
        {
            // Validar que no exista otro rol con la misma descripción
            var existingRoles = await _rolUsuarioRepository.GetAllAsync();
            if (existingRoles.Any(r => r.Estado != false &&
                r.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("RolUsuario", "DuplicateDescription")
                };
            }

            return new OperationResult { success = true };
        }

        private async Task<OperationResult> ValidateRolUsuarioUpdateBusinessRules(RolUsuarioApiModel dto)
        {
            // Validar que no exista otro rol con la misma descripción (excepto el mismo rol)
            var existingRoles = await _rolUsuarioRepository.GetAllAsync();
            if (existingRoles.Any(r => r.Estado != false &&
                r.IdRolUsuario != dto.IdRolUsuario &&
                r.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("RolUsuario", "DuplicateDescriptionUpdate")
                };
            }

            return new OperationResult { success = true };
        }
    }
}


