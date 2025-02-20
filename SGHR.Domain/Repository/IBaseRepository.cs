using SGHR.Domain.Base;

using System.Linq.Expressions;

namespace SGHR.Domain.Repository
{
    /// <summary>
    /// Interfaz que deben heredar todos los repositorios.
    /// </summary>
    /// <typeparam name="TEntity">Entidad</typeparam>
    /// <typeparam name="TType">El tipo de dato del primary key para realizarla consulta.</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetEntityByIdAsync(int id);
        Task<OperationResult> UpdateEntityAsync(TEntity entity);
        Task<OperationResult> SaveEntityAsync(TEntity entity);
        Task<OperationResult> DeleteEntityAsync(int entity);
        Task<List<TEntity>> GetAllAsync();
        Task<OperationResult> GetFilteredAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}