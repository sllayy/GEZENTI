using GeziRotasi.API.Data;
using GeziRotasi.API.Repositories.Categories;
using GeziRotasi.API.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

        try
        {
            Log.Information("Uygulama başlatılıyor...");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

            builder.Services.AddHttpClient();
            builder.Services.AddScoped<OsmService>();
            builder.Services.AddScoped<PoiService>();
            builder.Services.AddScoped<ICategoryRepository, InMemoryCategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddControllers();

            // 🔑 Buraya dikkat

            {
                builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            };

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

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
        catch (Exception ex)
        {
            Log.Fatal(ex, "Uygulama başlatılırken ölümcül bir hata oluştu.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}