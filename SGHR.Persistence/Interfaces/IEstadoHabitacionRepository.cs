using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IEstadoHabitacionRepository : IBaseRepository<EstadoHabitacion>
    {
        Task<IEnumerable<EstadoHabitacion>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int id);
        Task<OperationResult> SaveEntityAsync(int idEstadoHabitacion);
        Task<OperationResult> UpdateEntityAsync(EstadoHabitacion estadoHabitacion);
        Task<OperationResult> DeleteEntityAsync(int id);
        Task<OperationResult> ExistsAsync(int id);
    }
}

