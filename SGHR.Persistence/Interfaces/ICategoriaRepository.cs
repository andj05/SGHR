using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> ObtenerTodasLasCategoriasAsync();
        Task<bool> ExisteCategoriaAsync(int idCategoria);
        Task<OperationResult> DeleteEntityAsync(Categoria entity); 
    }
}
