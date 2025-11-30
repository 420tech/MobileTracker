using System.Threading.Tasks;
using MobileTracker.Models;

namespace MobileTracker.Services
{
    // Stub implementation for development/testing
    public class FirebaseAuthService : IAuthService
    {
        private AppUser _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public Task<AppUser> RegisterAsync(string email, string password)
        {
            // TODO: Integrate with Firebase SDK
            _currentUser = new AppUser { Uid = "stub-uid", Email = email };
            return Task.FromResult(_currentUser);
        }

        public Task<AppUser> LoginAsync(string email, string password)
        {
            // TODO: Integrate with Firebase SDK
            _currentUser = new AppUser { Uid = "stub-uid", Email = email };
            return Task.FromResult(_currentUser);
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email)
        {
            // TODO: Integrate with Firebase SDK
            return Task.CompletedTask;
        }

        public AppUser GetCurrentUser() => _currentUser;
    }
}