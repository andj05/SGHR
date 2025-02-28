using SGHR.Application.Base;
using SGHR.Application.Dtos.Cliente;

namespace SGHR.Application.Intefaces
{
    public interface IClientesService : IBaseService<SaveClienteDto, UpdateClienteDto, RemoveClienteDto>
    {

    }
}
