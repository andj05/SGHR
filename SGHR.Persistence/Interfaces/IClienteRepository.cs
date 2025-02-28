using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int idCliente);
        Task<OperationResult> GetClientsByStatusAsync(int idEstadoCliente);
        Task<OperationResult> GetClientsByFilterAsync(Expression<Func<Cliente, bool>> filter);
        Task<OperationResult> SaveEntityAsync(Cliente cliente);
        Task<OperationResult> UpdateEntityAsync(Cliente cliente);
        Task<OperationResult> DeleteEntityAsync(int idCliente);
        Task<OperationResult> ExistsAsync(int idCliente);

    }
}
