using SGHR.WebApi.Models.Categorias;
using SGHR.WebApi.Models.EstadoHabitacion;
using SGHR.WebApi.Models.Piso;
using SGHR.WebApi.Models.RolUsuario;
using SGHR.WebApi.Models.Servicios;
using SGHR.WebApi.Models.Tarifas;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.PersistenApi.Repository;
using SGHR.WebApi.PersistenceApi.Configuration;
using SGHR.WebApi.PersistenceApi.Repository;
using SGHR.WebApi.ServicesApi.Interface;
using SGHR.WebApi.ServicesApi.Service;

namespace SGHR.WebApi.IOCApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Register configuration
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            // Register HttpClient without BaseAddress
            services.AddHttpClient<IRepository<TarifasApiModel>, TarifasRepository>();
            services.AddHttpClient<IRepository<ServiciosApiModel>, ServiciosRepository>();
            services.AddHttpClient<IRepository<RolUsuarioApiModel>, RolUsuarioRepository>();
            services.AddHttpClient<IRepository<PisoApiModel>,PisoRepository>();
            services.AddHttpClient<IRepository<EstadoHabitacionApiModel>, EstadoHabitacionRepository>();
            services.AddHttpClient<IRepository<CategoriasApiModel>, CategoriasRepository>();



            // Register Repositories
            services.AddScoped<IRepository<TarifasApiModel>, TarifasRepository>();
            services.AddScoped<IRepository<ServiciosApiModel>, ServiciosRepository>();
            services.AddScoped<IRepository<RolUsuarioApiModel>, RolUsuarioRepository>();
            services.AddScoped<IRepository<PisoApiModel>, PisoRepository>();
            services.AddScoped<IRepository<EstadoHabitacionApiModel>, EstadoHabitacionRepository>();
            services.AddScoped<IRepository<CategoriasApiModel>, CategoriasRepository>();


            // Register Services
            services.AddScoped<ITarifasService, TarifasService>();
            services.AddScoped<IServiciosService, ServiciosService>();
            services.AddScoped<IRolUsuarioService, RolUsuarioService>();
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<IEstadoHabitacionService, EstadoHabitacionService>();
            services.AddScoped<ICategoriasService, CategoriasService>();


            // Register Error Message Service
            services.AddSingleton<IErrorMessageService, ErrorMessageService>();

            services.AddScoped(typeof(ILoggerManger<>), typeof(LoggerManager<>));

            // Add other dependencies as needed

            return services;
        }
    }
}
