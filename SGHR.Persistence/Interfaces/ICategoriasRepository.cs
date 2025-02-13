using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface ICategoriasRepository : IBaseRepository<Categorias>
    {
        Task<IEnumerable<Categorias>> ObtenerTodasLasCategoriasAsync();
        Task<IEnumerable<Categorias>> ObtenerCategoriasActivasAsync();
        Task<Categorias?> ObtenerCategoriaPorDescripcionAsync(string descripcion);
        Task<bool> ActualizarEstadoCategoriaAsync(int idCategoria, bool estado);
        Task<bool> ExisteCategoriaAsync(int idCategoria);
        Task<bool> ActualizarDescripcionCategoriaAsync(int idCategoria, string nuevaDescripcion);
    }
}