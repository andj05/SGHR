using Microsoft.EntityFrameworkCore;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.IOC.Dependencies.Reservation;

namespace SGHR.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<SGHRContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));

            // Inyección del MessageMapper como Singleton
            builder.Services.AddSingleton<MessageMapper>();

            builder.Services.AddHabitacionDependency();

            builder.Services.AddRecepcionDependency();

            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
