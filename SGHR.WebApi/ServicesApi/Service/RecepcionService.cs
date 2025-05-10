using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Recepcion;
using SGHR.WebApi.PersistenceApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.ServicesApi.Mappers;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class RecepcionService : IRecepcionService
    {
        private readonly IRepository<RecepcionModel> _recepcionRepository;
        private readonly ILoggerManager<RecepcionService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public RecepcionService(IRepository<RecepcionModel> recepcionRepository, ILoggerManager<RecepcionService> logger, IErrorMessageService errorMessageService)
        {
            _recepcionRepository = recepcionRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var recepciones = await _recepcionRepository.GetAllAsync();
                result.Success = true;
                result.Data = recepciones.OrderByDescending(r => r.ChangeDate).ToList();
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
            try
            {
                var recepcion = await _recepcionRepository.GetByIdAsync(id);
                if (recepcion == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("EntityBase", "NotFound")),
                        _errorMessageService.GetErrorMessage("EntityBase", "NotFound"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    return result;
                }

                result.Success = true;
                result.Data = recepcion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                result.Success = false;
                result.Message = _errorMessageService.GetErrorMessage("Generic", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(SaveRecepcionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = RecepcionMapper.ToRecepcionModel(dto);

                var validationResult = await ValidateRecepcionBusiness(recepcion);
                if (validationResult.Success != true)
                {
                    return validationResult;
                }

                await _recepcionRepository.AddAsync(recepcion);
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

        public async Task<OperationResult> Update(UpdateRecepcionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetByIdAsync(dto.Id);
                if (recepcion == null)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                recepcion.MapFromUpdateModel(dto);

                var validationResult = await ValidateRecepcionBusiness(recepcion);
                if (validationResult.Success != true)
                {
                    return validationResult;
                }

                await _recepcionRepository.UpdateAsync(recepcion);
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

        public async Task<OperationResult> Remove(RemoveRecepcionModel dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetByIdAsync(dto.Id);
                if (recepcion == null)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                if (!recepcion.Estado)
                {
                    _logger.LogWarning(_errorMessageService.GetErrorMessage("Operations", "AlreadyDeleted"));
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("Operations", "AlreadyDeleted");
                    return result;
                }

                if (recepcion.IdEstadoReserva == 2 || recepcion.IdEstadoReserva == 3)
                {
                    result.Success = false;
                    result.Message = _errorMessageService.GetErrorMessage("Operations", "DeleteInProgress");
                    return result;
                }

                await _recepcionRepository.DeleteAsync(recepcion.Id);
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
                var recepcion = await _recepcionRepository.GetByIdAsync(id);
                if (recepcion == null)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                result.Data = RecepcionMapper.ToUpdateModel(recepcion);
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
                var recepcion = await _recepcionRepository.GetByIdAsync(id);
                if (recepcion == null)
                {
                    var notFoundMessage = _errorMessageService.GetErrorMessage("EntityBase", "NotFound");
                    _logger.LogError(new Exception(notFoundMessage), notFoundMessage);
                    result.Success = false;
                    result.Message = notFoundMessage;
                    return result;
                }

                result.Data = RecepcionMapper.ToRemoveModel(recepcion);
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

        private async Task<OperationResult> ValidateRecepcionBusiness(RecepcionModel recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            if (recepcion.FechaEntrada == default)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "MissingEntryDate")
                };
            }
            if (recepcion.IdCliente.HasValue && recepcion.IdCliente <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidClientID")
                };
            }
            if (recepcion.IdHabitacion.HasValue && recepcion.IdHabitacion <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidRoomID")
                };
            }
            if (recepcion.IdEstadoReserva.HasValue && recepcion.IdEstadoReserva <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidStatusID")
                };
            }
            if (recepcion.Observacion != null && recepcion.Observacion.Length > 500)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "ObservationTooLong")
                };
            }
            if (recepcion.FechaSalida.HasValue && recepcion.FechaSalida < recepcion.FechaEntrada)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidExitDate")
                };
            }
            if (recepcion.FechaSalidaConfirmacion.HasValue && recepcion.FechaSalidaConfirmacion < recepcion.FechaEntrada)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidExitConfirmationDate")
                };
            }
            if (recepcion.PrecioInicial.HasValue && recepcion.PrecioInicial < 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidPrice")
                };
            }
            if (recepcion.Adelanto.HasValue && recepcion.Adelanto < 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidAdvance")
                };
            }
            if (recepcion.PrecioRestante.HasValue && recepcion.PrecioRestante < 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidRemainingPrice")
                };
            }
            if (recepcion.TotalPagado.HasValue && recepcion.TotalPagado < 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidTotalPaid")
                };
            }
            if (recepcion.CostoPenalidad.HasValue && recepcion.CostoPenalidad < 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("Reservation", "InvalidPenaltyCost")
                };
            }
            return new OperationResult { Success = true };
        }
    }
}
