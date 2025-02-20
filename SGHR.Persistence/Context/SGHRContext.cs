using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Configurations;


namespace SGHR.Persistence.Context
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options) 
        {
            
        }
        public DbSet<Habitacion> Habitacion { get; set; }
        public DbSet<Recepcion> Recepcion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new HabitacionConfiguration());
            modelBuilder.ApplyConfiguration(new RecepcionConfiguration());
        }

        }
}
