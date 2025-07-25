using CronJobCleaner.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CronJobCleaner.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TemporaryLog> TemporaryLogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
