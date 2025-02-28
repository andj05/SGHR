using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int idCategoria);
        Task<OperationResult> SaveEntityAsync(Categoria categoria);
        Task<OperationResult> UpdateEntityAsync(Categoria categoria);
        Task<OperationResult> DeleteEntityAsync(int idCategoria);
        Task<OperationResult> ExistsAsync(int idCategoria);

    }
}