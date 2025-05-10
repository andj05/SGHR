using Microsoft.EntityFrameworkCore;
using SGHR.Persistence.Context;
using SGHR.Persistence.Configurations;
using SGHR.IOC.Dependencies.Users;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Infraestructure.Logging.Base;

namespace SGHR.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar el DbContext
            builder.Services.AddDbContext<SGHRContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));

            // Inyección del MessageMapper como Singleton
            builder.Services.AddSingleton<MessageMapper>();
            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();


            // Inyecciones de dependencias personalizadas
            builder.Services.AddClienteDependency();
            builder.Services.AddUsuarioDependency();

            builder.Services.AddControllers();

            // Configuración de CORS para permitir conexiones desde el frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy.WithOrigins("http://localhost:5173")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader()
                                    .AllowCredentials());
            });

            // Configuración de Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configurar el pipeline de la API
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Habilitar CORS antes de Authorization
            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
