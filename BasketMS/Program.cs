using BlazorShared.Entities;
using BasketMS.Interfaces;
using BasketMS.Repository;
using BasketMS.Services;

namespace BasketMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddScoped<IAppLogger<BasketService>, AppLogger<BasketService>>();
            builder.Services.AddScoped<IRepository<Basket>, BasketRepository>();
            builder.Services.AddScoped<IBasketService, BasketService>();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
