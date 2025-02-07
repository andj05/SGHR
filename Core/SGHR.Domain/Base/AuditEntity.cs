

namespace SGHR.Domain.Base
{
    public abstract class AuditEntity
    {
        protected AuditEntity() 
        {
            this.Estado = true;
        }
        public bool Estado {  get; set; }   
    }
}
