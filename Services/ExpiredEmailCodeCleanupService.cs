using GeziRotasi.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GeziRotasi.API.Services
{
    public class ExpiredEmailCodeCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredEmailCodeCleanupService> _logger;

        public ExpiredEmailCodeCleanupService(IServiceScopeFactory scopeFactory, ILogger<ExpiredEmailCodeCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var now = DateTime.UtcNow;

                    var expired = await db.EmailCodes
                        .Where(x => x.CreatedAtUtc.AddMinutes(15) < now)
                        .ToListAsync(stoppingToken);

                    if (expired.Any())
                    {
                        db.EmailCodes.RemoveRange(expired);
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"{expired.Count} expired email codes deleted at {DateTime.UtcNow}.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up expired email codes.");
                }

                // 5 dakikada bir kontrol et
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
