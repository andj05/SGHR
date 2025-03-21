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
                _logger.LogWarn($"{_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]} Valor recibido: {id}");
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
                    _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NullEntity"];
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
                    return new OperationResult { Success = false, Message = $"{invalidState} Estado de cliente inválido. Debe ser 0 o 1." };
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
            // Validación de null al inicio
            if (cliente == null)
            {
                _logger.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }

            try
            {
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
                    // Mensaje sin concatenar la excepción
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Cliente cliente)
        {
            var result = new OperationResult();
            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                // Actualizar los datos modificables con Entity Framework Core
                _context.Entry(existingCliente).CurrentValues.SetValues(cliente);

                // Excluir propiedades que no deben ser actualizadas
                _context.Entry(existingCliente).Property(x => x.FechaCreacion).IsModified = false;
                _context.Entry(existingCliente).Property(x => x.CreationUser).IsModified = false;

                // Actualizar ModifyDate y ModifyUser
                existingCliente.ModifyDate = DateTime.UtcNow;
                existingCliente.ModifyUser = 1; // Obtener el usuario autenticado en producción

                // Guardar cambios
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
            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
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
            try
            {
                var existingCliente = await _context.Set<Cliente>().FindAsync(cliente.Id);
                if (existingCliente == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingCliente.Deleted = false;
                existingCliente.ModifyDate = DateTime.UtcNow;
                existingCliente.ModifyUser = 1; // En producción, obtener el cliente autenticado.

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
