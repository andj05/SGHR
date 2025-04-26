using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Piso;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class PisoService : IPisoService
    {
        private readonly IRepository<PisoApiModel> _pisoRepository;
        private readonly ILoggerManger<PisoService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public PisoService(IRepository<PisoApiModel> pisoRepository, ILoggerManger<PisoService> logger, IErrorMessageService errorMessageService)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                result.success = true;
                result.data = pisos;
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
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }
            try
            {
                var piso = await _pisoRepository.GetByIdAsync(id);
                if (piso == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = piso;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(PisoApiModel dto)
        {
            var result = ValidatePiso(dto);
            if (!result.success)
                return result;

            var businessValidationResult = await ValidatePisoBusinessRules(dto);
            if (!businessValidationResult.success)
                return businessValidationResult;

            try
            {
                await _pisoRepository.AddAsync(dto);
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

        public async Task<OperationResult> Update(PisoApiModel dto)
        {
            var result = new OperationResult();
            if (dto.IdPiso <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }

            var piso = await _pisoRepository.GetByIdAsync(dto.IdPiso);
            if (piso == null)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "NotFound"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                return result;
            }

            var validationResult = ValidatePiso(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var businessValidationResult = await ValidatePisoUpdateBusinessRules(dto, piso);
            if (!businessValidationResult.success)
            {
                return businessValidationResult;
            }

            try
            {
                await _pisoRepository.UpdateAsync(dto);
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

        public async Task<OperationResult> Remove(PisoApiModel dto)
        {
            var result = new OperationResult();
            if (dto.IdPiso <= 0)
            {
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }

            var piso = await _pisoRepository.GetByIdAsync(dto.IdPiso);
            if (piso == null)
            {
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                return result;
            }

            var removeValidationResult = await ValidatePisoRemoveBusinessRules(piso);
            if (!removeValidationResult.success)
                return removeValidationResult;

            try
            {
                await _pisoRepository.DeleteAsync(dto.IdPiso);
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

        private OperationResult ValidatePiso(PisoApiModel dto)
        {
            if (dto == null)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "NullPiso")
                };

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "EmptyDescription")
                };

            if (dto.Descripcion.Length > 50)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "DescriptionTooLong")
                };

            return new OperationResult { success = true };
        }

        private async Task<OperationResult> ValidatePisoBusinessRules(PisoApiModel dto)
        {
            var existingPisos = await _pisoRepository.GetAllAsync();
            if (existingPisos.Any(p => p.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "DuplicateDescription")
                };
            }

            if (existingPisos.Count() >= 100)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "MaxPisosExceeded")
                };
            }

            return new OperationResult { success = true };
        }

        private async Task<OperationResult> ValidatePisoUpdateBusinessRules(PisoApiModel dto, PisoApiModel existingPiso)
        {
            var existingPisos = await _pisoRepository.GetAllAsync();
            if (existingPisos.Any(p => p.IdPiso != dto.IdPiso && p.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "DuplicateDescriptionUpdate")
                };
            }

            if (IsSystemPiso(existingPiso))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "SystemPisoModification")
                };
            }

            return new OperationResult { success = true };
        }

        private async Task<OperationResult> ValidatePisoRemoveBusinessRules(PisoApiModel piso)
        {
            if (IsSystemPiso(piso))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Pisos", "SystemPisoDeletion")
                };
            }

            return new OperationResult { success = true };
        }

        private bool IsSystemPiso(PisoApiModel piso)
        {
            var systemPisoIds = new[] { 1 };
            var systemPisoNames = new[] { "Planta Principal" };

            return systemPisoIds.Contains(piso.IdPiso) || systemPisoNames.Contains(piso.Descripcion);
        }
    }
}
