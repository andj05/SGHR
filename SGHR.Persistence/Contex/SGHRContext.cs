using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Users;

namespace SGHR.Persistence.Contex
{
    public class SGHRContext : DbContext
    {
        public SGHRContext(DbContextOptions<SGHRContext> options) : base(options)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
