using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SGHR.Persistence.Configurations
{
    public class EstadoHabitacionConfiguration : IEntityTypeConfiguration<EstadoHabitacion>
    {
        public void Configure(EntityTypeBuilder<EstadoHabitacion> builder)
        {
            builder.HasKey(e => e.IdEstadoHabitacion);
        }
    }
}