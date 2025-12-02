using System.Threading.Tasks;

namespace MobileTracker.Services
{
    // Simple navigation abstraction so ViewModels don't reference MAUI types directly.
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, object parameter);
        Task GoBackAsync();
    }
}
