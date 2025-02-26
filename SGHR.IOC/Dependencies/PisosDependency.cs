using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;


namespace SGHR.IOC.Dependencies
{
    public static class PisosDependency
    {
        public static void AddPisosDependency(this IServiceCollection service)
        {
            service.AddScoped<IPisoRepository, PisoRepository>();
            service.AddScoped<IPisosService, PisosService>();
        }
    }
}
