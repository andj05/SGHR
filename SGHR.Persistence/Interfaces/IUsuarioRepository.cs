using SGHR.Domain.Entities.Users;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
    }
}
