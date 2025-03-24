using SGHR.Application.Dtos.Recepcion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;

namespace SGHR.Application.Services
{
    public class RecepcionService : IRecepcionService
    {
        private readonly IRecepcionRepository _recepcionRepository;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public RecepcionService(
            IRecepcionRepository recepcionRepository,
            ILoggerManager logger,
            MessageMapper messageMapper)
        {
            _recepcionRepository = recepcionRepository;
            _logger = logger;
            _messageMapper = messageMapper;
        }
        
        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var recepciones = await _recepcionRepository.GetAllAsync();
                // filtra entidades eliminadas
                var activeRecepciones = recepciones.Where(r => !r.Deleted)
                    .Select(RecepcionMapper.ToDto)
                    .OrderByDescending(r => r.ChangeDate)
                    .ToList();
                result.Data = activeRecepciones;
                result.Success = true;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        
        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                return result;
            }
            try
            {
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(id);
                if (recepcion == null || recepcion.Deleted)
                {
                    _logger.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }

                result.Success = true;
                result.Data = RecepcionMapper.ToDto(recepcion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        
        public async Task<OperationResult> Save(SaveRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = RecepcionMapper.ToEntity(dto);

                var validationResult = await ValidateRecepcionBusiness(recepcion);
                if (!validationResult.Success.GetValueOrDefault())
                {
                    return validationResult;
                }

                var saveResult = await _recepcionRepository.SaveEntityAsync(recepcion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"];
            }
            return result;
        }

        
        public async Task<OperationResult> Update(UpdateRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var existingRecepcion = await _recepcionRepository.GetEntityByIdAsync(dto.Id);
                if (existingRecepcion == null || existingRecepcion.Deleted)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }

                existingRecepcion.UpdateFromDto(dto);

                existingRecepcion.UpdateFromDto(dto);

                var validationResult = await ValidateRecepcionBusiness(existingRecepcion);
                if (!validationResult.Success.GetValueOrDefault())
                {
                    return validationResult;
                }

                var updateResult = await _recepcionRepository.UpdateEntityAsync(existingRecepcion);
                if (updateResult.Success != true)
                {
                    result.Success = false;
                    result.Message = updateResult.Message;
                    return result;
                }

                result.Success = true;
                result.Data = existingRecepcion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        
        public async Task<OperationResult> Remove(RemoveRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(dto.Id);
                if (recepcion.Deleted)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Operations"]["AlreadyDeleted"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["AlreadyDeleted"];
                    return result;
                }
                if (recepcion.IdEstadoReserva == 2)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteInProgress"];
                    return result;
                }
                if (recepcion.IdEstadoReserva == 3)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteInProgress"];
                    return result;
                }

                recepcion.RemoveFromDto(dto);
                var deleteResult = await _recepcionRepository.DeleteEntityAsync(recepcion);

                if (deleteResult.Success != true)
                {
                    result.Success = false;
                    result.Message = deleteResult.Message;
                    return result;
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Operations"]["DeleteFailed"]}: {ex.Message}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"];
            }
            return result;
        }

        
        public async Task<OperationResult> Restore(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                return result;
            }
            try
            {
                
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(id);
                if (!recepcion.Deleted)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Reservation"]["AlreadyActive"];
                    return result;
                }

                recepcion.RestoreFromDto(1);
                result = await _recepcionRepository.RestoreEntityAsync(id);
                if (result.Success!=true)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        
        public async Task<List<RecepcionDto>> ObtenerRecepcionesPorEstadoReserva(int idEstadoReserva)
        {
            try
            {
                if (idEstadoReserva <= 0)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Reservation"]["InvalidStatusID"]);
                    return new List<RecepcionDto>();
                }

                var recepciones = await _recepcionRepository.ObtenerRecepcionesPorEstadoReservaAsync(idEstadoReserva);
                return recepciones.Where(r => !r.Deleted).Select(RecepcionMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                return new List<RecepcionDto>();
            }
        }

        public async Task<List<RecepcionDto>> ObtenerRecepcionesPorClienteId(int idCliente)
        {
            try
            {
                if (idCliente <= 0)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Reservation"]["InvalidClientID"]);
                    return new List<RecepcionDto>();
                }

                var recepciones = await _recepcionRepository.ObtenerRecepcionesPorClienteIdAsync(idCliente);
                return recepciones.Where(r => !r.Deleted).Select(RecepcionMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                return new List<RecepcionDto>();
            }
        }

        public async Task<List<RecepcionDto>> ObtenerRecepcionesPorHabitacionId(int idHabitacion)
        {
            try
            {
                if (idHabitacion <= 0)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Reservation"]["InvalidRoomID"]);
                    return new List<RecepcionDto>();
                }

                var recepciones = await _recepcionRepository.ObtenerRecepcionesPorHabitacionIdAsync(idHabitacion);
                return recepciones.Where(r => !r.Deleted).Select(RecepcionMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                return new List<RecepcionDto>();
            }
        }

        public async Task<List<RecepcionDto>> ObtenerRecepcionesPorPrecioInicial(decimal precioInicial)
        {
            try
            {
                if (precioInicial < 0)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Rate"]["InvalidPrice"]);
                    return new List<RecepcionDto>();
                }

                var recepciones = await _recepcionRepository.ObtenerRecepcionesPorPrecioInicialAsync(precioInicial);
                return recepciones.Where(r => !r.Deleted).Select(RecepcionMapper.ToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                return new List<RecepcionDto>();
            }
        }

        
        public async Task<OperationResult> ValidateRecepcionBusiness(Recepcion recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"] };
            }
            if (recepcion.FechaEntrada == default)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["MissingEntryDate"] };
            }
            if (recepcion.IdCliente.HasValue && recepcion.IdCliente <= 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidClientID"] };
            }
            if (recepcion.IdHabitacion.HasValue && recepcion.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidRoomID"] };
            }
            if (recepcion.IdEstadoReserva.HasValue && recepcion.IdEstadoReserva <= 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidStatusID"] };
            }
            if (recepcion.Observacion != null && recepcion.Observacion.Length > 500)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["ObservationTooLong"] };
            }
            if (recepcion.Deleted)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidDeleteState"] };
            }
            if (recepcion.FechaCreacion == default)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["CreationDateRequired"] };
            }

            // business
            if (recepcion.FechaSalida.HasValue && recepcion.FechaSalida < recepcion.FechaEntrada)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidExitDate"] };
            }
            if (recepcion.FechaSalidaConfirmacion.HasValue && recepcion.FechaSalidaConfirmacion < recepcion.FechaEntrada)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidExitConfirmationDate"] };
            }

            
            if (recepcion.PrecioInicial.HasValue && recepcion.PrecioInicial < 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidPrice"] };
            }
            if (recepcion.Adelanto.HasValue && recepcion.Adelanto < 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidAdvance"] };
            }
            if (recepcion.PrecioRestante.HasValue && recepcion.PrecioRestante < 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidRemainingPrice"] };
            }
            if (recepcion.TotalPagado.HasValue && recepcion.TotalPagado < 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidTotalPaid"] };
            }
            if (recepcion.CostoPenalidad.HasValue && recepcion.CostoPenalidad < 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Reservation"]["InvalidPenaltyCost"] };
            }

            return new OperationResult { Success = true };
        }
    }
}