using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Cliente
{
    public class ClienteDto : DtoBase
    {
        public int? IdCliente { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Telefono { get; set; }
        public string? Nacionalidad { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public bool Estado { get; set; }
        public bool Deleted { get; set; }
    }
}
