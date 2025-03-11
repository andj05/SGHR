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
                result.Data = recepciones.Where(r => !r.Deleted).ToList();
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
                result.Data = recepcion;
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
                var recepcion = new Recepcion
                {
                    IdCliente = dto.IdCliente,
                    IdHabitacion = dto.IdHabitacion,
                    IdEstadoReserva = dto.IdEstadoReserva,
                    FechaEntrada = dto.FechaEntrada,
                    FechaSalida = dto.FechaSalida,
                    FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion,
                    PrecioInicial = dto.PrecioInicial,
                    Adelanto = dto.Adelanto,
                    PrecioRestante = dto.PrecioRestante,
                    TotalPagado = dto.TotalPagado,
                    CostoPenalidad = dto.CostoPenalidad,
                    Observacion = dto.Observacion,
                    Deleted = false,
                    CreationUser = 1, //en produccion cambiar por el usuario actual
                    FechaCreacion = DateTime.Now
                };

                var validationResult = ValidateRecepcionBusiness(recepcion);
                if (!validationResult.Success.GetValueOrDefault())
                {
                    return validationResult;
                }

                var saveResult = await _recepcionRepository.SaveEntityAsync(recepcion);
                if (saveResult.Success != true)
                {
                    result.Success = false;
                    result.Message = saveResult.Message;
                }
                else
                {
                    result.Success = true;
                    result.Data = recepcion;
                }
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

                existingRecepcion.IdCliente = dto.IdCliente;
                existingRecepcion.IdHabitacion = dto.IdHabitacion;
                existingRecepcion.IdEstadoReserva = dto.IdEstadoReserva;
                existingRecepcion.FechaEntrada = dto.FechaEntrada;
                existingRecepcion.FechaSalida = dto.FechaSalida;
                existingRecepcion.FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion;
                existingRecepcion.PrecioInicial = dto.PrecioInicial;
                existingRecepcion.Adelanto = dto.Adelanto;
                existingRecepcion.PrecioRestante = dto.PrecioRestante;
                existingRecepcion.TotalPagado = dto.TotalPagado;
                existingRecepcion.CostoPenalidad = dto.CostoPenalidad;
                existingRecepcion.Observacion = dto.Observacion;
                existingRecepcion.ModifyDate = DateTime.Now;
                existingRecepcion.ModifyUser = 1; //en produccion cambiar por el usuario actual

                var validationResult = ValidateRecepcionBusiness(existingRecepcion);
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
                if (recepcion == null)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                if (recepcion.IdEstadoReserva == 2)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteInProgress"];
                    return result;
                }

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
                
                var existing = await _recepcionRepository.GetEntityByIdAsync(id);
                if (!existing.Deleted)
                {
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Reservation"]["AlreadyActive"];
                    return result;
                }

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

        
        public async Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReserva(int idEstadoReserva)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorEstadoReservaAsync(idEstadoReserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorClienteId(int idCliente)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorClienteIdAsync(idCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionId(int idHabitacion)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorHabitacionIdAsync(idHabitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorPrecioInicial(decimal precioInicial)
        {
            var result = new List<Recepcion>();
            try
            {
                if (precioInicial < 0)
                {
                    _logger.LogError(_messageMapper.ErrorMessages["Rate"]["InvalidPrice"]);
                    return result;
                }
                result = await _recepcionRepository.ObtenerRecepcionesPorPrecioInicialAsync(precioInicial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_messageMapper.ErrorMessages["Generic"]["GenericError"]} [Price: {precioInicial}]");
            }
            return result;
        }

        
        private OperationResult ValidateRecepcionBusiness(Recepcion recepcion)
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
            return new OperationResult { Success = true };
        }
    }
}