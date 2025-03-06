using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class ServiciosService : IServiciosService
    {
        private readonly IServiciosRepository _serviciosRepository;
        private readonly ILogger<ServiciosService> _logger;
        private readonly IConfiguration _configuration;

        public ServiciosService(IServiciosRepository serviciosRepository,
                                ILogger<ServiciosService> logger,
                                IConfiguration configuration)
        {
            _serviciosRepository = serviciosRepository;
            _logger = logger;
            _configuration = configuration;
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
                _logger.LogError(ex, "Error al obtener todos los servicios.");
                operationResult.Message = "Error al obtener todos los servicios";
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
                    operationResult.Message = "Servicio no encontrado";
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
                _logger.LogError(ex, "Error al obtener el servicio por ID.");
                operationResult.Message = "Error al obtener el servicio por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }
        public async Task<OperationResult> Save(SaveServiciosDto dto)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var servicio = new Servicios
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // Usar el usuario autenticado en producción
                };

                operationResult = await _serviciosRepository.SaveEntityAsync(servicio);
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:SaveError"];
                _logger.LogError(_configuration["LoggingMessages:SaveError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }


        public async Task<OperationResult> Update(UpdateServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult { Success = false, Message = "ID de servicio inválido." };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicio == null || servicio.Deleted)
                return new OperationResult { Success = false, Message = "Servicio no encontrado o eliminado." };

            servicio.Nombre = dto.Nombre ?? servicio.Nombre;
            servicio.Descripcion = dto.Descripcion ?? servicio.Descripcion;
            servicio.Estado = dto.Estado != default ? dto.Estado : servicio.Estado;
            servicio.ModifyDate = DateTime.Now;
            servicio.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _serviciosRepository.UpdateEntityAsync(servicio);
        }

        public async Task<OperationResult> Remove(RemoveServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult { Success = false, Message = "ID del Servicio es inválido." };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicio == null || servicio.Deleted)
                return new OperationResult { Success = false, Message = "Servicio no encontrado o ya eliminado." };

            servicio.Deleted = true;
            servicio.DeletedUser = 1;
            servicio.ModifyDate = DateTime.Now;

            return await _serviciosRepository.UpdateEntityAsync(servicio);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio == null || !servicio.Deleted)
                return new OperationResult { Success = false, Message = "Servicio no encontrado o ya activo." };

            servicio.Deleted = false;
            servicio.ModifyDate = DateTime.Now;
            servicio.ModifyUser = 1;

            return await _serviciosRepository.UpdateEntityAsync(servicio);
        }
    }
}
