using System;
using System.Threading;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MobileTracker.Models;
using MobileTracker.Services;
using Moq;
using Xunit;

namespace MobileTracker.UnitTests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly FirebaseAuthService _authService;

        public AuthServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _authService = new FirebaseAuthService(_httpClient);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var expectedUid = "test-uid-123";
            var expectedIdToken = "test-id-token";

            var responseContent = $@"
            {{
                ""kind"": ""identitytoolkit#VerifyPasswordResponse"",
                ""localId"": ""{expectedUid}"",
                ""email"": ""{email}"",
                ""displayName"": """",
                ""idToken"": ""{expectedIdToken}"",
                ""registered"": true,
                ""refreshToken"": ""test-refresh-token"",
                ""expiresIn"": ""3600""
            }}";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUid, result.Uid);
            Assert.Equal(email, result.Email);
            Assert.Equal(AccountStatus.Active, result.AccountStatus);
            Assert.True(result.LastLoginDate > DateTime.MinValue);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsException()
        {
            // Arrange
            var email = "invalid@example.com";
            var password = "wrongpassword";

            var responseContent = $@"
            {{
                ""error"": {{
                    ""code"": 400,
                    ""message"": ""INVALID_LOGIN_CREDENTIALS"",
                    ""errors"": [
                        {{
                            ""domain"": ""global"",
                            ""reason"": ""invalid"",
                            ""message"": ""INVALID_LOGIN_CREDENTIALS""
                        }}
                    ]
                }}
            }}";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(responseContent)
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(email, password));
            Assert.Contains("INVALID_LOGIN_CREDENTIALS", exception.Message);
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_WithValidEmail_SendsResetEmail()
        {
            // Arrange
            var email = "test@example.com";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            await _authService.SendPasswordResetEmailAsync(email);

            // Assert
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString().Contains("sendOobCode")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_WithInvalidEmail_ThrowsException()
        {
            // Arrange
            var email = "invalid@example.com";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(@"
                    {
                        ""error"": {
                            ""code"": 400,
                            ""message"": ""EMAIL_NOT_FOUND""
                        }
                    }")
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.SendPasswordResetEmailAsync(email));
            Assert.Contains("EMAIL_NOT_FOUND", exception.Message);
        }
    }
}