using GeziRotasi.API.Services;
using Serilog;
using GeziRotasi.API.Data; // AppDbContext'in yaşadığı yer
using Microsoft.EntityFrameworkCore; // UseNpgsql için gerekli
using System.Text.Json.Serialization;

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
    // VERİTABANI BAĞLANTISI 
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
    
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Enum'ları metin olarak (string) işlemesini sağlayan dönüştürücüyü ekliyoruz.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // ReferenceHandler.IgnoreCycles, bir döngü tespit ettiğinde
        // o referansı görmezden gelerek sonsuz döngüyü engeller.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

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