using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Intefaces;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class ClientesService : IClientesService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

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
                var cliente = await _clienteRepository.GetAllAsync();
                var activeCliente = cliente
                    .Where(t => !t.Deleted)
                    .Select(ClienteMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                operationResult.Data = activeCliente;
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

        public async Task<OperationResult> GerAllDelete()
        {
            var operationResult = new OperationResult();
            try
            {
                var cliente = await _clienteRepository.GetAllAsync();
                var activeCliente = cliente
                    .Where(t => t.Deleted)
                    .Select(ClienteMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                operationResult.Data = activeCliente;
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
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = cliente;
                    operationResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
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
                var cliente = ClienteMapper.ToEntity(dto);
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
            if (dto.IdCliente <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
            if (cliente == null || cliente.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            cliente.UpdateFromDto(dto);
            return await _clienteRepository.UpdateEntityAsync(cliente);
        }


        public async Task<OperationResult> Remove(RemoveClienteDto dto)
        {
            if (dto.IdCliente <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
            if (cliente == null || cliente.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            cliente.RemoveFromDto(dto);
            return await _clienteRepository.UpdateEntityAsync(cliente);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente == null || !cliente.Deleted)
            {
                var message = cliente == null
                    ? _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    : _messageMapper.ErrorMessages["EntityBase"].ContainsKey("AlreadyActive")
                        ? _messageMapper.ErrorMessages["EntityBase"]["AlreadyActive"]
                        : "Entity is already active.";

                return new OperationResult
                {
                    Success = false,
                    Message = message
                };
            }

            cliente.RestoreFromDto(1);
            return await _clienteRepository.UpdateEntityAsync(cliente);
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
