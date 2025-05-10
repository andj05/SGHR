using WebAPI.Models;
using WebAPI.Models.Usuario;
using WebAPI.Models.Interfaces;
using WebAPI.Interfaces;
using System.Text.Json;

namespace WebAPI.Services
{
    public class UsuarioRepository : BaseApiClient, IUsuarioRepository
    {
        private const string BaseEndpoint = "Usuario";
        private readonly ILoggerManager _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly MessageMapper _messageMapper;

        public UsuarioRepository(
            HttpClient httpClient,
            IConfiguration configuration,
            ILoggerManager logger,
            MessageMapper messageMapper)
            : base(httpClient, configuration, logger)
        {
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _messageMapper = messageMapper;
        }

        public async Task<IEnumerable<UsuarioModel>> GetAllAsync()
        {
            return await GetAsync<IEnumerable<UsuarioModel>>($"{BaseEndpoint}/GetUsuarios");
        }

        public async Task<UsuarioModel> GetByIdAsync(int id)
        {
            return await GetAsync<UsuarioModel>($"{BaseEndpoint}/GetUsuarioByID/{id}");
        }

        public async Task<UsuarioModel> AddAsync(UsuarioModel entity)
        {
            var saveDto = new SaveUsuarioDto
            {
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                Clave = entity.Clave,
                IdRolUsuario = entity.IdRolUsuario,
                Estado = entity.Estado,
                CreationUser = 1
            };
            return await PostAsync<UsuarioModel>($"{BaseEndpoint}/SaveUsuario", saveDto);
        }

        public async Task UpdateAsync(UsuarioModel entity, int id)
        {
            var updateDto = new UpdateUsuarioDto
            {
                IdUsuario = id,
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                Clave = entity.Clave,
                IdRolUsuario = entity.IdRolUsuario,
                Estado = entity.Estado,
                ChangeUser = 1,
                ChangeDate = DateTime.Now
            };
            await PutAsync($"{BaseEndpoint}/UpdateUsuario/{id}", updateDto);
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteAsync($"{BaseEndpoint}/DeleteUsuario/{id}");
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var result = await GetAsync<UsuarioModel>($"{BaseEndpoint}/GetUsuarioByID/{id}");
            return result != null;
        }

        public async Task<IEnumerable<UsuarioModel>> GetActiveAsync()
        {
            var result = await GetAsync<IEnumerable<UsuarioModel>>($"{BaseEndpoint}/GetUsuarios");
            return result?.Where(u => !u.Deleted) ?? Enumerable.Empty<UsuarioModel>();
        }

        public async Task<UsuarioModel> GetByFilterAsync(Func<UsuarioModel, bool> predicate)
        {
            var all = await GetAllAsync();
            return all?.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<UsuarioModel>> GetDeletedAsync()
        {
            return await GetAsync<IEnumerable<UsuarioModel>>($"{BaseEndpoint}/GetDeletedUsuarios");
        }

        public async Task<UsuarioModel> GetDeletedByIdAsync(int id)
        {
            return await GetAsync<UsuarioModel>($"{BaseEndpoint}/GetDeletedUsuarioByID/{id}");
        }

        public async Task<UsuarioModel> RestoreAsync(int id)
        {
            await PutAsync($"{BaseEndpoint}/RestoreUsuario/{id}", null);
            return await GetByIdAsync(id);
        }

        public async Task<LoginResponseModel> LoginAsync(LoginModel login)
        {
            return await PostAsync<LoginResponseModel>($"{BaseEndpoint}/Login", login);
        }

        public async Task<UsuarioModel> CreateAsync(UsuarioModel usuario)
        {
            usuario.Estado = true;
            usuario.Deleted = false;
            usuario.FechaCreacion = DateTime.Now;
            usuario.CreationUser = 1;
            usuario.ModifyUser = 1;
            usuario.ModifyDate = DateTime.Now;

            var saveDto = new SaveUsuarioDto
            {
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                Clave = usuario.Clave,
                IdRolUsuario = usuario.IdRolUsuario,
                Estado = usuario.Estado,
                CreationUser = usuario.CreationUser
            };

            return await PostAsync<UsuarioModel>($"{BaseEndpoint}/SaveUsuario", saveDto);
        }
    }
}
