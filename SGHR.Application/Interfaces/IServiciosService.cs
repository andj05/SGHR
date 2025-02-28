using SGHR.Application.Base;
using SGHR.Application.Dtos.Servicios;  
using SGHR.Domain.Entities.Configuration;  

namespace SGHR.Application.Interfaces
{
    public interface IServiciosService : IBaseService<SaveServiciosDto, UpdateServiciosDto, RemoveServiciosDto>
    {

    }
}
