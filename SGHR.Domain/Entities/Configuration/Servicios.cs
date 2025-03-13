using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios : BaseEntity<int>
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public bool Estado { get; set; }
        public int? CreationUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyUser { get; set; }
        public int? DeletedUser { get; set; }
        public bool Deleted { get; set; }
    }
}

