using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Persistence.Configurations
{
    public class ServiciosConfiguration : IEntityTypeConfiguration<Servicios>
    {
        public void Configure(EntityTypeBuilder<Servicios> builder)
        {
            builder.HasKey(s => s.IdServicio);
        }
    }
}
