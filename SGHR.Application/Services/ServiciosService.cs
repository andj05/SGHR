using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Dtos.Tarifas;
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                operationResult.Message = "Error al obtener el servicio por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveServiciosDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var servicio = new Servicios
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1
                };
                operationResult = await _serviciosRepository.SaveEntityAsync(servicio);
            }
            catch (Exception)
            {
                operationResult.Message = "Error al guardar el servicio";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdateServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult { Success = false, Message = "ID de servicio inválido." };

            var servicioResult = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicioResult?.Data is not Servicios servicio || servicio.Deleted)
                return new OperationResult { Success = false, Message = "Servicio no encontrado o eliminado." };

            servicio.Nombre = dto.Nombre ?? servicio.Nombre;
            servicio.Descripcion = dto.Descripcion ?? servicio.Descripcion;
            servicio.Estado = dto.Estado != default ? dto.Estado : servicio.Estado;
            servicio.ModifyDate = DateTime.Now;
            servicio.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _serviciosRepository.UpdateEntityAsync(servicio);
        }


        public async Task<OperationResult> Remove(RemoveServiciosDto dto)
        {
            if (dto.IdServicio <= 0)
                return new OperationResult { Success = false, Message = "ID del Servicio es  inválido." };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
            if (servicio.Data is not Servicios servicioData || servicioData.Deleted)
                return new OperationResult { Success = false, Message = "Servico no encontrado o ya eliminado." };

            servicioData.Deleted = true;
            servicioData.DeletedUser = 1;
            servicioData.ModifyDate = DateTime.Now;

            return await _serviciosRepository.UpdateEntityAsync(servicioData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio.Data is not Servicios servicioData || !servicioData.Deleted)
                return new OperationResult { Success = false, Message = "Servicio no encontrado o ya activo." };

            servicioData.Deleted = false;
            servicioData.ModifyDate = DateTime.Now;
            servicioData.ModifyUser = 1;

            return await _serviciosRepository.UpdateEntityAsync(servicioData);
        }

        public async Task<OperationResult> DeletePermanent(int id)
        {
            if (id <= 0)
                return new OperationResult { Success = false, Message = "ID de cliente inválido." };

            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio.Data is not Servicios servicioData)
                return new OperationResult { Success = false, Message = "Cliente no encontrado." };

            return await _serviciosRepository.DeleteEntityAsync(id);
        }

    }
}

