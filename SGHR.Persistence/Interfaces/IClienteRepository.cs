using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<IEnumerable<Cliente>> ObtenerTodosLosClientesAsync();
        Task<OperationResult> GetEntityByIdAsync(int idCliente);
        Task<OperationResult> ObtenerClientePorEstadoIdAsync(int idEstadoCliente);
        Task<OperationResult> ObtenerClientePorFilterAsync(Expression<Func<Cliente, bool>> filter);
        Task<OperationResult> SaveEntityAsync(int idCliente);
        Task<OperationResult> UpdateEntityAsync(int idCliente);
        Task<OperationResult> DeleteEntityAsync(int idCliente);
        Task<OperationResult> ExisteClienteAsync(int idCliente);
    }
}
