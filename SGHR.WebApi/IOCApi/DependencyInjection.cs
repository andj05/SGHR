using SGHR.WebApi.Models.Habitacion;
using SGHR.WebApi.Models.Recepcion;
using SGHR.WebApi.PersistenceApi.Configuration;
using SGHR.WebApi.PersistenceApi.Interface;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.ServicesApi.Service;

namespace SGHR.WebApi.IOCApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
            services.AddHttpClient<IRepository<HabitacionModel>, HabitacionRepository>();
            services.AddHttpClient<IRepository<RecepcionModel>, RecepcionRepository>();
            services.AddScoped<IRepository<HabitacionModel>, HabitacionRepository>();
            services.AddScoped<IRepository<RecepcionModel>, RecepcionRepository>();
            services.AddScoped<IHabitacionService, HabitacionService>();
            services.AddScoped<IRecepcionService, RecepcionService>();
            services.AddSingleton<IErrorMessageService, ErrorMessageService>();
            services.AddScoped(typeof(ILoggerManager<>), typeof(LoggerManager<>));
            return services;
        }
    }
}
