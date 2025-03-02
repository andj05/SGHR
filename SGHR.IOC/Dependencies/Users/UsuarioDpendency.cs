using Microsoft.Extensions.DependencyInjection;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Users
{
    public static class UsuarioDpendency
    {
        public static void AddUsuarioDependency(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
        }
    }
}
