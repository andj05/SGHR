using SGHR.WebApi.Models.EstadoHabitacion;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenApi.Repository
{
    public class EstadoHabitacionRepository : BaseApiClient, IRepository<EstadoHabitacionApiModel>
    {
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public EstadoHabitacionRepository(HttpClient httpClient, IConfiguration configuration, ILogger<EstadoHabitacionRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateEstadoHabitacionPersistence(EstadoHabitacionApiModel estadoHabitacion)
        {
            if (estadoHabitacion == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<EstadoHabitacionApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<EstadoHabitacionApiModel>>("EstadoHabitacion/GetEstadoHabitacion");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<EstadoHabitacionApiModel>();
            }
        }

        public async Task<EstadoHabitacionApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<EstadoHabitacionApiModel>($"EstadoHabitacion/GetEstadoByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(EstadoHabitacionApiModel entity)
        {
            var result = ValidateEstadoHabitacionPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("EstadoHabitacion/SaveEstadoHabitacion", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(EstadoHabitacionApiModel entity)
        {
            var result = ValidateEstadoHabitacionPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"EstadoHabitacion/UpdateEstadoHabitacion/{entity.IdEstadoHabitacion}", entity);
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
                await DeleteAsync($"EstadoHabitacion/DeleteEstado/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}

