using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Configurations;
namespace SGHR.Persistence.Context
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options)
        {
        }

        public DbSet<Piso> Pisos { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<EstadoHabitacion> EstadoHabitacion { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<RolUsuario> RolUsuario { get; set; }
        public DbSet<Tarifas> Tarifas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EstadoHabitacionConfiguration());
            modelBuilder.ApplyConfiguration(new PisoConfiguration());
            modelBuilder.ApplyConfiguration(new ServiciosConfiguration());
            modelBuilder.ApplyConfiguration(new CategoriasConfiguration());
            modelBuilder.ApplyConfiguration(new TarifasConfiguration());
            modelBuilder.ApplyConfiguration(new RolUsuarioConfiguration());
        }
    }
}