using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ClienteRepository> _logger;
        private readonly IConfiguration _configuration;

        public ClienteRepository(SGHRContext context,
                                ILogger<ClienteRepository> logger,
                                IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosLosClientesAsync()
        {
            try
            {
                return await _context.Set<Cliente>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes.");
                throw;
            }
        }

        public async Task<OperationResult> ObtenerClientePorEstadoIdAsync(int idEstadoCliente)
        {
            var result = new OperationResult();
            try
            {
                bool estado = idEstadoCliente == 1;
                var clientes = await _context.Set<Cliente>()
                    .Where(c => c.Estado == estado)
                    .ToListAsync();
                result.Data = clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener clientes con estado {idEstadoCliente}.");
                result.Success = false;
                result.Message = $"Error al obtener clientes por estado: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ObtenerClientePorFilterAsync(Expression<Func<Cliente, bool>> filter)
        {
            if (filter == null)
            {
                return new OperationResult { Success = false, Message = "El filtro no puede ser nulo." };
            }

            var result = new OperationResult();
            try
            {
                var clientes = await _context.Set<Cliente>().Where(filter).ToListAsync();
                result.Data = clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes por filtro.");
                result.Success = false;
                result.Message = $"Error al obtener clientes por filtro: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> SaveEntityAsync(int idCliente)
        {
            var cliente = await _context.Set<Cliente>().FindAsync(idCliente);
            if (cliente == null)
            {
                return new OperationResult { Success = false, Message = "El cliente no fue encontrado." };
            }
            return await SaveEntityAsync(cliente);
        }

        public async Task<OperationResult> UpdateEntityAsync(int idCliente)
        {
            var cliente = await _context.Set<Cliente>().FindAsync(idCliente);
            if (cliente == null)
            {
                return new OperationResult { Success = false, Message = "El cliente no fue encontrado." };
            }
            return await UpdateEntityAsync(cliente);
        }

        public async Task<OperationResult> DeleteClienteAsync(int id)
        {
            var cliente = await _context.Set<Cliente>().FindAsync(id);
            if (cliente == null)
            {
                return new OperationResult { Success = false, Message = "El cliente no fue encontrado." };
            }
            return await DeleteEntityAsync(cliente.IdCliente);
        }

        public async Task<OperationResult> GetEntityByIdAsync(int idCliente)
        {
            var result = new OperationResult();
            try
            {
                var cliente = await _context.Set<Cliente>().FindAsync(idCliente);
                if (cliente == null)
                {
                    result.Success = false;
                    result.Message = "Cliente no encontrado.";
                }
                else
                {
                    result.Success = true;
                    result.Data = cliente;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el cliente con id {idCliente}.");
                result.Success = false;
                result.Message = $"Error al obtener el cliente: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ExisteClienteAsync(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "El ID del cliente es inválido.";
                return result;
            }
            try
            {
                bool exists = await _context.Set<Cliente>().AnyAsync(c => c.IdCliente == id);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del cliente con id {id}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del cliente: {ex.Message}";
            }
            return result;
        }
    }
}
