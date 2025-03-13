
namespace SGHR.Application.Dtos.RolUsuario
{
    public class RolUsuarioDto : DtoBase
    {
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
