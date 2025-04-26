namespace SGHR.WebApi.Models.RolUsuario
{
    public class RolUsuarioApiModel
    {
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }
        public bool? Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
