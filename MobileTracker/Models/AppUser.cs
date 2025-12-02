namespace MobileTracker.Models
{
    public class AppUser
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public AccountStatus AccountStatus { get; set; }
    }

    public enum AccountStatus
    {
        Active,
        Locked
    }
}