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

        public async Task<IEnumerable<Servicios>> ObtenerTodosLosServiciosAsync()
        {
            return await _serviciosRepository.ObtenerTodosLosServiciosAsync();
        }

        public async Task<bool> ActualizarEstadoServicioAsync(int idServicio, bool estado)
        {
            return await _serviciosRepository.ActualizarEstadoServicioAsync(idServicio, estado);
        }

        public async Task<bool> ExisteServicioAsync(int idServicio)
        {
            return await _serviciosRepository.ExisteServicioAsync(idServicio);
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var servicios = await _serviciosRepository.ObtenerTodosLosServiciosAsync();
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
                    Estado = dto.Estado
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
            var operationResult = new OperationResult();
            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
                if (servicio == null)
                {
                    operationResult.Message = "Servicio no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    servicio.Nombre = dto.Nombre;
                    servicio.Descripcion = dto.Descripcion;
                    servicio.Estado = dto.Estado;
                    operationResult = await _serviciosRepository.UpdateEntityAsync(servicio);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al actualizar el servicio";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveServiciosDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(dto.IdServicio);
                if (servicio == null)
                {
                    operationResult.Message = "Servicio no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _serviciosRepository.DeleteEntityAsync(servicio);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al eliminar el servicio";
                operationResult.Success = false;
            }
            return operationResult;
        }
    }
}

