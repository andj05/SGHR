using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Reservation
{
    public static class HabitacionDependency
    {
        public static void AddHabitacionDependency(this IServiceCollection service)
        {
            service.AddScoped<IHabitacionRepository, HabitacionRepository>();
            service.AddTransient<IHabitacionService, HabitacionService>();
        }       
    }
}
