using SGHR.Application.Base;
using SGHR.Application.Dtos.Pisos;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface IPisosService : IBaseService<SavePisosDto, UpdatePisosDto, RemovePisosDto>
    {

    }
}
