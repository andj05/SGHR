

using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Persistence.Repository;
using SGHR.Application.Services;

namespace SGHR.IOC.Dependencies
{
    public static class TarifasDependency
    {
        public static void AddTarifasDependency(this IServiceCollection service)
        {
            service.AddScoped<ITarifasRepository, TarifasRepository>();
            service.AddScoped<ITarifasService, TarifasService>();
        }
    }
}
