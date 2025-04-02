using SGHR.WebApi.Models.Categorias;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenApi.Repository
{
    public class CategoriasRepository : BaseApiClient, IRepository<CategoriasApiModel>
    {
        private readonly ILogger<CategoriasRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public CategoriasRepository(HttpClient httpClient, IConfiguration configuration, ILogger<CategoriasRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateCategoriaPersistence(CategoriasApiModel categoria)
        {
            if (categoria == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<CategoriasApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<CategoriasApiModel>>("Categoria/GetCategoria");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<CategoriasApiModel>();
            }
        }

        public async Task<CategoriasApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<CategoriasApiModel>($"Categoria/GetCategoriaByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(CategoriasApiModel entity)
        {
            var result = ValidateCategoriaPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("Categoria/SaveCategoria", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(CategoriasApiModel entity)
        {
            var result = ValidateCategoriaPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"Categoria/UpdateCategoria/{entity.IdCategoria}", entity);
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
                await DeleteAsync($"Categoria/DeleteCategoria/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}


