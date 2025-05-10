namespace WebAPI.Models.Usuario
{
    public class SaveUsuarioDto
    {
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public int IdRolUsuario { get; set; }
        public string Clave { get; set; }
        public bool Estado { get; set; }
        public int CreationUser { get; set; }
    }
}
