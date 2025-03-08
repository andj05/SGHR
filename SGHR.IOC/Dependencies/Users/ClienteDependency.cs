using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Intefaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Users
{
    public static class ClienteDependency
    {
        public static void AddClienteDependency(this IServiceCollection services)
        {
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddTransient<IClientesService, ClientesService>();
        }
    }
}
