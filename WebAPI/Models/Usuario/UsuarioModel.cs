namespace WebAPI.Models.Usuario
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public int IdRolUsuario { get; set; }
        public string Clave { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int CreationUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyUser { get; set; }
        public int? DeletedUser { get; set; }
        public bool Deleted { get; set; }
    }
}