﻿namespace CronJobCleaner.Entities
{
    public class TemporaryLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
