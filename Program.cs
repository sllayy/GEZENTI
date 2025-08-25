using GeZenti.Api.Services;
using GeziRotasi.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Servislerin Kaydedildiði Alan ---

// Sena'nýn eklediði: OsmService'in çalýþmasý için gerekli.
builder.Services.AddHttpClient();
builder.Services.AddScoped<OsmService>();
// PoiService kaydý buradan kaldýrýldý, çünkü o dosya henüz yok.

builder.Services.AddHttpClient<IRouteService, RouteService>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ... (dosyanýn geri kalaný ayný) ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();