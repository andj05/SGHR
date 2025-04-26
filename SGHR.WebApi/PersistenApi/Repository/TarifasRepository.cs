using SGHR.WebApi.Models.Tarifas;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenceApi.Repository
{
    public class TarifasRepository : BaseApiClient, IRepository<TarifasApiModel>
    {
        private readonly ILogger<TarifasRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public TarifasRepository(HttpClient httpClient, IConfiguration configuration, ILogger<TarifasRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateTarifaPersistence(TarifasApiModel tarifa)
        {
            if (tarifa == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<TarifasApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<TarifasApiModel>>("Tarifas/GetTarifas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<TarifasApiModel>();
            }
        }

        public async Task<TarifasApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<TarifasApiModel>($"Tarifas/GetTarifasByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(TarifasApiModel entity)
        {
            var result = ValidateTarifaPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("Tarifas/SaveTarifas", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(TarifasApiModel entity)
        {
            var result = ValidateTarifaPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"Tarifas/UpdateTarifa/{entity.IdTarifa}", entity);
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
                await DeleteAsync($"Tarifas/DeleteTarifa/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}
