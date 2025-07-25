using CronJobCleaner.Data;
using CronJobCleaner.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CronJobCleaner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/logs
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.TemporaryLogs
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return Ok(logs);
        }

        // POST: api/logs
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] TemporaryLog log)
        {
            log.CreatedAt = DateTime.UtcNow;
            _context.TemporaryLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLogs), new { id = log.Id }, log);
        }
    }
}
