using Microsoft.EntityFrameworkCore;
using SGHR.Persistence.Context;
using SGHR.IOC.Dependencies.Reservation;
using SGHR.Persistence.Configurations;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Infraestructure.Logging.Base;

namespace SGHR.Api
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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
