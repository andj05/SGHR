using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Habitacion;
using SGHR.WebApi.PersistenceApi.Configuration;
using SGHR.WebApi.PersistenceApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.PersistenceApi.Repository
{
    public class HabitacionRepository : BaseApiClient, IRepository<HabitacionModel>
    {
        private readonly ILoggerManager<HabitacionRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public HabitacionRepository(HttpClient httpClient, IConfiguration configuration, ILoggerManager<HabitacionRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateHabitacionPersistence(HabitacionModel habitacion)
        {
            if (habitacion == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { Success = true };
        }

        public async Task<IEnumerable<HabitacionModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<HabitacionModel>>("Habitacion/GetHabitacion");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<HabitacionModel>();
            }
        }

        public async Task<HabitacionModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<HabitacionModel>($"Habitacion/GetHabitacionById?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(HabitacionModel entity)
        {
            var result = ValidateHabitacionPersistence(entity);
            if (!result.Success)
            {
                _logger.LogError(new Exception(result.Message), result.Message);
                return;
            }

            try
            {
                await PostAsync<HabitacionModel>("Habitacion/GuardarHabitacion", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(HabitacionModel entity)
        {
            var result = ValidateHabitacionPersistence(entity);
            if (!result.Success)
            {
                _logger.LogError(new Exception(result.Message), result.Message);
                return;
            }

            try
            {
                await PutAsync($"Habitacion/ActualizarHabitacion/{entity.Id}", entity);
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
                await DeleteAsync($"Habitacion/BorrarHabitacion/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}
