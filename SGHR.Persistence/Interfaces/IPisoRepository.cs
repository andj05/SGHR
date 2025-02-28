using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        Task<IEnumerable<Piso>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int id);
        Task<OperationResult> SaveEntityAsync(Piso piso);
        Task<OperationResult> UpdateEntityAsync(Piso piso);
        Task<OperationResult> DeleteEntityAsync(int id);
        Task<OperationResult> ExistsAsync(int id);

    }
}
