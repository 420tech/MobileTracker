using System;

namespace MobileTracker.Models
{
    public class TimeEntry
    {
        public string? Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string[]? Tags { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
                throw new ArgumentException("ClientId is required", nameof(ClientId));
            if (DurationMinutes < 0)
                throw new ArgumentOutOfRangeException(nameof(DurationMinutes), "DurationMinutes cannot be negative");
        }
    }
}
