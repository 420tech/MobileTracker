using System.Threading.Tasks;
using MobileTracker.Models;

namespace MobileTracker.Services
{
    public interface IAuthService
    {
        Task<AppUser> RegisterAsync(string email, string password);
        Task<AppUser> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task SendPasswordResetEmailAsync(string email);
        // Return the current Firebase ID token for authenticated requests or null if not authenticated
        Task<string?> GetIdTokenAsync();
        AppUser GetCurrentUser();
        bool IsAuthenticated { get; }
    }
}