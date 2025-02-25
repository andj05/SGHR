using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Persistence.Configurations
{
    public class TarifasConfiguration : IEntityTypeConfiguration<Tarifas>
    {
        public void Configure(EntityTypeBuilder<Tarifas> builder)
        {
            builder.HasKey(t => t.IdTarifa);
            builder.Property(t => t.IdTarifa).ValueGeneratedOnAdd();
        }
    }
}
