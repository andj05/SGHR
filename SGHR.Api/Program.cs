using Microsoft.EntityFrameworkCore;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;
using SGHR.IOC.Dependencies.Users;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;
namespace SGHR.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<SGHRContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));

            builder.Services.AddClienteDependency();
            builder.Services.AddUsuarioDependency();
            builder.Services.AddControllers();

            // Habilitar CORS para permitir conexiones desde el frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder => builder.WithOrigins("http://localhost:5173") // URL de tu frontend
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