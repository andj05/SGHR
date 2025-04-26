using SGHR.WebApi.Models.RolUsuario;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;
using Microsoft.Extensions.Logging;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.Models;

namespace SGHR.WebApi.PersistenApi.Repository
{
    public class RolUsuarioRepository : BaseApiClient, IRepository<RolUsuarioApiModel>
    {
        private readonly ILogger<RolUsuarioRepository> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public RolUsuarioRepository(HttpClient httpClient, IConfiguration configuration, ILogger<RolUsuarioRepository> logger, IErrorMessageService errorMessageService)
            : base(httpClient, configuration)
        {
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        private OperationResult ValidateRolUsuarioPersistence(RolUsuarioApiModel rolUsuario)
        {
            if (rolUsuario == null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("EntityBase", "NullEntity")
                };
            }
            return new OperationResult { success = true };
        }

        public async Task<IEnumerable<RolUsuarioApiModel>> GetAllAsync()
        {
            try
            {
                return await GetAsync<List<RolUsuarioApiModel>>("RolUsuario/GetRolUsuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return new List<RolUsuarioApiModel>();
            }
        }

        public async Task<RolUsuarioApiModel> GetByIdAsync(int id)
        {
            try
            {
                return await GetAsync<RolUsuarioApiModel>($"RolUsuario/GetRolByID?id={id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Generic", "GenericError"));
                return null;
            }
        }

        public async Task AddAsync(RolUsuarioApiModel entity)
        {
            var result = ValidateRolUsuarioPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PostAsync("RolUsuario/SaveRolUsuario", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
            }
        }

        public async Task UpdateAsync(RolUsuarioApiModel entity)
        {
            var result = ValidateRolUsuarioPersistence(entity);
            if (!result.success)
            {
                _logger.LogError(new Exception(result.message), result.message);
                return;
            }

            try
            {
                await PutAsync($"RolUsuario/UpdateRol/{entity.IdRolUsuario}", entity);
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
                await DeleteAsync($"RolUsuario/DeleteRolUsuario/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
            }
        }
    }
}
