using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Persistence.Configurations
{
    public class PisoConfiguration : IEntityTypeConfiguration<Piso>
    {
        public void Configure(EntityTypeBuilder<Piso> builder)
        {
            builder.ToTable("Pisos");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .HasColumnName("IdPiso");
        }
    }
}
