using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MobileTracker.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        [ObservableProperty] private string? email = string.Empty;
        [ObservableProperty] private string? password = string.Empty;
        [ObservableProperty] private string? confirmPassword = string.Empty;
        [ObservableProperty] private string? errorMessage = string.Empty;
        [ObservableProperty] private bool hasError;
        [ObservableProperty] private bool isBusy;

        public RegistrationViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Register()
        {
            IsBusy = true;
            HasError = false;
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "All fields are required.";
                    HasError = true;
                    return;
                }
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    HasError = true;
                    return;
                }
                var user = await _authService.RegisterAsync(Email!, Password!);
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
        private async Task GoToLogin()
        {
            // Prefer popping the navigation stack when using NavigationPage root
            var nav = Application.Current?.Windows?.FirstOrDefault()?.Page?.Navigation;
            if (nav != null && nav.NavigationStack.Count > 0)
            {
                await nav.PopAsync();
                return;
            }

            try
            {
                var loginPage = App.Services?.GetService<MobileTracker.Views.LoginPage>();
                if (loginPage != null && nav != null)
                {
                    await nav.PushAsync(loginPage);
                    return;
                }
            }
            catch
            {
                // fallback to Shell navigation
            }

            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync(nameof(MobileTracker.Views.LoginPage));
            }
        }
    }
}
