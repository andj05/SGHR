using SGHR.Domain.Base;
using System.Linq.Expressions;

namespace SGHR.Domain.Repository
{
    public interface IBaseRepository <TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<OperationResult> UpdateEntityAsync(TEntity entity);
        Task<OperationResult> SaveEntityAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
