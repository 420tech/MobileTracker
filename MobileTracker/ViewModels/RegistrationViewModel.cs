using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MobileTracker.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;
        [ObservableProperty] private string? email = string.Empty;
        [ObservableProperty] private string? password = string.Empty;
        [ObservableProperty] private string? confirmPassword = string.Empty;
        [ObservableProperty] private string? errorMessage = string.Empty;
        [ObservableProperty] private bool hasError;
        [ObservableProperty] private bool isBusy;

        public RegistrationViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
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

                if (!IsValidEmail(Email!))
                {
                    ErrorMessage = "Invalid email format.";
                    HasError = true;
                    return;
                }

                if (!IsValidPassword(Password!))
                {
                    ErrorMessage = "Password must be at least 8 characters with letters and numbers.";
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
                // Navigate to dashboard via navigation service
                await _navigationService.NavigateToAsync("//DashboardPage");
            }
            catch (Exception ex)
            {
                ErrorMessage = GetUserFriendlyErrorMessage(ex.Message);
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            // RFC 5322 compliant email validation
            var emailRegex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            return emailRegex.IsMatch(email);
        }

        private bool IsValidPassword(string password)
        {
            // Minimum 8 characters with at least one letter and one number
            return password.Length >= 8 && Regex.IsMatch(password, @"[a-zA-Z]") && Regex.IsMatch(password, @"[0-9]");
        }

        private string GetUserFriendlyErrorMessage(string errorMessage)
        {
            // Convert Firebase error messages to user-friendly messages
            if (errorMessage.Contains("EMAIL_EXISTS"))
            {
                return "Email already registered";
            }
            else if (errorMessage.Contains("INVALID_EMAIL"))
            {
                return "Invalid email format";
            }
            else if (errorMessage.Contains("WEAK_PASSWORD"))
            {
                return "Password is too weak";
            }
            else if (errorMessage.Contains("OPERATION_NOT_ALLOWED"))
            {
                return "Registration is currently disabled";
            }
            else if (errorMessage.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
            {
                return "Too many attempts. Please try again later";
            }
            else
            {
                return "Registration failed. Please try again";
            }
        }

        [RelayCommand]
        private async Task GoToLogin()
        {
            // Use navigation service to go to login
            await _navigationService.NavigateToAsync("//LoginPage");
        }
    }
}
