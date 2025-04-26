using SGHR.WebApi.Models.Servicios;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenApi.Repository
{
    public class ServiciosRepository : BaseApiClient, IRepository<ServiciosApiModel>
    {
        private readonly ILogger<ServiciosRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public ServiciosRepository(HttpClient httpClient, IConfiguration configuration, ILogger<ServiciosRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateServicioPersistence(ServiciosApiModel servicio)
        {
            if (servicio == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<ServiciosApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<ServiciosApiModel>>("Servicios/GetServicios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<ServiciosApiModel>();
            }
        }

        public async Task<ServiciosApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<ServiciosApiModel>($"Servicios/GetServiciosByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(ServiciosApiModel entity)
        {
            var result = ValidateServicioPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("Servicios/SaveServicio", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(ServiciosApiModel entity)
        {
            var result = ValidateServicioPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"Servicios/UpdateServicio/{entity.IdServicio}", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "UpdateFailed"));
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await DeleteAsync($"Servicios/DeleteServicio/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}
