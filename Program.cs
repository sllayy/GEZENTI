using GeZenti.Api.Services;
using GeziRotasi.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Servislerin Kaydedildi�i Alan ---

// Sena'n�n ekledi�i: OsmService'in �al��mas� i�in gerekli.
builder.Services.AddHttpClient();
builder.Services.AddScoped<OsmService>();
// PoiService kayd� buradan kald�r�ld�, ��nk� o dosya hen�z yok.

builder.Services.AddHttpClient<IRouteService, RouteService>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ... (dosyan�n geri kalan� ayn�) ...

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();