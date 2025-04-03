using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Recepcion;
using SGHR.WebApi.PersistenceApi.Configuration;
using SGHR.WebApi.PersistenceApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.PersistenceApi.Repository
{
    public class RecepcionRepository : BaseApiClient, IRepository<RecepcionModel>
    {
        private readonly ILoggerManager<RecepcionRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public RecepcionRepository(HttpClient httpClient, IConfiguration configuration,
            ILoggerManager<RecepcionRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateRecepcionPersistence(RecepcionModel recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { Success = true };
        }

        public async Task<IEnumerable<RecepcionModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<RecepcionModel>>("Recepcion/GetRecepciones");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<RecepcionModel>();
            }
        }

        public async Task<RecepcionModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<RecepcionModel>($"Recepcion/GetRecepcionById/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(RecepcionModel entity)
        {
            var result = ValidateRecepcionPersistence(entity);
            if (!result.Success)
            {
                _logger.LogError(new Exception(result.Message), result.Message);
                return;
            }

            try
            {
                await PostAsync<RecepcionModel>("Recepcion/GuardarRecepcion", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(RecepcionModel entity)
        {
            var result = ValidateRecepcionPersistence(entity);
            if (!result.Success)
            {
                _logger.LogError(new Exception(result.Message), result.Message);
                return;
            }

            try
            {
                await PutAsync($"Recepcion/ActualizarRecepcion/{entity.Id}", entity);
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
                await DeleteAsync($"Recepcion/BorrarRecepcion/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}