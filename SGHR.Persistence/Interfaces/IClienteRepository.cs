using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<OperationResult> ObtenerTodosLosClientesAsync();
        Task<OperationResult> ObtenerClientePorEstadoIdAsync(int idEstadoCliente);
        Task<OperationResult> ObtenerClientePorFilterAsync(Expression<Func<Cliente, bool>> filter);
        Task<OperationResult> GuardarClienteAsync(Cliente cliente);
        Task<OperationResult> ActualizarClienteAsync(Cliente cliente);
        Task<bool> ExisteClienteAsync(int id);
    }
}
