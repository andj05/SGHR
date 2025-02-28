
using SGHR.Application.Base;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface ITarifasService : IBaseService<SaveTarifasDto, UpdateTarifasDto, RemoveTarifasDto>
    {

    }
}
