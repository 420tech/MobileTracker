using System;

namespace MobileTracker.Models
{
    public class Client
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Client name is required", nameof(Name));
            if (!string.IsNullOrWhiteSpace(ContactEmail) && !ContactEmail.Contains("@"))
                throw new ArgumentException("ContactEmail is invalid", nameof(ContactEmail));
        }
    }
}
