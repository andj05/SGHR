using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SGHR.Domain.Base
{
    public abstract class BaseEntity<Ttype> : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public abstract Ttype Id { get; set; }
    }
}
