using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Intefaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Users
{
    public static class UsuarioDependency
    {
        public static void AddUsuarioDependency(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IUsuariosService, UsuariosService>();
        }
    }
}
