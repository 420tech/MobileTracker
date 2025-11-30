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
        AppUser GetCurrentUser();
        bool IsAuthenticated { get; }
    }
}