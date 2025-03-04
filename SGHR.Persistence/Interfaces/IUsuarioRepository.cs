using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<OperationResult> GetEntityByIdAsync(int idUsuario);
        Task<OperationResult> GetUsersByStatusAsync(int idEstadoUsuario);
        Task<OperationResult> GetByEmailAsync(string email);
        Task<OperationResult> GetUsersByFilterAsync(Expression<Func<Usuario, bool>> filter);
        Task<OperationResult> SaveEntityAsync(Usuario usuario);
        Task<OperationResult> UpdateEntityAsync(Usuario usuario);
        Task<OperationResult> DeleteEntityAsync(int idUsuario);
        Task<OperationResult> ExistsAsync(int idUsuario);
    }
}
