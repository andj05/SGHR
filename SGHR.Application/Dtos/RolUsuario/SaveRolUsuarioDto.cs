
namespace SGHR.Application.Dtos.RolUsuario
{
    public class SaveRolUsuarioDto : RolUsuarioDto
    {
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
