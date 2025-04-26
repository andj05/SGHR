

namespace SGHR.Domain.Base
{
    public abstract class AuditEntity
    {
        protected AuditEntity()
        {
            this.Estado = true;
            this.ModifyDate = DateTime.Now;
            this.FechaCreacion = DateTime.Now;
            this.Deleted = false;
        }

        public bool Estado { get; set; }
        public int? CreationUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyUser { get; set; }
        public int? DeletedUser { get; set; }
        public bool Deleted { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
