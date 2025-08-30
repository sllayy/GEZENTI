using GeziRotasi.API.Data;
using GeziRotasi.API.Entities;
using GeziRotasi.API.Services;
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("GezentiDb")));

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
if (string.IsNullOrEmpty(fbCredPath) || !File.Exists(fbCredPath))
{
    throw new FileNotFoundException("Firebase credential dosyası bulunamadı: " + fbCredPath);
}
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(fbCredPath)
});
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
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("client", p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// ----------------------
// 7. Custom Services
// ----------------------

// HttpClient Factory
builder.Services.AddHttpClient();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<ICodeService, CodeService>();

builder.Services.AddHttpClient<OsmService>();
builder.Services.AddScoped<OsmService>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddHttpClient<IRouteService, RouteService>();
builder.Services.AddScoped<RecommendationService>();

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

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("client");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- Migration otomatik çalıştırma ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

Log.Information("Uygulama başarıyla başlatıldı 🚀");
app.Run();

Log.CloseAndFlush();
