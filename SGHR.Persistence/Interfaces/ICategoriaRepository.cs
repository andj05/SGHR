using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> ObtenerTodasLasCategoriasAsync();
        Task<Categoria?> ObtenerCategoriaPorDescripcionAsync(string descripcion);
        Task<bool> ActualizarEstadoCategoriaAsync(int idCategoria, bool estado);
        Task<bool> ExisteCategoriaAsync(int idCategoria);
        Task<bool> ActualizarDescripcionCategoriaAsync(int idCategoria, string nuevaDescripcion);
        Task<OperationResult> DeleteEntityAsync(Categoria entity); 
    }
}
