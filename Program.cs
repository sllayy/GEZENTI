using GeziRotasi.API.Data;
using GeziRotasi.API.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;
using GeziRotasi.API.Repositories.Categories; // ICategoryRepository için

var builder = WebApplication.CreateBuilder(args);

// 1. Loglama (Serilog) Yapılandırması
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));

try
{
    Log.Information("Uygulama servisleri yapılandırılıyor...");

    // 2. Servisleri Ekleme (Dependency Injection)
    
    // Veritabanı Bağlantısı
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
    
    // HTTP İstekleri için
    builder.Services.AddHttpClient();
    
    // API Controller'ları ve JSON Ayarları
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
    
    // API Dokümantasyonu (Swagger)
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    
    // Kendi yazdığımız servisler
    builder.Services.AddScoped<OsmService>();
    builder.Services.AddScoped<PoiService>();
    builder.Services.AddScoped<ICategoryRepository, InMemoryCategoryRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    // Buraya diğer servisleriniz de (IRouteService vb.) eklenecek

    // 3
