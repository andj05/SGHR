namespace WebAPI.Models.Cliente
{
    public class SaveClienteDto
    {
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Telefono { get; set; }
        public string Nacionalidad { get; set; }
        public int ChangeUser { get; set; }
    }
}
