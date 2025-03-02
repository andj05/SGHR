using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Reservation
{
    public static class RecepcionDependency
    {
        public static void AddRecepcionDependency(this IServiceCollection service)
        {
            service.AddScoped<IRecepcionRepository, RecepcionRepository>();
            service.AddTransient<IRecepcionService, RecepcionService>();
        }   
    }
}
