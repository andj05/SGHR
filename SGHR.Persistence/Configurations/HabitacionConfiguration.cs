using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Persistence.Configurations
{
    public class HabitacionConfiguration : IEntityTypeConfiguration<Habitacion>
    {
        public void Configure(EntityTypeBuilder<Habitacion> builder)
        {
            builder.HasKey(h => h.IdHabitacion); 
            builder.Property(h => h.Numero).IsRequired();
            
        }
    }
}