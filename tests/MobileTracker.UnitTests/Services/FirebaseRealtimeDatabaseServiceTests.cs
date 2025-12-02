using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MobileTracker.Models;
using MobileTracker.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace MobileTracker.UnitTests.Services
{
    public class FirebaseRealtimeDatabaseServiceTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;

        public FirebaseRealtimeDatabaseServiceTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            // Ensure test env has a base URL
            Environment.SetEnvironmentVariable("FIREBASE_DATABASE_URL", "https://testproject.firebaseio.com");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            Environment.SetEnvironmentVariable("FIREBASE_DATABASE_URL", null);
        }

        [Fact]
        public async Task GetAsync_ReturnsDeserializedObject()
        {
            // Arrange
            var user = new AppUser { Uid = "user-123", Email = "a@b.com" };
            var token = "tok-1";

            var authService = new Mock<IAuthService>();
            authService.Setup(a => a.GetCurrentUser()).Returns(user);
            authService.Setup(a => a.GetIdTokenAsync()).ReturnsAsync(token);

            var json = "{ \"foo\": \"bar\" }";

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(json) })
                .Verifiable();

            var svc = new FirebaseRealtimeDatabaseService(_httpClient, authService.Object);

            // Act
            var result = await svc.GetAsync<dynamic>("mydata");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("bar", (string)result.foo);

            _mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().Contains("users/user-123/mydata.json")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task SetAsync_SendsPutRequest()
        {
            var user = new AppUser { Uid = "user-99", Email = "a@b.com" };
            var token = "tok-2";

            var authService = new Mock<IAuthService>();
            authService.Setup(a => a.GetCurrentUser()).Returns(user);
            authService.Setup(a => a.GetIdTokenAsync()).ReturnsAsync(token);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK })
                .Verifiable();

            var svc = new FirebaseRealtimeDatabaseService(_httpClient, authService.Object);
            await svc.SetAsync("settings/theme", new { color = "blue" });

            _mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri!.ToString().Contains("users/user-99/settings/theme.json")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task PushAsync_ReturnsKeyFromResponse()
        {
            var user = new AppUser { Uid = "user-77", Email = "x@y.com" };
            var token = "tok-3";

            var authService = new Mock<IAuthService>();
            authService.Setup(a => a.GetCurrentUser()).Returns(user);
            authService.Setup(a => a.GetIdTokenAsync()).ReturnsAsync(token);

            var respJson = "{ \"name\": \"generated-abc\" }";

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(respJson) })
                .Verifiable();

            var svc = new FirebaseRealtimeDatabaseService(_httpClient, authService.Object);
            var key = await svc.PushAsync("items", new { name = "test" });

            Assert.Equal("generated-abc", key);
            _mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri!.ToString().Contains("users/user-77/items.json")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_SendsDeleteRequest()
        {
            var user = new AppUser { Uid = "user-4", Email = "u@v.com" };
            var token = "tok-4";

            var authService = new Mock<IAuthService>();
            authService.Setup(a => a.GetCurrentUser()).Returns(user);
            authService.Setup(a => a.GetIdTokenAsync()).ReturnsAsync(token);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK })
                .Verifiable();

            var svc = new FirebaseRealtimeDatabaseService(_httpClient, authService.Object);
            await svc.DeleteAsync("items/1");

            _mockHandler.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri!.ToString().Contains("users/user-4/items/1.json")), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_WhenUnauthenticated_ThrowsInvalidOperationException()
        {
            var authService = new Mock<IAuthService>();
            authService.Setup(a => a.GetCurrentUser()).Returns((AppUser?)null);

            var svc = new FirebaseRealtimeDatabaseService(_httpClient, authService.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.GetAsync<object>("should-fail"));
        }
    }
}
