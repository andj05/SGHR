using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Cliente
{
    public class SaveClienteDto : PersonaDto
    {
        public int IdCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Nacionalidad { get; set; }
    }
}
