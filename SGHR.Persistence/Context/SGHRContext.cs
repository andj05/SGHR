using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Persistence.Context
{
    public class SGHRContext : DbContext
    {
       public SGHRContext(DbContextOptions<SGHRContext> options) : base(options)
        { 
            
        }

        public DbSet<Piso> Pisos { get; set; }

        public DbSet<Categorias> Categorias { get; set; }

        public DbSet<EstadoHabitacion> EstadoHabitacion { get; set; }

        public DbSet<Servicios> Servicios { get; set; }

        public DbSet<RolUsuario> RolUsuarios { get; set; }

        public DbSet<Tarifas> Tarifas { get; set; }

        public DbSet<LogReservas> LogReservas { get; set; }

        public DbSet<Categoria_Servicio> Categoria_Servicio { get; set; }
    }
}
