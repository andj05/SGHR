using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Intefaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class ClientesService : IClientesService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClientesService> _logger;
        private readonly IConfiguration _configuration;

        public ClientesService(IClienteRepository clienteRepository,
                                ILogger<ClientesService> logger,
                                IConfiguration configuration)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                operationResult.Data = clientes.Where(c => !c.Deleted).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes.");
                operationResult.Success = false;
                operationResult.Message = $"Error al obtener clientes: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente.Data is Cliente clienteData && clienteData.Deleted)
                return new OperationResult { Success = false, Message = "Cliente eliminado." };

            return cliente;
        }


        public async Task<OperationResult> Save(SaveClienteDto dto)
        {
            var validationResult = ValidateCliente(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var cliente = new Cliente
                {
                    TipoDocumento = dto.TipoDocumento,
                    Documento = dto.Documento,
                    NombreCompleto = dto.NombreCompleto,
                    Correo = dto.Correo,
                    Telefono = dto.Telefono,
                    Nacionalidad = dto.Nacionalidad,
                    CreationUser = 1
                };

                return await _clienteRepository.SaveEntityAsync(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el cliente.");
                return new OperationResult { Success = false, Message = $"Error al guardar el cliente: {ex.Message}" };
            }
        }

        public async Task<OperationResult> Update(UpdateClienteDto dto)
        {
            if (dto.IdCliente <= 0)
                return new OperationResult { Success = false, Message = "ID de cliente inválido." };

            var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
            if (cliente.Data is not Cliente clienteData || clienteData.Deleted)
                return new OperationResult { Success = false, Message = "Cliente no encontrado o eliminado." };

            clienteData.TipoDocumento = dto.TipoDocumento ?? clienteData.TipoDocumento;
            clienteData.Documento = dto.Documento ?? clienteData.Documento;
            clienteData.NombreCompleto = dto.NombreCompleto ?? clienteData.NombreCompleto;
            clienteData.Correo = dto.Correo ?? clienteData.Correo;
            clienteData.Telefono = dto.Telefono ?? clienteData.Telefono;
            clienteData.Nacionalidad = dto.Nacionalidad ?? clienteData.Nacionalidad;
            clienteData.ModifyDate = DateTime.Now;
            clienteData.ModifyUser = 1;

            return await _clienteRepository.UpdateEntityAsync(clienteData);
        }

        public async Task<OperationResult> Remove(RemoveClienteDto dto)
        {
            if (dto.IdCliente <= 0)
                return new OperationResult { Success = false, Message = "ID de cliente inválido." };

            var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
            if (cliente.Data is not Cliente clienteData || clienteData.Deleted)
                return new OperationResult { Success = false, Message = "Cliente no encontrado o ya eliminado." };

            clienteData.Deleted = true;
            clienteData.DeletedUser = 1;
            clienteData.ModifyDate = DateTime.Now;

            return await _clienteRepository.UpdateEntityAsync(clienteData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente.Data is not Cliente clienteData || !clienteData.Deleted)
                return new OperationResult { Success = false, Message = "Cliente no encontrado o ya activo." };

            clienteData.Deleted = false;
            clienteData.ModifyDate = DateTime.Now;
            clienteData.ModifyUser = 1;

            return await _clienteRepository.UpdateEntityAsync(clienteData);
        }

        private OperationResult ValidateCliente(dynamic cliente)
        {
            if (cliente == null)
                return new OperationResult { Success = false, Message = "El cliente no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(cliente.NombreCompleto) || cliente.NombreCompleto.Length > 50)
                return new OperationResult { Success = false, Message = "El nombre completo no puede estar vacío y debe tener un máximo de 50 caracteres." };

            if (!string.IsNullOrWhiteSpace(cliente.Correo) && cliente.Correo.Length > 50)
                return new OperationResult { Success = false, Message = "El correo debe tener un máximo de 50 caracteres." };

            if (!string.IsNullOrWhiteSpace(cliente.Telefono) && cliente.Telefono.Length > 20)
                return new OperationResult { Success = false, Message = "El teléfono debe tener un máximo de 20 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}
