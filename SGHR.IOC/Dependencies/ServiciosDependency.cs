using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;

namespace SGHR.IOC.Dependencies
{
    public static class ServiciosDependency
    {
        public static void AddServiciosDependency(this IServiceCollection service)
        {
            service.AddScoped<IServiciosRepository, ServiciosRepository>();
            service.AddScoped<IServiciosService, ServiciosService>();
        }
    }
}
