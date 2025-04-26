using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;

namespace SGHR.IOC.Dependencies
{
    public static class RolUsuarioDependency
    {
        public static void AddRolUsuarioDependency(this IServiceCollection service)
        {
            service.AddScoped<IRolUsuarioRepository, RolUsuarioRepository>();
            service.AddScoped<IRolUsuarioService, RolUsuarioService>();
        }
    }
}
