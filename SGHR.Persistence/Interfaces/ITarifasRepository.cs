using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

public interface ITarifasRepository : IBaseRepository<Tarifas>
{
    Task<IEnumerable<Tarifas>> GetAllAsync();
    Task<OperationResult> GetEntityByIdAsync(int id);
    Task<OperationResult> SaveEntityAsync(Tarifas tarifas);
    Task<OperationResult> UpdateEntityAsync(Tarifas tarifas);
    Task<OperationResult> DeleteEntityAsync(int id);
    Task<OperationResult> ExistsAsync(int id);
}

