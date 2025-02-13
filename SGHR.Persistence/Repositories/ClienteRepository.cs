using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Base;
using SGHR.Persistence.Contex;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ClienteRepository> _logger;
        private readonly IConfiguration _configuration;

        public ClienteRepository(
            SGHRContext context,
            ILogger<ClienteRepository> logger,
            IConfiguration configuration)
            : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> ObtenerTodosLosClientesAsync()
        {
            var result = new OperationResult();
            try
            {
                var clientes = await _context.Set<Cliente>().ToListAsync();
                result.Data = clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes.");
                result.Success = false;
                result.Message = $"Error al obtener clientes: {ex.Message}";
            }
            return result;
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

        public async Task<OperationResult> GuardarClienteAsync(Cliente cliente)
        {
            if (cliente == null)
            {
                return new OperationResult { Success = false, Message = "El cliente no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(cliente.NombreCompleto))
            {
                return new OperationResult { Success = false, Message = "El nombre completo es obligatorio." };
            }

            try
            {
                return await SaveEntityAsync(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el cliente.");
                return new OperationResult { Success = false, Message = $"Error al guardar el cliente: {ex.Message}" };
            }
        }

        public async Task<OperationResult> ActualizarClienteAsync(Cliente cliente)
        {
            if (cliente == null)
            {
                return new OperationResult { Success = false, Message = "El cliente no puede ser nulo." };
            }
            if (cliente.IdCliente <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del cliente es inválido." };
            }
            try
            {
                return await UpdateEntityAsync(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente.");
                return new OperationResult { Success = false, Message = $"Error al actualizar el cliente: {ex.Message}" };
            }
        }

        public async Task<bool> ExisteClienteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }
            try
            {
                return await _context.Set<Cliente>().AnyAsync(c => c.IdCliente == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del cliente con id {id}.");
                return false;
            }
        }
    }
}
