using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Persistence.Configurations
{
    public class RecepcionConfiguration : IEntityTypeConfiguration<Recepcion>
    {
        public void Configure(EntityTypeBuilder<Recepcion> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("IdRecepcion");
            builder.Property(r => r.FechaEntrada).IsRequired();
            // Agrega explícitamente la propiedad FechaCreacion como requerida
            builder.Property(r => r.FechaCreacion)
                   .IsRequired();
            // Si lo necesitas, también puedes mapear otras propiedades de auditoría:
            builder.Property(r => r.Estado).IsRequired();
            builder.Property(r => r.CreationUser).IsRequired();
            // etc.
        }
    }
}
