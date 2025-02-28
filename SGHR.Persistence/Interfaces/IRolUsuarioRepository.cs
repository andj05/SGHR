using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IRolUsuarioRepository : IBaseRepository<RolUsuario>
    {
        Task<IEnumerable<RolUsuario>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int id);
        Task<OperationResult> SaveEntityAsync(RolUsuario rolUsuario);
        Task<OperationResult> UpdateEntityAsync(RolUsuario rolUsuario);
        Task<OperationResult> DeleteEntityAsync(int id);
        Task<OperationResult> ExistsAsync(int id);
    }
}
