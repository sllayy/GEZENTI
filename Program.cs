using GeziRotasi.API.Data;
using GeziRotasi.API.Entities;
using GeziRotasi.API.Services;
using GeziRotasi.API.Models;
using GeziRotasi.API.Repositories.Categories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using GeziRotasi.API.Configurations;
using GeziRotasi.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// 1. Loglama (Serilog)
// ----------------------
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));

// ----------------------
// 2. Config binding
// ----------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ----------------------
// 3. Database
// ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------
// 4. Identity
// ----------------------
builder.Services.AddIdentity<AppUser, IdentityRole<int>>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.AllowedForNewUsers = true;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ----------------------
// 5. Auth - JWT + Firebase
// ----------------------
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

var fbCredPath = builder.Configuration["Firebase:CredentialFile"];
if (!string.IsNullOrEmpty(fbCredPath) && File.Exists(fbCredPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(fbCredPath)
    });
}

builder.Services.AddHostedService<ExpiredEmailCodeCleanupService>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = key,
        ClockSkew = TimeSpan.Zero
    };
});

// ----------------------
// 6. CORS (React için)
// ----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:") ||
                origin.StartsWith("https://localhost:") ||
                origin.StartsWith("http://127.0.0.1:") ||
                origin.StartsWith("https://127.0.0.1:")
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
        else
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:7248",
                "http://localhost:7248"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        }
    });
});

// ----------------------
// 7. Custom Services
// ----------------------
builder.Services.AddHttpClient<IRouteService, RouteService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<ICodeService, CodeService>();
builder.Services.AddHttpClient<OsmService>();
builder.Services.AddScoped<OsmService>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMapRouteService, MapRouteService>();
builder.Services.AddScoped<UserPreferencesService>();

// ----------------------
// 8. Controllers & Swagger
// ----------------------
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gezenti API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
});

// ----------------------
// 9. Build & Pipeline
// ----------------------
var app = builder.Build();

try
{
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }

    app.UseCors("AllowFrontend");
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // --- Migration otomatik çalıştırma + Seed veriler ---
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        // Eğer TravelRoutes tablosu boşsa kategorileri ekle
        if (!db.TravelRoutes.Any())
        {
            db.TravelRoutes.AddRange(
                new TravelRoute { Category = "Tarih", AverageRating = 4.5, DistanceKm = 10, Duration = "short" },
                new TravelRoute { Category = "Yemek", AverageRating = 4.7, DistanceKm = 5, Duration = "short" },
                new TravelRoute { Category = "Müzik", AverageRating = 4.6, DistanceKm = 12, Duration = "medium" },
                new TravelRoute { Category = "Müze", AverageRating = 4.8, DistanceKm = 8, Duration = "short" },
                new TravelRoute { Category = "Alışveriş", AverageRating = 4.4, DistanceKm = 15, Duration = "medium" },
                new TravelRoute { Category = "Eğlence", AverageRating = 4.9, DistanceKm = 20, Duration = "long" },
                new TravelRoute { Category = "Sanat", AverageRating = 4.3, DistanceKm = 7, Duration = "short" },
                new TravelRoute { Category = "Sahil", AverageRating = 4.6, DistanceKm = 30, Duration = "medium" },
                new TravelRoute { Category = "Doğa", AverageRating = 4.9, DistanceKm = 50, Duration = "long" }
            );
            db.SaveChanges();
        }
    }


    Log.Information("Uygulama başarıyla başlatıldı 🚀");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılırken kritik hata oluştu");
}
finally
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // db.Database.Migrate();
    }

    Log.CloseAndFlush();
}
public partial class Program { }