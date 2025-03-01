using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IServiciosRepository : IBaseRepository<Servicios>
    {
        Task<IEnumerable<Servicios>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int id);
        Task<OperationResult> SaveEntityAsync(Servicios servicio);
        Task<OperationResult> UpdateEntityAsync(Servicios servicio);
        Task<OperationResult> DeleteEntityAsync(int id);
        Task<OperationResult> ExistsAsync(int id);
    }
}