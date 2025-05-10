namespace WebAPI.Models.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity, int id);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<T>> GetActiveAsync();
        Task<T> GetByFilterAsync(Func<T, bool> predicate);
        Task<IEnumerable<T>> GetDeletedAsync();
        Task<T> GetDeletedByIdAsync(int id);
        Task<T> RestoreAsync(int id);
        Task<T> CreateAsync(T entity);
    }
}

