using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracker.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        [ObservableProperty] private string? email = string.Empty;
        [ObservableProperty] private string? password = string.Empty;
        [ObservableProperty] private string? errorMessage = string.Empty;
        [ObservableProperty] private bool hasError;
        [ObservableProperty] private bool isBusy;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            IsBusy = true;
            HasError = false;
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Email and password are required.";
                    HasError = true;
                    return;
                }
                var user = await _authService.LoginAsync(Email!, Password!);
                // Navigate to dashboard/main app
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            // If app is using Shell this will work. For the initial unauthenticated flow
            // we start a NavigationPage (not Shell) so Shell.Current is null. Prefer pushing
            // the DI-resolved RegistrationPage onto the current MainPage navigation stack.
            try
            {
                var regPage = App.Services?.GetService<MobileTracker.Views.RegistrationPage>();
                var nav = Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
                if (regPage != null && nav != null)
                {
                    await nav.PushAsync(regPage);
                    return;
                }
            }
            catch
            {
                // ignore and fallback to Shell navigation
            }

            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync(nameof(MobileTracker.Views.RegistrationPage));
            }
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            IsBusy = true;
            HasError = false;
            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Email is required.";
                    HasError = true;
                    return;
                }
                await _authService.SendPasswordResetEmailAsync(Email!);
                ErrorMessage = "Password reset email sent.";
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
