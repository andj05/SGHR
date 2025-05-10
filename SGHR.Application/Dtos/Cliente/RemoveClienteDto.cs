using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Cliente
{
    public class RemoveClienteDto : DtoBase
    {
        public int IdCliente { get; set; }
        public bool Removed { get; set; }
    }
}
