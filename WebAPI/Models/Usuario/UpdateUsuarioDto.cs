namespace WebAPI.Models.Usuario
{
    public class UpdateUsuarioDto
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public int IdRolUsuario { get; set; }
        public bool Estado { get; set; }
        public int ChangeUser { get; set; }
        public DateTime ChangeDate { get; set; }
    }
}
