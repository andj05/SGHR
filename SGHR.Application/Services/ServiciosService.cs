using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;

namespace SGHR.Application.Services
{
    public class ServiciosService : IServiciosService
    {
        private readonly IServiciosRepository _serviciosRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public ServiciosService(IServiciosRepository serviciosRepository,
                                MessageMapper messageMapper,
                                ILoggerManager loggerManager)
        {
            _serviciosRepository = serviciosRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var servicios = await _serviciosRepository.GetAllAsync();
                operationResult.Data = servicios;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
                if (servicio == null)
                {
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = servicio;
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

        public async Task<OperationResult> Save(SaveServiciosDto dto)
        {
            var validation = ValidateServicios(dto);
            if (validation.Success != true)
                return validation;

            try
            {
                var servicio = new Servicios
                {
                    FechaCreacion = DateTime.UtcNow,
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1
                };

                return await _serviciosRepository.SaveEntityAsync(servicio);
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

        public async Task<OperationResult> Update(UpdateServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicio == null || servicio.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            servicio.Nombre = dto.Nombre ?? servicio.Nombre;
            servicio.Descripcion = dto.Descripcion ?? servicio.Descripcion;
            servicio.Estado = dto.Estado;
            servicio.ModifyDate = DateTime.Now;
            servicio.ModifyUser = 1; // En producción, obtener el usuario autenticado

            var result = await _serviciosRepository.UpdateEntityAsync(servicio);
            return result;
        }

        public async Task<OperationResult> Remove(RemoveServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicio == null || servicio.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            servicio.Deleted = true;
            servicio.DeletedUser = 1;
            servicio.ModifyDate = DateTime.Now;

           var result = await _serviciosRepository.UpdateEntityAsync(servicio);
            return result;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio == null || !servicio.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            servicio.Deleted = false;
            servicio.ModifyDate = DateTime.Now;
            servicio.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _serviciosRepository.UpdateEntityAsync(servicio);
            return result;
        }

        private OperationResult ValidateServicios(dynamic service)
        {
            if (service == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Servicios"]["NullServicio"]
                };
            }

            if (string.IsNullOrWhiteSpace(service.Nombre))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Servicios"]["MissingName"]
                };
            }

            if (string.IsNullOrWhiteSpace(service.Descripcion) || service.Descripcion.Length > 255)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Servicios"]["InvalidDescription"]
                };
            }

            return new OperationResult { Success = true };
        }
    }
}