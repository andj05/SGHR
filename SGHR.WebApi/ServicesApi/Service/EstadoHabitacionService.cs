using SGHR.WebApi.Models;
using SGHR.WebApi.Models.EstadoHabitacion;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IRepository<EstadoHabitacionApiModel> _estadoHabitacionRepository;
        private readonly ILoggerManger<EstadoHabitacionService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public EstadoHabitacionService(IRepository<EstadoHabitacionApiModel> estadoHabitacionRepository, ILoggerManger<EstadoHabitacionService> logger, IErrorMessageService errorMessageService)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var estados = await _estadoHabitacionRepository.GetAllAsync();
                result.success = true;
                result.data = estados;
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
                var estado = await _estadoHabitacionRepository.GetByIdAsync(id);
                if (estado == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = estado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(EstadoHabitacionApiModel dto)
        {
            var validationResult = ValidateEstadoHabitacion(dto);
            if (!validationResult.success)
                return validationResult;

            var isDuplicateResult = await IsDuplicateDescripcion(dto.Descripcion);
            if (!isDuplicateResult.success)
                return isDuplicateResult;

            try
            {
                if (IsReservedStateName(dto.Descripcion))
                {
                    return new OperationResult
                    {
                        success = false,
                        message = "Nombre de estado reservado para el sistema."
                    };
                }

                await _estadoHabitacionRepository.AddAsync(dto);
                return new OperationResult
                {
                    success = true,
                    message = _errorMessageService.GetErrorMessage("Operations", "SaveSuccess")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed")
                };
            }
        }

        public async Task<OperationResult> Update(EstadoHabitacionApiModel dto)
        {
            var result = new OperationResult();
            if (dto.IdEstadoHabitacion <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }

            var estado = await _estadoHabitacionRepository.GetByIdAsync(dto.IdEstadoHabitacion);
            if (estado == null)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "NotFound"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                return result;
            }

            var validationResult = ValidateEstadoHabitacion(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var isDuplicateResult = await IsDuplicateDescripcion(dto.Descripcion, dto.IdEstadoHabitacion);
            if (!isDuplicateResult.success)
            {
                return isDuplicateResult;
            }

            if (IsReservedStateName(dto.Descripcion))
            {
                return new OperationResult
                {
                    success = false,
                    message = "Nombre de estado reservado para el sistema."
                };
            }

            try
            {
                await _estadoHabitacionRepository.UpdateAsync(dto);
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

        public async Task<OperationResult> Remove(EstadoHabitacionApiModel dto)
        {
            var result = new OperationResult();
            try
            {
                await _estadoHabitacionRepository.DeleteAsync(dto.IdEstadoHabitacion);
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

        private OperationResult ValidateEstadoHabitacion(EstadoHabitacionApiModel estado)
        {
            if (estado == null)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EstadoHabitacion", "NullEstado")
                };

            if (string.IsNullOrWhiteSpace(estado.Descripcion))
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EstadoHabitacion", "EmptyDescription")
                };

            if (estado.Descripcion.Length > 50)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EstadoHabitacion", "DescriptionTooLong")
                };

            if (!IsValidDescripcion(estado.Descripcion))
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EstadoHabitacion", "InvalidDescriptionCharacters")
                };

            return new OperationResult { success = true };
        }

        private bool IsValidDescripcion(string descripcion)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                descripcion,
                @"^[a-zA-Z0-9áéíóúüñÁÉÍÓÚÜÑ\s\-_\.,:;()]+$"
            );
        }

        private async Task<OperationResult> IsDuplicateDescripcion(string descripcion, int? idExcluir = null)
        {
            try
            {
                var allEstadosHabitacion = await _estadoHabitacionRepository.GetAllAsync();
                bool isDuplicate = allEstadosHabitacion
                    .Where(e => e.Descripcion.Trim().ToLower() == descripcion.Trim().ToLower() &&
                                (idExcluir == null || e.IdEstadoHabitacion != idExcluir))
                    .Any();

                if (isDuplicate)
                {
                    return new OperationResult
                    {
                        success = false,
                        message = _errorMessageService.GetErrorMessage("EstadoHabitacion", "DuplicateDescription")
                    };
                }

                return new OperationResult { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar duplicados de EstadoHabitacion");
                return new OperationResult
                {
                    success = false,
                    message = "Error al verificar duplicados: " + ex.Message
                };
            }
        }

        private bool IsReservedStateName(string descripcion)
        {
            string[] reservedNames = new string[]
            {
                "ocupado", "disponible", "mantenimiento", "limpieza", "reservado",
                "bloqueado"
            };

            return reservedNames.Any(name =>
                descripcion.Trim().ToLower() == name ||
                descripcion.Trim().ToLower().StartsWith($"{name}_"));
        }
    }
}
