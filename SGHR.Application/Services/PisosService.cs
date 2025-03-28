using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
namespace SGHR.Application.Services
{
    public class PisosService : IPisosService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public PisosService(IPisoRepository pisoRepository,
                            IConfiguration configuration,
                            MessageMapper messageMapper,
                            ILoggerManager loggerManager)
        {
            _pisoRepository = pisoRepository;
            _configuration = configuration;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                var activePisos = pisos
                    .Where(t => !t.Deleted)
                    .Select(PisoMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activePisos;
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

        public async Task<OperationResult> GetAllDelete()
        {
            var result = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetAllAsync();
                var deletedPisos = piso
                    .Where(t => t.Deleted)
                    .Select(PisoMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = deletedPisos;
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
            var result = new OperationResult();
            if (id <= 0)
            {
                _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                return result;
            }
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso == null || piso.Deleted)
                {
                    _loggerManager.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                result.Data = PisoMapper.ToDto(piso);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        public async Task<OperationResult> Save(SavePisosDto dto)
        {
            // Validaciones básicas del objeto
            var validationResult = ValidatePiso(dto);
            if (validationResult.Success != true)
                return validationResult;

            // Validaciones de negocio específicas para guardar
            var businessValidationResult = await ValidatePisoBusinessRules(dto);
            if (businessValidationResult.Success != true)
                return businessValidationResult;

            try
            {
                var piso = PisoMapper.ToEntity(dto);
                return await _pisoRepository.SaveEntityAsync(piso);
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
        public async Task<OperationResult> Update(UpdatePisosDto dto)
        {
            var result = new OperationResult();
            try
            {
                if (dto.IdPiso <= 0)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                    return result;
                }

                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null || piso.Deleted)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }

                var validationResult = ValidatePiso(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                var businessValidationResult = await ValidatePisoUpdateBusinessRules(dto, piso);
                if (!businessValidationResult.Success)
                {
                    return businessValidationResult;
                }

                piso.UpdateFromDto(dto);
                await _pisoRepository.UpdateEntityAsync(piso);

                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }


        public async Task<OperationResult> Remove(RemovePisosDto dto)
        {
            if (dto.IdPiso <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
            if (piso == null || piso.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            // Validación de negocio para eliminar
            var removeValidationResult = await ValidatePisoRemoveBusinessRules(piso);
            if (removeValidationResult.Success != true)
                return removeValidationResult;

            piso.Deleted = true;
            piso.ModifyDate = DateTime.Now;
            return await _pisoRepository.UpdateEntityAsync(piso);
        }


        public async Task<OperationResult> Restore(int id)
        {
            var piso = await _pisoRepository.GetEntityByIdAsync(id);
            if (piso == null || !piso.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            // Validación de negocio para restaurar
            var restoreValidationResult = await ValidatePisoRestoreBusinessRules(piso);
            if (restoreValidationResult.Success != true)
                return restoreValidationResult;

            piso.Deleted = false;
            piso.ModifyDate = DateTime.Now;
            piso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        // Validaciones básicas de datos
        private OperationResult ValidatePiso(dynamic dto)
        {
            if (dto == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["NullPiso"]
                };

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["EmptyDescription"]
                };

            if (dto.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["DescriptionTooLong"]
                };

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidatePisoBusinessRules(SavePisosDto dto)
        {
            // Validar que no exista otro piso con la misma descripción
            var existingPisos = await _pisoRepository.GetAllAsync();
            if (existingPisos.Any(p => !p.Deleted &&
                p.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["DuplicateDescription"]
                };
            }

            // Validar que no exceda el número máximo de pisos permitidos
            if (existingPisos.Count(p => !p.Deleted) >= 100) // Supongamos un límite de 100 pisos activos
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["MaxPisosExceeded"]
                };
            }

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidatePisoUpdateBusinessRules(UpdatePisosDto dto, Piso existingPiso)
        {
            // Validar que no exista otro piso con la misma descripción (excepto el mismo piso)
            var existingPisos = await _pisoRepository.GetAllAsync();
            if (existingPisos.Any(p => !p.Deleted &&
                p.Id != dto.IdPiso &&
                p.Descripcion.Equals(dto.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["DuplicateDescriptionUpdate"]
                };
            }

            // Verificar si es un piso del sistema o predeterminado (si aplica)
            if (IsSystemPiso(existingPiso))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["SystemPisoModification"]
                };
            }

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidatePisoRemoveBusinessRules(Piso piso)
        {
            // No permitir eliminar pisos del sistema o predeterminados
            if (IsSystemPiso(piso))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["SystemPisoDeletion"]
                };
            }

            // Verificar si el piso está en uso (por ejemplo, si hay habitaciones asociadas)
            var isInUse = await IsPisoInUse(piso.Id);
            if (isInUse)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["PisoInUse"]
                };
            }

            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> ValidatePisoRestoreBusinessRules(Piso piso)
        {
            // Verificar que no exista otro piso activo con el mismo nombre
            var existingPisos = await _pisoRepository.GetAllAsync();
            if (existingPisos.Any(p => !p.Deleted &&
                p.Id != piso.Id &&
                p.Descripcion.Equals(piso.Descripcion, StringComparison.OrdinalIgnoreCase)))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["DuplicateDescriptionRestore"]
                };
            }

            // Verificar que no exceda el límite de pisos activos
            if (existingPisos.Count(p => !p.Deleted) >= 100) // Supongamos un límite de 100 pisos activos
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Pisos"]["MaxPisosExceeded"]
                };
            }

            return new OperationResult { Success = true };
        }


        // Función para verificar si un piso es del sistema o predeterminado
        private bool IsSystemPiso(Piso piso)
        {
            // Implementar lógica de identificación de pisos del sistema
            // Por ejemplo, pisos con IDs específicos o con nombres específicos
            var systemPisoIds = new[] { 1 }; // IDs hipotéticos de pisos del sistema
            var systemPisoNames = new[] { "Planta Principal" }; // Nombres hipotéticos

            return systemPisoIds.Contains(piso.Id) ||
                   systemPisoNames.Contains(piso.Descripcion);
        }

        // Método para verificar si un piso está en uso
        private async Task<bool> IsPisoInUse(int pisoId)
        {
            return false; 
        }
    }
}