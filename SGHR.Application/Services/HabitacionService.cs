using SGHR.Application.Dtos.Habitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Repositories;

namespace SGHR.Application.Services
    {
        public class HabitacionService : IHabitacionService
        {
            private readonly IHabitacionRepository _habitacionRepository;
            private readonly ILoggerManager _logger;
            private readonly MessageMapper _messageMapper;

            public HabitacionService(
                IHabitacionRepository habitacionRepository,
                ILoggerManager logger,
                MessageMapper messageMapper)
            {
                _habitacionRepository = habitacionRepository;
                _logger = logger;
                _messageMapper = messageMapper;
            }

            
            public async Task<OperationResult> GetAll()
            {
                var result = new OperationResult();
                try
                {
                    var habitaciones = await _habitacionRepository.GetAllAsync();
                    // filtra entidades eliminadas
                    var activeHabitaciones = habitaciones.Where(h => !h.Deleted).ToList();
                    result.Data = activeHabitaciones;
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
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                if (habitacion == null || habitacion.Deleted)
                {
                    _logger.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                result.Success = true;
                result.Data = habitacion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

            public async Task<OperationResult> Save(SaveHabitacionDto dto)
            {
                var result = new OperationResult();
                try
                {
                    var habitacion = new Habitacion
                    {
                        IdPiso = dto.IdPiso,
                        IdCategoria = dto.IdCategoria,
                        Numero = dto.Numero,
                        Detalle = dto.Detalle,
                        IdEstadoHabitacion = dto.IdEstadoHabitacion,
                        FechaCreacion = DateTime.Now,
                        Deleted = false
                    };

                // validacion de reglas de negocio
                var validationResult = ValidateHabitacionBusiness(habitacion);
                    if (!validationResult.Success.GetValueOrDefault())
                    {
                        return validationResult;
                    }

                    await _habitacionRepository.SaveEntityAsync(habitacion);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Operations"]["SaveFailed"]}: {ex}");
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"];
                }
                return result;
            }

            public async Task<OperationResult> Update(UpdateHabitacionDto dto)
            {
                var result = new OperationResult();
                try
                {
                    var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                    if (habitacion.Deleted)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                        result.Success = false;
                        result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                        return result;
                    }

                    habitacion.IdPiso = dto.IdPiso;
                    habitacion.IdCategoria = dto.IdCategoria;
                    habitacion.Numero = dto.Numero;
                    habitacion.Detalle = dto.Detalle;
                    habitacion.IdEstadoHabitacion = dto.IdEstadoHabitacion;

                    var validationResult = ValidateHabitacionBusiness(habitacion);
                    if (!validationResult.Success.GetValueOrDefault())
                    {
                        return validationResult;
                    }

                    await _habitacionRepository.UpdateEntityAsync(habitacion);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex}");
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
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
                // obtiene la entidad por id
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);

                // confirma que la habitacion se encuentre eliminada
                if (!habitacion.Deleted)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Reservation"]["AlreadyActive"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Reservation"]["AlreadyActive"];
                    return result;
                }

                // llama al metodo del repositorio
                result = await _habitacionRepository.RestoreEntityAsync(id);

                if (result.Success!=true)
                {
                    _logger.LogWarn(_messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex.Message}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }

            return result;
        }

            public async Task<OperationResult> Remove(RemoveHabitacionDto dto)
            {
                var result = new OperationResult();
                try
                {
                    var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.Id);

                    // verifica que el numero no este borrado
                    if (habitacion.Deleted)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Operations"]["AlreadyDeleted"]);
                        result.Success = false;
                        result.Message = _messageMapper.ErrorMessages["Operations"]["AlreadyDeleted"];
                        return result;
                    }

                    // confirmar que la habitacion no esta ocupada
                    if (habitacion.IdEstadoHabitacion == 2)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Operations"]["DeleteInProgress"]);
                        result.Success = false;
                        result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteInProgress"];
                        return result;
                    }

                    result = await _habitacionRepository.DeleteEntityAsync(habitacion);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Operations"]["DeleteFailed"]}: {ex}");
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"];
                }
                return result;
            }

            public async Task<List<Habitacion>> ObtenerHabitacionesPorEstadoId(int idEstadoHabitacion)
            {
                try
                {
                    if (idEstadoHabitacion <= 0)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Room"]["InvalidStatusID"]);
                        return new List<Habitacion>();
                    }

                    var habitaciones = await _habitacionRepository.ObtenerHabitacionesPorEstadoIdAsync(idEstadoHabitacion);
                    return habitaciones.Where(h => !h.Deleted).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                    return new List<Habitacion>();
                }
            }

            public async Task<List<Habitacion>> ObtenerHabitacionesPorNumero(string numero)
            {
                try
                {
                    if (string.IsNullOrEmpty(numero))
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Room"]["MissingNumber"]);
                        return new List<Habitacion>();
                    }
                    if (numero.Length > 50)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Room"]["NumberTooLong"]);
                        return new List<Habitacion>();
                    }

                    var habitaciones = await _habitacionRepository.ObtenerHabitacionesPorNumeroAsync(numero);
                    return habitaciones.Where(h => !h.Deleted).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                    return new List<Habitacion>();
                }
            }

            public async Task<List<Habitacion>> ObtenerHabitacionesPorPisoId(int idPiso)
            {
                try
                {
                    if (idPiso <= 0)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Room"]["InvalidFloorID"]);
                        return new List<Habitacion>();
                    }

                    var habitaciones = await _habitacionRepository.ObtenerHabitacionesPorPisoIdAsync(idPiso);
                    return habitaciones.Where(h => !h.Deleted).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                    return new List<Habitacion>();
                }
            }

            public async Task<List<Habitacion>> ObtenerHabitacionesPorCategoriaId(int idCategoria)
            {
                try
                {
                    if (idCategoria <= 0)
                    {
                        _logger.LogWarn(_messageMapper.ErrorMessages["Room"]["InvalidCategoryID"]);
                        return new List<Habitacion>();
                    }

                    var habitaciones = await _habitacionRepository.ObtenerHabitacionesPorCategoriaIdAsync(idCategoria);
                    return habitaciones.Where(h => !h.Deleted).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                    return new List<Habitacion>();
                }
            }

            private OperationResult ValidateHabitacionBusiness(Habitacion habitacion)
            {
                if (string.IsNullOrWhiteSpace(habitacion.Numero))
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["MissingNumber"]
                    };
                }
                if (habitacion.Numero.Length > 50)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["NumberTooLong"]
                    };
                }
                if (habitacion.Detalle?.Length > 100)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["DetailTooLong"]
                    };
                }
                if (habitacion.IdEstadoHabitacion <= 0)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["InvalidStatusID"]
                    };
                }
                if (habitacion.IdPiso <= 0)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["InvalidFloorID"]
                    };
                }
                if (habitacion.IdCategoria <= 0)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Room"]["InvalidCategoryID"]
                    };
                }
                return new OperationResult { Success = true };
            }
        }
    }