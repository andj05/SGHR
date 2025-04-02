using SGHR.WebApi.Models.Piso;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenApi.Repository
{
    public class PisoRepository : BaseApiClient, IRepository<PisoApiModel>
    {
        private readonly ILogger<PisoRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public PisoRepository(HttpClient httpClient, IConfiguration configuration, ILogger<PisoRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidatePisoPersistence(PisoApiModel piso)
        {
            if (piso == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<PisoApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<PisoApiModel>>("Piso/GetPisos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<PisoApiModel>();
            }
        }

        public async Task<PisoApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<PisoApiModel>($"Piso/GetPisoByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(PisoApiModel entity)
        {
            var result = ValidatePisoPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("Piso/SavePiso", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(PisoApiModel entity)
        {
            var result = ValidatePisoPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"Piso/UpdatePiso/{entity.IdPiso}", entity);
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
                await DeleteAsync($"Piso/DeletePiso/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}

