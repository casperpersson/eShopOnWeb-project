using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ProductCatalogMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Entity Framework
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/root/.aspnet/DataProtection-Keys"));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CatalogContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Only ensure database exists (won't recreate if it exists)
        var created = await context.Database.EnsureCreatedAsync();
        if (created)
        {
            logger.LogInformation("Database created successfully");
        }
        else
        {
            logger.LogInformation("Database already exists");
        }

        // Seed the database
        await CatalogContextSeed.SeedAsync(context, logger);
        logger.LogInformation("Database initialization completed");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred initializing the database.");
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
