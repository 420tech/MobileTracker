using System;
using System.Threading.Tasks;
using MobileTracker.Services;
using MobileTracker.ViewModels;
using Moq;
using Xunit;

namespace MobileTracker.UnitTests.ViewModels
{
    public class LoginViewModelTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<MobileTracker.Services.INavigationService> _mockNavigationService;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockNavigationService = new Mock<MobileTracker.Services.INavigationService>();
            _viewModel = new LoginViewModel(_mockAuthService.Object, _mockNavigationService.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_NavigatesToDashboard()
        {
            // Arrange
            _viewModel.Email = "test@example.com";
            _viewModel.Password = "password123";

            var mockUser = new MobileTracker.Models.AppUser
            {
                Uid = "test-uid",
                Email = "test@example.com",
                AccountStatus = MobileTracker.Models.AccountStatus.Active
            };

            _mockAuthService
                .Setup(x => x.LoginAsync("test@example.com", "password123"))
                .ReturnsAsync(mockUser);

            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.False(_viewModel.HasError);
            // ViewModel initializes ErrorMessage as empty string for clean state
            Assert.Equal(string.Empty, _viewModel.ErrorMessage);
            _mockAuthService.Verify(x => x.LoginAsync("test@example.com", "password123"), Times.Once);
            _mockNavigationService.Verify(x => x.NavigateToAsync("//DashboardPage"), Times.Once);
        }

        [Fact]
        public async Task GoToRegister_InvokesNavigationService()
        {
            // Act
            await _viewModel.GoToRegisterCommand.ExecuteAsync(null);

            // Assert
            _mockNavigationService.Verify(x => x.NavigateToAsync("//RegistrationPage"), Times.Once);
        }

        [Fact]
        public async Task Login_WithEmptyEmail_ShowsError()
        {
            // Arrange
            _viewModel.Email = "";
            _viewModel.Password = "password123";

            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.HasError);
            Assert.Equal("Email and password are required.", _viewModel.ErrorMessage);
            _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ShowsError()
        {
            // Arrange
            _viewModel.Email = "test@example.com";
            _viewModel.Password = "";

            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.HasError);
            Assert.Equal("Email and password are required.", _viewModel.ErrorMessage);
            _mockAuthService.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithAuthServiceException_ShowsError()
        {
            // Arrange
            _viewModel.Email = "test@example.com";
            _viewModel.Password = "password123";

            _mockAuthService
                .Setup(x => x.LoginAsync("test@example.com", "password123"))
                .ThrowsAsync(new Exception("Invalid credentials"));

            // Act
            await _viewModel.LoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.HasError);
            Assert.Equal("Invalid credentials", _viewModel.ErrorMessage);
        }

        [Fact]
        public async Task ForgotPassword_WithValidEmail_SendsResetEmail()
        {
            // Arrange
            _viewModel.Email = "test@example.com";

            _mockAuthService
                .Setup(x => x.SendPasswordResetEmailAsync("test@example.com"))
                .Returns(Task.CompletedTask);

            // Act
            await _viewModel.ForgotPasswordCommand.ExecuteAsync(null);

            // Assert success message is set
            Assert.True(_viewModel.HasSuccessMessage);
            Assert.Equal("Password reset email sent. Please check your email and follow the instructions.", _viewModel.SuccessMessage);
            _mockAuthService.Verify(x => x.SendPasswordResetEmailAsync("test@example.com"), Times.Once);
        }

        [Fact]
        public async Task ForgotPassword_WithEmptyEmail_ShowsError()
        {
            // Arrange
            _viewModel.Email = "";

            // Act
            await _viewModel.ForgotPasswordCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.HasError);
            Assert.Equal("Email is required.", _viewModel.ErrorMessage);
            _mockAuthService.Verify(x => x.SendPasswordResetEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPassword_WithAuthServiceException_ShowsError()
        {
            // Arrange
            _viewModel.Email = "test@example.com";

            _mockAuthService
                .Setup(x => x.SendPasswordResetEmailAsync("test@example.com"))
                .ThrowsAsync(new Exception("Email not found"));

            // Act
            await _viewModel.ForgotPasswordCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.HasError);
            Assert.Equal("Email not found", _viewModel.ErrorMessage);
        }

        
    }
}