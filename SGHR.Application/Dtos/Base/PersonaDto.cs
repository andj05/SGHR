
namespace SGHR.Application.Dtos.Base
{
    public abstract class PersonaDto
    {
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Telefono { get; set; }
    }
}
