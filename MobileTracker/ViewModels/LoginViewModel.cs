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
        private readonly INavigationService _navigationService;
        [ObservableProperty] private string? email = string.Empty;
        [ObservableProperty] private string? password = string.Empty;
        [ObservableProperty] private string? errorMessage = string.Empty;
        [ObservableProperty] private bool hasError;
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isAlreadyAuthenticated;
        [ObservableProperty] private bool hasSuccessMessage;
        [ObservableProperty] private string? successMessage = string.Empty;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            CheckAuthenticationState();
        }

        private void CheckAuthenticationState()
        {
            // Do not reference MAUI Shell directly here to keep ViewModel platform-agnostic (helps unit testing).
            IsAlreadyAuthenticated = _authService.IsAuthenticated;
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
                // Navigate to the dashboard using the registered navigation service
                await _navigationService.NavigateToAsync("//DashboardPage");
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
            // Navigate to registration page via navigation service
            await _navigationService.NavigateToAsync("//RegistrationPage");
        }

        [RelayCommand]
        private async Task ForgotPassword()
        {
            IsBusy = true;
            HasError = false;
            HasSuccessMessage = false;
            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Email is required.";
                    HasError = true;
                    return;
                }
                await _authService.SendPasswordResetEmailAsync(Email!);
                SuccessMessage = "Password reset email sent. Please check your email and follow the instructions.";
                HasSuccessMessage = true;
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
