using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Intefaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.Application.Services
{
    public class ClientesService : IClientesService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;
        private ClienteRepository clienteRepository;
        private MessageMapper messageMapper;
        private LoggerManager logger;

        public ClientesService(IClienteRepository clienteRepository,
                               MessageMapper messageMapper,
                               ILoggerManager loggerManager)
        {
            _clienteRepository = clienteRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                operationResult.Data = clientes;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente == null)
                {
                    operationResult.Success = false;
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                }
                else
                {
                    operationResult.Success = true;
                    operationResult.Data = cliente;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
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
                    Clave = dto.Clave,
                    Telefono = dto.Telefono,
                    Nacionalidad = dto.Nacionalidad,
                    CreationUser = 1
                };

                return await _clienteRepository.SaveEntityAsync(cliente);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = $"{_messageMapper.ErrorMessages["Operations"]["SaveFailed"]}: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> Update(UpdateClienteDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdCliente <= 0)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"] };

                var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
                if (cliente == null)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                if (cliente is not Cliente clienteData || clienteData.Deleted)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                clienteData.TipoDocumento = dto.TipoDocumento ?? clienteData.TipoDocumento;
                clienteData.Documento = dto.Documento ?? clienteData.Documento;
                clienteData.NombreCompleto = dto.NombreCompleto ?? clienteData.NombreCompleto;
                clienteData.Correo = dto.Correo ?? clienteData.Correo;
                clienteData.Clave = dto.Clave ?? clienteData.Clave;
                clienteData.Telefono = dto.Telefono ?? clienteData.Telefono;
                clienteData.Nacionalidad = dto.Nacionalidad ?? clienteData.Nacionalidad;
                clienteData.ModifyDate = DateTime.Now;
                clienteData.ModifyUser = 1;

                var updateResult = await _clienteRepository.UpdateEntityAsync(clienteData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
                operationResult.Data = updateResult.Data;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveClienteDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdCliente <= 0)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"] };

                var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
                if (cliente == null)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                if (cliente is not Cliente clienteData || clienteData.Deleted)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                clienteData.Deleted = true;
                clienteData.DeletedUser = 1;
                clienteData.ModifyDate = DateTime.Now;

                var updateResult = await _clienteRepository.UpdateEntityAsync(clienteData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
                operationResult.Data = updateResult.Data;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DeleteFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente == null)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                if (cliente is not Cliente clienteData || !clienteData.Deleted)
                    return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] };

                clienteData.Deleted = false;
                clienteData.ModifyDate = DateTime.Now;
                clienteData.ModifyUser = 1;

                var updateResult = await _clienteRepository.UpdateEntityAsync(clienteData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
                operationResult.Data = updateResult.Data;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["RestoreFailed"]}: {ex.Message}";
            }
            return operationResult;
        }

        private OperationResult ValidateCliente(dynamic cliente)
        {
            if (cliente == null)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"] };

            if (string.IsNullOrWhiteSpace(cliente.NombreCompleto) || cliente.NombreCompleto.Length > 50)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Client"]["MissingName"] };

            if (!string.IsNullOrWhiteSpace(cliente.Correo) && cliente.Correo.Length > 50)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Client"]["InvalidEmail"] };

            if (!string.IsNullOrWhiteSpace(cliente.Telefono) && cliente.Telefono.Length > 20)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Client"]["InvalidPhone"] };

            if (string.IsNullOrWhiteSpace(cliente.Clave) || cliente.Clave.Length < 6)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Client"]["MissingPassword"] };

            return new OperationResult { Success = true };
        }
    }
}
