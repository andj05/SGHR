namespace SGHR.WebApi.Models.Servicios
{
    public class ServiciosApiModel
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
