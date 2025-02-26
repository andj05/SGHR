using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;


namespace SGHR.IOC.Dependencies
{
    public static class CategoriasDependency
    {
        public static void AddCategoriasDependency(this IServiceCollection service)
        {
            service.AddScoped<ICategoriaRepository, CategoriaRepository>();
            service.AddScoped<ICategoriasService, CategoriasService>();
        }
    }
}
