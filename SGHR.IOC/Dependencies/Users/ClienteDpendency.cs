
using Microsoft.Extensions.DependencyInjection;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.IOC.Dependencies.Users
{
    public static class ClienteDpendency
    {
        public static void AddClienteDependency(this IServiceCollection services)
        {
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddTransient<IClienteRepository, ClienteRepository>();
        }
    }
}
