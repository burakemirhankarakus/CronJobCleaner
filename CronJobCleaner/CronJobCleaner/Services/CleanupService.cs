using CronJobCleaner.Data;
using Microsoft.EntityFrameworkCore;
using Cronos;

namespace CronJobCleaner.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupService> _logger;
        private readonly CronExpression _cron;

        public CleanupService(IServiceScopeFactory scopeFactory, ILogger<CleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            // Her dakikanın başında çalışacak cron
            _cron = CronExpression.Parse("* * * * *", CronFormat.Standard);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var next = _cron.GetNextOccurrence(now);

                if (next.HasValue)
                {
                    var delay = next.Value - now;

                    // Beklenilen cron zamanına kadar bekle
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, stoppingToken);

                    // Tam zamanı geldiğinde çalıştır
                    _logger.LogInformation(" Temizlik başlatıldı: {time}", DateTime.UtcNow);
                    await DoCleanupAsync();
                }
            }
        }

        private async Task DoCleanupAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            _logger.LogInformation(" Toplam kayıt sayısı: {count}", db.TemporaryLogs.Count());

            var silinecekler = await db.TemporaryLogs
                .Where(x => x.CreatedAt < DateTime.UtcNow.AddMinutes(-1)) // veya AddDays(-1) prod için
                .ToListAsync();

            if (silinecekler.Any())
            {
                db.TemporaryLogs.RemoveRange(silinecekler);
                await db.SaveChangesAsync();

                foreach (var item in silinecekler)
                {
                    _logger.LogInformation(" Silindi: {msg} ({date})", item.Message, item.CreatedAt);
                }
            }
            else
            {
                _logger.LogInformation(" Silinecek kayıt bulunamadı. Saat: {now}", DateTime.UtcNow);
            }
        }
    }
}
