using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Servicios;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using System.Text.RegularExpressions;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class ServiciosService : IServiciosService
    {
        private readonly IRepository<ServiciosApiModel> _serviciosRepository;
        private readonly ILoggerManger<ServiciosService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public ServiciosService(IRepository<ServiciosApiModel> serviciosRepository, ILoggerManger<ServiciosService> logger, IErrorMessageService errorMessageService)
        {
            _serviciosRepository = serviciosRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var servicios = await _serviciosRepository.GetAllAsync();
                result.success = true;
                result.data = servicios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("EntityBase", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("EntityBase", "InvalidID");
                return result;
            }
            try
            {
                var servicio = await _serviciosRepository.GetByIdAsync(id);
                if (servicio == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = servicio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(ServiciosApiModel dto)
        {
            var validationResult = await ValidateServicios(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                await _serviciosRepository.AddAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed");
            }
            return result;
        }

        public async Task<OperationResult> Update(ServiciosApiModel dto)
        {
            var validationResult = await ValidateServicios(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                await _serviciosRepository.UpdateAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "UpdateFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateFailed");
            }
            return result;
        }

        public async Task<OperationResult> Remove(ServiciosApiModel dto)
        {
            var result = new OperationResult();
            try
            {
                await _serviciosRepository.DeleteAsync(dto.IdServicio);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteFailed");
            }
            return result;
        }

        private async Task<OperationResult> ValidateServicios(dynamic service)
        {
            if (service == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "EmptyService")
                };
            }

            if (string.IsNullOrWhiteSpace(service.Nombre))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "EmptyName")
                };
            }

            if (string.IsNullOrWhiteSpace(service.Descripcion) || service.Descripcion.Length > 255)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "InvalidDescriptionLength")
                };
            }

            // Validaciones de negocio

            // 1. Validar longitud mínima del nombre (regla de negocio)
            if (service.Nombre.Length < 10)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "ShortName")
                };
            }

            // 2. Validar que el nombre no contenga caracteres especiales o símbolos inapropiados
            if (!IsValidServiceName(service.Nombre))
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "InvalidNameCharacters")
                };
            }

            // 3. Validar longitud mínima de descripción (regla de negocio)
            if (service.Descripcion.Length < 25)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Servicios", "ShortDescription")
                };
            }

            return new OperationResult { success = true };
        }

        // Método auxiliar para validar el formato del nombre del servicio
        private bool IsValidServiceName(string name)
        {
            // Permitir letras, números, espacios y algunos caracteres específicos
            // Rechazar caracteres especiales como @, #, $, %, etc.
            var regex = new Regex(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-_(),.]+$");
            return regex.IsMatch(name);
        }
    }
}

