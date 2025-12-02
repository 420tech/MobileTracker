using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Linq;
using Microsoft.Maui.ApplicationModel;
using System.Threading;

namespace MobileTracker.Services
{
    // Implementation using AppShell (Shell.Current) at runtime.
    public class ShellNavigationService : INavigationService
    {
        public Task NavigateToAsync(string route)
        {
            // Prefer Shell navigation when available
            if (Shell.Current != null)
                return Shell.Current.GoToAsync(route);

            // Fall back to NavigationPage flow when Shell isn't in use (e.g., unauthenticated flow)
            var nav = Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
            if (nav != null)
            {
                var routeName = route.Trim('/');

                // Common simple routes mapping
                if (string.Equals(routeName, "RegistrationPage", System.StringComparison.OrdinalIgnoreCase))
                {
                    var page = App.Services?.GetService<MobileTracker.Views.RegistrationPage>();
                    if (page != null)
                        return nav.PushAsync(page);
                }

                if (string.Equals(routeName, "LoginPage", System.StringComparison.OrdinalIgnoreCase))
                {
                    var page = App.Services?.GetService<MobileTracker.Views.LoginPage>();
                    if (page != null)
                        return nav.PushAsync(page);
                }

                // Navigating to dashboard in a non-shell flow means switching root to AppShell
                if (string.Equals(routeName, "DashboardPage", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Switch to Shell-rooted app on main thread
                    MainThread.BeginInvokeOnMainThread(() => Application.Current.MainPage = new AppShell());
                    return Task.CompletedTask;
                }

                // fallback noop
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        public Task NavigateToAsync(string route, object parameter)
        {
            // For now, ignore the parameter and call the single-arg overload
            return NavigateToAsync(route);
        }

        public Task GoBackAsync()
        {
            return Shell.Current?.GoToAsync("..") ?? Task.CompletedTask;
        }
    }
}
