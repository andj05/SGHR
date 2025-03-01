using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int idCategoria);
        Task<OperationResult> SaveEntityAsync(int idCategoria);
        Task<OperationResult> UpdateEntityAsync(Categoria categoria);
        Task<OperationResult> DeleteEntityAsync(int id);
        Task<OperationResult> ExistsAsync(int id);

    }
}