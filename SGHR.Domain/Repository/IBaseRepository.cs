using SGHR.Domain.Base;
using System.Linq.Expressions;

namespace SGHR.Domain.Repository
{
    public interface IBaseRepository <TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task UpdateEntityAsync(TEntity entity);
        Task DeleteEntityAsync(TEntity entity);
        Task SaveEntityAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<OperactionResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
