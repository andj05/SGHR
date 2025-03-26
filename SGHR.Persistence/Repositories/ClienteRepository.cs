using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public ClienteRepository(SGHRContext context,
                                ILoggerManager logger,
                                MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateCliente(Cliente cliente, bool isUpdate = false)
        {
            var result = new OperationResult();

            if (cliente == null)
            {
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(result.Message);
                return result;
            }

            if (isUpdate && cliente.Id <= 0)
            {
                result.Success = false;
                result.Message = $"{_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]} Valor recibido: {cliente.Id}";
                _logger.LogWarn(result.Message);
                return result;
            }

            // Validar otros campos obligatorios del cliente
            // Ejemplo:
            // if (string.IsNullOrWhiteSpace(cliente.Nombre))
            // {
            //     result.Success = false;
            //     result.Message = "El campo 'Nombre' es obligatorio.";
            //     _logger.LogWarn(result.Message);
            //     return result;
            // }

            result.Success = true;
            return result;
        }

        public override async Task<List<Cliente>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Cliente>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<Cliente> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                string msg = $"{_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]} Valor recibido: {id}";
                _logger.LogWarn(msg);
                return null;
            }

            try
            {
                var cliente = await _context.Set<Cliente>().FindAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarn($"{_messageMapper.ErrorMessages["EntityBase"]["NotFound"]} ID: {id}");
                    return null;
                }
                return cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<OperationResult> GetFilteredAsync(Expression<Func<Cliente, bool>> filter)
        {
            var result = new OperationResult();
            try
            {
                if (filter == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                    _logger.LogWarn(msg);
                    result.Success = false;
                    result.Message = msg;
                    return result;
                }

                _logger.LogInfo($"Aplicando filtro: {filter}");
                var filteredEntities = await base.GetFilteredAsync(filter);
                result.Success = true;
                result.Data = filteredEntities.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["DbException"];
            }
            return result;
        }

        public async Task<OperationResult> GetClientsByStatusAsync(int idEstadoCliente)
        {
            try
            {
                if (idEstadoCliente is not (0 or 1))
                {
                    string invalidState = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                    string msg = $"{invalidState} Estado de cliente inválido. Debe ser 0 o 1.";
                    _logger.LogWarn(msg);
                    return new OperationResult { Success = false, Message = msg };
                }

                bool estado = idEstadoCliente == 1;
                var clientes = await _context.Set<Cliente>()
                    .AsNoTracking()
                    .Where(c => c.Estado == estado)
                    .ToListAsync();

                _logger.LogInfo($"Se recuperaron {clientes.Count} clientes con el estado {idEstadoCliente}");
                return new OperationResult { Success = true, Data = clientes };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: Error al obtener clientes con estado {idEstadoCliente}");
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["DbException"] + ": " + ex.Message
                };
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Cliente cliente)
        {
            // Validar que el objeto no sea null y que cumpla con las reglas de negocio.
            var validationResult = ValidateCliente(cliente, isUpdate: false);
            if (!validationResult.Success)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"] + " - " + validationResult.Message
                };
            }

            try
            {
                // Asignar fechas y usuario de modificación (se recomienda obtener el usuario actual en producción)
                cliente.FechaCreacion = DateTime.UtcNow;
                cliente.ModifyDate = DateTime.UtcNow;
                cliente.ModifyUser = 1;

                await _context.Set<Cliente>().AddAsync(cliente);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Data = cliente,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Cliente cliente)
        {
            var result = new OperationResult();

            // Validar que el objeto y su ID sean válidos, y otros campos obligatorios
            var validationResult = ValidateCliente(cliente, isUpdate: true);
            if (!validationResult.Success)
            {
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"] + " - " + validationResult.Message;
                return result;
            }

            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                // Actualizar los datos modificables con Entity Framework Core
                _context.Entry(existingCliente).CurrentValues.SetValues(cliente);

                // Excluir propiedades que no deben ser actualizadas
                _context.Entry(existingCliente).Property(x => x.FechaCreacion).IsModified = false;
                _context.Entry(existingCliente).Property(x => x.CreationUser).IsModified = false;

                // Actualizar fecha y usuario de modificación
                existingCliente.ModifyDate = DateTime.UtcNow;
                existingCliente.ModifyUser = 1; // En producción, obtener el usuario autenticado

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingCliente;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"] + ": " + ex.Message;
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Cliente cliente)
        {
            // Validar que el objeto no sea null y tenga un ID válido.
            if (cliente == null || cliente.Id <= 0)
            {
                string msg = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(msg);
                return new OperationResult
                {
                    Success = false,
                    Message = msg
                };
            }

            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                _context.Set<Cliente>().Remove(existingCliente);
                await _context.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["DeleteSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"] + ": " + ex.Message
                };
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Cliente, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                return false;
            }

            try
            {
                return await _context.Set<Cliente>().AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                throw;
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(Cliente cliente)
        {
            // Validar que el objeto no sea null y tenga un ID válido.
            if (cliente == null || cliente.Id <= 0)
            {
                string msg = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
                _logger.LogWarn(msg);
                return new OperationResult
                {
                    Success = false,
                    Message = msg
                };
            }

            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    string msg = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    _logger.LogWarn(msg);
                    return new OperationResult
                    {
                        Success = false,
                        Message = msg
                    };
                }

                existingCliente.Deleted = false;
                existingCliente.ModifyDate = DateTime.UtcNow;
                existingCliente.ModifyUser = 1; // En producción, obtener el usuario autenticado

                await _context.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["RestoreSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"] + ": " + ex.Message
                };
            }
        }
    }
}
