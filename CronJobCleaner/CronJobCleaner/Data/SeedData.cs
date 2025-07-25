using CronJobCleaner.Entities;

namespace CronJobCleaner.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Zaten veriler eklenmişse tekrar ekleme
            if (context.TemporaryLogs.Any())
                return;

            var now = DateTime.UtcNow;

            context.TemporaryLogs.AddRange(
                new TemporaryLog
                {
                    Message = "Bugünkü log kaydı",
                    CreatedAt = now
                },
                new TemporaryLog
                {
                    Message = "Dünkü log kaydı",
                    CreatedAt = now.AddDays(-1)
                },
                new TemporaryLog
                {
                    Message = "2 gün önceki log kaydı",
                    CreatedAt = now.AddDays(-2)
                }
            );

            context.SaveChanges();
        }
    }
}
