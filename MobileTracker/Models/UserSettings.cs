namespace MobileTracker.Models
{
    public class UserSettings
    {
        public string? Id { get; set; }
        public string? Timezone { get; set; }
        public string? Currency { get; set; }
        public decimal? DefaultRate { get; set; }
        public bool ReceiveNotifications { get; set; } = true;

        public void Validate()
        {
            // No strict validation here; settings are mostly optional
        }
    }
}
