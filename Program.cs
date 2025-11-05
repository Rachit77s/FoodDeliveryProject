using FoodDeliveryPolaris.Data;
using FoodDeliveryPolaris.Repositories;
using FoodDeliveryPolaris.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryPolaris
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure Entity Framework Core with SQL Server
            builder.Services.AddDbContext<FoodDeliveryDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            // Register repositories as scoped (best practice for database operations)
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRiderRepository, RiderRepository>();
            builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Register services as scoped (standard practice for business logic)
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRiderService, RiderService>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IFoodRecommendationService, FoodRecommendationService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Food Delivery Polaris API",
                    Version = "v1",
                    Description = "RESTful API for food delivery system with full CRUD operations for Users, Riders, and Restaurants"
                });
            });

            var app = builder.Build();

            // Apply migrations, create database, and seed data on startup (Development only)
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    
                    try
                    {
                        var dbContext = services.GetRequiredService<FoodDeliveryDbContext>();
                        
                        // Apply any pending migrations
                        logger.LogInformation("Applying database migrations...");
                        dbContext.Database.Migrate();
                        logger.LogInformation("Database migrations applied successfully.");
                        
                        // Seed sample data
                        logger.LogInformation("Checking if database seeding is needed...");
                        DatabaseSeeder.SeedDatabase(dbContext);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    }
                }
            }

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
