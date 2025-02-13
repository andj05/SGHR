using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<OperationResult> ObtenerTodosLosUsuariosAsync();
        Task<OperationResult> ObtenerUsuariosPorEstadoIdAsync(int idEstadoUsuario);
        Task<OperationResult> ObtenerUsuariosPorFilterAsync(Expression<Func<Usuario, bool>> filter);
        Task<OperationResult> GuardarUsuariosAsync(Usuario usario);
        Task<OperationResult> ActualizarUsuariosAsync(Usuario usario);
        Task<bool> ExisteUsuarioAsync(int id);
    }
}
