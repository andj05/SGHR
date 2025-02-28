using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Cliente
{
    public class SaveClienteDto : PersonaDto
    {
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Nacionalidad { get; set; }
    }
}
