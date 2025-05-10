using Microsoft.EntityFrameworkCore;
using SGHR.Persistence.Context;
using SGHR.IOC.Dependencies.Users;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;

namespace SGHR.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Agregar el DbContext
            builder.Services.AddDbContext<SGHRContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));

            // Inyección del MessageMapper como Singleton
            builder.Services.AddSingleton<MessageMapper>();
            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();


            // Inyecciones de dependencias personalizadas
            builder.Services.AddClienteDependency();
            builder.Services.AddUsuarioDependency();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }

    }
}