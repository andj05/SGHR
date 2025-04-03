using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Habitacion;
using SGHR.WebApi.PersistenceApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.ServicesApi.Mappers;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IRepository<HabitacionModel> _habitacionRepository;
        private readonly ILoggerManager<HabitacionService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public HabitacionService(IRepository<HabitacionModel> habitacionRepository, ILoggerManager<HabitacionService> logger, IErrorMessageService errorMessageService)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                var activeHabitaciones = habitaciones
                    .Where(h => h.Estado)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activeHabitaciones;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Generic", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("EntityBase", "InvalidID"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("EntityBase", "InvalidID");
                return result;
            }
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                if (habitacion == null || !habitacion.Estado)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("EntityBase", "NotFound")),
                        _errorMessageService.GetErrorMessage("EntityBase", "NotFound"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    return result;
                }
                result.Data = habitacion;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Generic", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(SaveHabitacionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = HabitacionMapper.ToHabitacionModel(dto);
                var validationResult = await ValidateHabitacionBusiness(habitacion);
                if (validationResult.Success != true)
                {
                    return validationResult;
                }

                await _habitacionRepository.AddAsync(habitacion);
                result.Success = true;
                result.Message = _errorMessageService.GetErrorMessage("SuccessMessages", "SaveSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed");
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateHabitacionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(dto.Id);
                if (habitacion == null || !habitacion.Estado)
                {
                    _logger.LogWarning(_errorMessageService.GetErrorMessage("EntityBase", "NotFound"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    return result;
                }

                habitacion.MapFromUpdateModel(dto);

                var validationResult = await ValidateHabitacionBusiness(habitacion);
                if (validationResult.Success != true)
                {
                    return validationResult;
                }

                await _habitacionRepository.UpdateAsync(habitacion);
                result.Success = true;
                result.Message = _errorMessageService.GetErrorMessage("SuccessMessages", "UpdateSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "UpdateFailed"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Operations", "UpdateFailed");
            }
            return result;
        }

        public async Task<OperationResult> Remove(RemoveHabitacionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(dto.Id);

                if (!habitacion.Estado)
                {
                    _logger.LogWarning(_errorMessageService.GetErrorMessage("Operations", "AlreadyDeleted"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("Operations", "AlreadyDeleted");
                    return result;
                }

                if (habitacion.IdEstadoHabitacion == 2)
                {
                    _logger.LogWarning(_errorMessageService.GetErrorMessage("Operations", "DeleteInProgress"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("Operations", "DeleteInProgress");
                    return result;
                }

                if (habitacion.IdEstadoHabitacion == 3)
                {
                    _logger.LogWarning(_errorMessageService.GetErrorMessage("Operations", "DeleteInProgress"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("Operations", "DeleteInProgress");
                    return result;
                }

                await _habitacionRepository.DeleteAsync(dto.Id);
                result.Success = true;
                result.Message = _errorMessageService.GetErrorMessage("SuccessMessages", "DeleteSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Operations", "DeleteFailed");
            }
            return result;
        }

        public async Task<OperationResult> GetForUpdate(int id)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                if (habitacion == null || !habitacion.Estado)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                result.Data = HabitacionMapper.ToUpdateModel(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Generic", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> GetForRemove(int id)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetByIdAsync(id);
                if (habitacion == null || !habitacion.Estado)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                result.Data = HabitacionMapper.ToRemoveModel(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Generic", "GenericError");
            }
            return result;
        }

        private async Task<OperationResult> ValidateHabitacionBusiness(HabitacionModel habitacion)
        {
            if (string.IsNullOrWhiteSpace(habitacion.Numero))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "MissingNumber")
                };
            }
            if (habitacion.Numero.Length > 50)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "NumberTooLong")
                };
            }
            if (habitacion.Detalle?.Length > 100)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "DetailTooLong")
                };
            }
            if (habitacion.IdEstadoHabitacion <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "InvalidStatusID")
                };
            }
            if (habitacion.IdPiso <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "InvalidFloorID")
                };
            }
            if (habitacion.IdCategoria <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "InvalidCategoryID")
                };
            }

            var habitacionesConNumero = await _habitacionRepository.GetAllAsync();
            if (habitacionesConNumero.Any(h => h.Numero == habitacion.Numero && h.Id != habitacion.Id))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Room", "DuplicateNumber")
                };
            }

            return new OperationResult { Success = true };
        }
    }
}