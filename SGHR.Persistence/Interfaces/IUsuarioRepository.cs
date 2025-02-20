using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<OperationResult> ObtenerTodosLosUsuariosAsync();
        Task<OperationResult> GetEntityByIdAsync(int idUsuario);
        Task<OperationResult> ObtenerUsuariosPorFilterAsync(Expression<Func<Usuario, bool>> filter);
        Task<OperationResult> SaveEntityAsync(Usuario usario);
        Task<OperationResult> UpdateEntityAsync(Usuario usario);
        Task<OperationResult> DeleteEntityAsync(int idUsuario);
        Task<OperationResult> ExisteUsuarioAsync(int idUsuario);
    }
}
