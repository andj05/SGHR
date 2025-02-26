using SGHR.Application.Base;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface ICategoriasService : IBaseService<SaveCategoriasDto,UpdateCategoriasDto,RemoveCategoriasDto>
    {
        Task<IEnumerable<Categoria>> ObtenerTodasLasCategoriasAsync();
        Task<bool> ExisteCategoriaAsync(int idCategoria);
        Task<OperationResult> DeleteEntityAsync(Categoria entity);
    }
}
