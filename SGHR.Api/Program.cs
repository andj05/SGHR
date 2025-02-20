
using Microsoft.EntityFrameworkCore;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<SGHRContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBHotel")));

            builder.Services.AddScoped<IHabitacionRepository, HabitacionRepository>();

            builder.Services.AddScoped<IRecepcionRepository, RecepcionRepository>();

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
