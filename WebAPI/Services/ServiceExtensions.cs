using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Models.Cliente;
using WebAPI.Models.Usuario;
using WebAPI.Models.Interfaces;
using WebAPI.Services;
using WebAPI.Interfaces;

namespace WebAPI.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<BaseApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5187/api/");
            });

            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IRepository<ClienteModel>, ClienteRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }
    }
}
