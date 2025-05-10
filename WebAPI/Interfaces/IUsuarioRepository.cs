using WebAPI.Models.Usuario;
using WebAPI.Models.Interfaces;

namespace WebAPI.Interfaces
{
    public interface IUsuarioRepository : IRepository<UsuarioModel>
    {
        Task<LoginResponseModel> LoginAsync(LoginModel login);
    }
}