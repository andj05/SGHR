namespace SGHR.WebApi.Models.Piso
{
    public class PisoApiModel
    {
        public int IdPiso { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
