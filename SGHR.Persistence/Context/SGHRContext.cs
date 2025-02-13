using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Reservation;


namespace SGHR.Persistence.Context
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options) 
        {
            
        }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Recepcion> Recepciones { get; set; }

    }
}
