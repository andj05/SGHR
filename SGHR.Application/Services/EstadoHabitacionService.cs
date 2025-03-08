using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;

namespace SGHR.Application.Services
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public EstadoHabitacionService(IEstadoHabitacionRepository estadoHabitacionRepository,
                                       MessageMapper messageMapper,
                                       ILoggerManager loggerManager)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var estados = await _estadoHabitacionRepository.GetAllAsync();
                operationResult.Success = true;
                operationResult.Data = estados;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var categoria = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (categoria == null)
                {
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = categoria;
                    operationResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveEstadoHabitacionDto dto)
        {
            var validationResult = ValidateEstadoHabitacion(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var estado = new EstadoHabitacion
                {
                    Descripcion = dto.Descripcion,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = dto.Estado,
                    CreationUser = 1
                };

                return await _estadoHabitacionRepository.SaveEntityAsync(estado);
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

            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estadoHabitacion == null || estadoHabitacion.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            estadoHabitacion.Descripcion = dto.Descripcion ?? estadoHabitacion.Descripcion;
            estadoHabitacion.Estado = dto.Estado;
            estadoHabitacion.FechaCreacion = dto.FechaCreacion != default ? dto.FechaCreacion : estadoHabitacion.FechaCreacion;
            estadoHabitacion.ModifyDate = DateTime.Now;
            estadoHabitacion.ModifyUser = 1;

            var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);
            return result;
        }

        public async Task<OperationResult> Remove(RemoveEstadoHabitacionDto dto)
        {

            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estadoHabitacion == null || estadoHabitacion.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            estadoHabitacion.Deleted = true;
            estadoHabitacion.DeletedUser = 1;
            estadoHabitacion.ModifyDate = DateTime.Now; estadoHabitacion.Deleted = true;
            estadoHabitacion.DeletedUser = 1;
            estadoHabitacion.ModifyDate = DateTime.Now;

            var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);
            return result;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estadoHabitacion == null || !estadoHabitacion.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            estadoHabitacion.Deleted = false;
            estadoHabitacion.ModifyDate = DateTime.Now;
            estadoHabitacion.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _estadoHabitacionRepository.UpdateEntityAsync(estadoHabitacion);
            return result;
        }

        private OperationResult ValidateEstadoHabitacion(dynamic estado)
        {
            if (estado == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["NullEstado"]
                };

            if (string.IsNullOrWhiteSpace(estado.Descripcion) || estado.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EstadoHabitacion"]["InvalidDescripcion"]
                };

            return new OperationResult { Success = true };
        }
    }
}