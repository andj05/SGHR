using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public EstadoHabitacionService(
            IEstadoHabitacionRepository estadoHabitacionRepository,
            MessageMapper messageMapper,
            ILoggerManager loggerManager)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var estadoHabitacion = await _estadoHabitacionRepository.GetAllAsync();
                var activeEstadoHabitacion = estadoHabitacion
                    .Where(t => !t.Deleted)
                    .Select(EstadoHabitacionMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activeEstadoHabitacion;
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
                var estadoHabitacion = await _estadoHabitacionRepository.GetAllAsync();
                var activeestadoHabitacion = estadoHabitacion
                    .Where(t => t.Deleted)
                    .Select(EstadoHabitacionMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activeestadoHabitacion;
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
                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estadoHabitacion == null || estadoHabitacion.Deleted)
                {
                    _loggerManager.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                result.Data = EstadoHabitacionMapper.ToDto(estadoHabitacion);
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

        public async Task<OperationResult> GetDeletedById(int id)
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
                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estadoHabitacion == null || !estadoHabitacion.Deleted)
                {
                    _loggerManager.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                result.Data = EstadoHabitacionMapper.ToDto(estadoHabitacion);
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

        public async Task<OperationResult> Save(SaveEstadoHabitacionDto dto)
        {
            // Validar los datos básicos del estado de habitación
            var validationResult = ValidateEstadoHabitacion(dto);
            if (validationResult.Success != true)
                return validationResult;

            // Validar que no exista un estado con la misma descripción
            var isDuplicateResult = await IsDuplicateDescripcion(dto.Descripcion);
            if (!isDuplicateResult.Success)
                return isDuplicateResult;

            try
            {
                var estadoHabitacion = EstadoHabitacionMapper.ToEntity(dto);

                // Validar nombre reservado o palabras clave del sistema
                if (IsReservedStateName(dto.Descripcion))
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Nombre de estado reservado para el sistema."
                    };
                }

                return await _estadoHabitacionRepository.SaveEntityAsync(estadoHabitacion);
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

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            var result = new OperationResult();
            try
            {
                if (dto.IdEstadoHabitacion <= 0)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                    return result;
                }

                var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estadoHabitacion == null || estadoHabitacion.Deleted)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }

                var validationResult = ValidateEstadoHabitacion(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                var isDuplicateResult = await IsDuplicateDescripcion(dto.Descripcion, dto.IdEstadoHabitacion);
                if (!isDuplicateResult.Success)
                {
                    return isDuplicateResult;
                }

                if (IsReservedStateName(dto.Descripcion))
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Nombre de estado reservado para el sistema."
                    };
                }

                estadoHabitacion.UpdateFromDto(dto);
                await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);

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


        public async Task<OperationResult> Remove(RemoveEstadoHabitacionDto dto)
        {
            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estado == null)
            {
                return new OperationResult { Success = false, Message = "EstadoHabitacion no encontrado" };
            }

            estado.Deleted = true;
            var result = await _estadoHabitacionRepository.UpdateEntityAsync(estado);
            return result;
        }

        public async Task<OperationResult> Restore(int id)
        {
            if (id <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estadoHabitacion == null || !estadoHabitacion.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            // Validar que no exista un estado activo con la misma descripción
            var isDuplicateResult = await IsDuplicateDescripcion(estadoHabitacion.Descripcion);
            if (!isDuplicateResult.Success)
                return isDuplicateResult;

            estadoHabitacion.RestoreFromDto(1);
            return await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);
        }

        // Validaciones de negocio
        private OperationResult ValidateEstadoHabitacion(dynamic estado)
        {
            if (estado == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["NullEstado"]
                };

            // Validación de la descripción
            if (string.IsNullOrWhiteSpace(estado.Descripcion))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["EmptyDescription"]
                };

            if (estado.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["DescriptionTooLong"]
                };

            // Validar que la descripción no contenga caracteres especiales no permitidos
            if (!IsValidDescripcion(estado.Descripcion))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["InvalidDescriptionCharacters"]
                };

            // Si el estado incluye un color, validar el formato
            if (estado.GetType().GetProperty("ColorHex") != null &&
                !string.IsNullOrEmpty(estado.ColorHex) &&
                !IsValidColorHex(estado.ColorHex))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["InvalidColorHex"]
                };
            }

            // Validar orden si está presente en el DTO
            if (estado.GetType().GetProperty("Orden") != null &&
                estado.Orden <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["InvalidOrder"]
                };
            }

            return new OperationResult { Success = true };
        }

        private bool IsValidDescripcion(string descripcion)
        {
            // Verificar que la descripción solo contiene letras, números, espacios y algunos caracteres permitidos
            return System.Text.RegularExpressions.Regex.IsMatch(
                descripcion,
                @"^[a-zA-Z0-9áéíóúüñÁÉÍÓÚÜÑ\s\-_\.,:;()]+$"
            );
        }

        private bool IsValidColorHex(string colorHex)
        {
            // Validar formato de color hexadecimal (#RRGGBB o #RGB)
            return System.Text.RegularExpressions.Regex.IsMatch(
                colorHex,
                @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"
            );
        }

        private async Task<OperationResult> IsDuplicateDescripcion(string descripcion, int? idExcluir = null)
        {
            try
            {
                var allEstadosHabitacion = await _estadoHabitacionRepository.GetAllAsync();
                bool isDuplicate = allEstadosHabitacion
                    .Where(e => !e.Deleted &&
                                e.Descripcion.Trim().ToLower() == descripcion.Trim().ToLower() &&
                                (idExcluir == null || e.Id != idExcluir))
                    .Any();

                if (isDuplicate)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["DuplicateDescription"]
                    };
                }

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, "Error al verificar duplicados de EstadoHabitacion");
                return new OperationResult
                {
                    Success = false,
                    Message = "Error al verificar duplicados: " + ex.Message
                };
            }
        }

        private bool IsReservedStateName(string descripcion)
        {
            // Lista de nombres reservados para estados del sistema
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
