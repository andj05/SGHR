namespace WebAPI.Models.Cliente
{
    public class ClienteModel
    {
        public int IdCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Telefono { get; set; }
        public string? Nacionalidad { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyUser { get; set; }
        public int? DeletedUser { get; set; }
        public bool Deleted { get; set; }
    }

}
