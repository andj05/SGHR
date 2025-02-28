using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Piso : BaseEntity<int>
    {
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
