using WebAPI.Models;
using WebAPI.Models.Cliente;
using WebAPI.Models.Interfaces;
using WebAPI.Interfaces;
using System.Text.Json;

namespace WebAPI.Services
{
    public class ClienteRepository : BaseApiClient, IRepository<ClienteModel>
    {
        private const string BaseEndpoint = "Cliente";
        private readonly ILoggerManager _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly MessageMapper _messageMapper;

        public ClienteRepository(
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

        public async Task<IEnumerable<ClienteModel>> GetAllAsync()
        {
            return await GetAsync<IEnumerable<ClienteModel>>($"{BaseEndpoint}/GetClientes");
        }

        public async Task<ClienteModel> GetByIdAsync(int id)
        {
            return await GetAsync<ClienteModel>($"{BaseEndpoint}/GetClienteByID/{id}");
        }

        public async Task<ClienteModel> AddAsync(ClienteModel entity)
        {
            var saveDto = new SaveClienteDto
            {
                TipoDocumento = entity.TipoDocumento,
                Documento = entity.Documento,
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                Clave = entity.Clave,
                Telefono = entity.Telefono,
                Nacionalidad = entity.Nacionalidad,
                ChangeUser = 1
            };
            return await PostAsync<ClienteModel>($"{BaseEndpoint}/SaveCliente", saveDto);
        }

        public async Task UpdateAsync(ClienteModel entity, int id)
        {
            var updateDto = new UpdateClienteDto
            {
                IdCliente = id,
                TipoDocumento = entity.TipoDocumento,
                Documento = entity.Documento,
                NombreCompleto = entity.NombreCompleto,
                Correo = entity.Correo,
                Clave = entity.Clave,
                Telefono = entity.Telefono,
                Nacionalidad = entity.Nacionalidad,
                Estado = entity.Estado,
                ChangeUser = 1,
                ChangeDate = DateTime.Now
            };
            await PutAsync($"{BaseEndpoint}/UpdateCliente/{id}", updateDto);
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteAsync($"{BaseEndpoint}/DeleteCliente/{id}");
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var result = await GetAsync<ClienteModel>($"{BaseEndpoint}/GetClienteByID/{id}");
            return result != null;
        }

        public async Task<IEnumerable<ClienteModel>> GetActiveAsync()
        {
            var result = await GetAsync<IEnumerable<ClienteModel>>($"{BaseEndpoint}/GetClientes");
            return result?.Where(c => !c.Deleted) ?? Enumerable.Empty<ClienteModel>();
        }

        public async Task<ClienteModel> GetByFilterAsync(Func<ClienteModel, bool> predicate)
        {
            var all = await GetAllAsync();
            return all?.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<ClienteModel>> GetDeletedAsync()
        {
            return await GetAsync<IEnumerable<ClienteModel>>($"{BaseEndpoint}/GetDeletedClientes");
        }

        public async Task<ClienteModel> GetDeletedByIdAsync(int id)
        {
            return await GetAsync<ClienteModel>($"{BaseEndpoint}/GetDeletedClienteByID/{id}");
        }

        public async Task<ClienteModel> RestoreAsync(int id)
        {
            await PutAsync($"{BaseEndpoint}/RestoreCliente/{id}", null);
            return await GetByIdAsync(id);
        }

        public async Task<ClienteModel> CreateAsync(ClienteModel cliente)
        {
            cliente.Estado = true;
            cliente.Deleted = false;
            cliente.FechaCreacion = DateTime.Now;
            cliente.CreationUser = 1;
            cliente.ModifyUser = 1;
            cliente.ModifyDate = DateTime.Now;

            var saveDto = new SaveClienteDto
            {
                TipoDocumento = cliente.TipoDocumento,
                Documento = cliente.Documento,
                NombreCompleto = cliente.NombreCompleto,
                Correo = cliente.Correo,
                Clave = cliente.Clave,
                Telefono = cliente.Telefono,
                Nacionalidad = cliente.Nacionalidad,
                ChangeUser = cliente.CreationUser
            };

            return await PostAsync<ClienteModel>($"{BaseEndpoint}/SaveCliente", saveDto);
        }
    }
}
