
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using SGHR.Persistence.Context;

namespace SGHR.Persistence.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly SGHRContext _context;
        private DbSet<TEntity> Entity { get; set; }
        public BaseRepository(SGHRContext context)
        {
            _context = context;
            Entity = _context.Set<TEntity>();
        }
        public virtual async Task<OperationResult> SaveEntityAsync(TEntity entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                Entity.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error guardando los datos: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult> UpdateEntityAsync(TEntity entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                Entity.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error guardando los datos: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            OperationResult result = new OperationResult();

            try
            {
                var datos = await Entity.Where(filter).ToListAsync();
                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error obteniendo los datos: {ex.Message}";
            }

            return result;
        }

        public virtual async Task<TEntity> GetEntityByIdAsync(int id)
        {
            return await Entity.FindAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {

            return await Entity.AnyAsync(filter);
        }
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Entity.ToListAsync();
        }

        public async Task<OperationResult> DeleteEntityAsync(int id)
        {
            OperationResult result = new OperationResult();
            try
            {
                var entity = await Entity.FindAsync(id);
                if (entity == null)
                {
                    result.Success = false;
                    result.Message = "Entity not found";
                    return result;
                }

                Entity.Remove(entity);
                await _context.SaveChangesAsync();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error eliminando los datos: {ex.Message}";
            }
            return result;
        }
    }
}
