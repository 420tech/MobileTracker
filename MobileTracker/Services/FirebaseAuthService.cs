using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MobileTracker.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MobileTracker.Services
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<FirebaseAuthService> _logger;
        private AppUser _currentUser;
        private string? _idToken;
        private readonly Dictionary<string, int> _failedLoginAttempts = new();
        private readonly Dictionary<string, DateTime> _lockoutUntil = new();
        private const int MaxFailedAttempts = 5;
        private readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        public FirebaseAuthService()
            : this(new HttpClient(), NullLogger<FirebaseAuthService>.Instance)
        {
        }

        public FirebaseAuthService(HttpClient httpClient, ILogger<FirebaseAuthService>? logger = null)
        {
            _httpClient = httpClient;
            _logger = logger ?? NullLogger<FirebaseAuthService>.Instance;
            _apiKey = GetApiKeyFromEnvironment();

            try
            {
                _logger.LogInformation("FirebaseAuthService initialized. ApiKey present: {HasKey}", !string.IsNullOrWhiteSpace(_apiKey) && _apiKey != "YOUR_FIREBASE_API_KEY");
            }
            catch
            {
                // best-effort logging
            }
        }

        private static string GetApiKeyFromEnvironment()
        {
            // Read from environment first. We intentionally keep a fallback so tests
            // and CI without a .env don't immediately break.
            var key = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
            return string.IsNullOrWhiteSpace(key) ? "YOUR_FIREBASE_API_KEY" : key!;
        }

        public bool IsAuthenticated => _currentUser != null;

        public async Task<AppUser> RegisterAsync(string email, string password)
        {
            var requestBody = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var url = $"https://identitytool.googleapis.com/v1/accounts:signUp?key={_apiKey}";
            _logger.LogDebug("Register request: {Url} for {Email}", url, email);
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                _logger.LogInformation("Register succeeded for {Email}, uid={Uid}", email, result?.localId);
                _currentUser = new AppUser
                {
                    Uid = result.localId,
                    Email = result.email,
                    RegistrationDate = DateTime.UtcNow,
                    AccountStatus = AccountStatus.Active
                };
                // store id token for authenticated calls
                _idToken = result?.idToken;
                return _currentUser;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
                _logger.LogError("Register failed for {Email}: {Error}", email, error?.error?.message);
                throw new Exception(error?.error?.message ?? "Unknown error");
            }
        }

        public async Task<AppUser> LoginAsync(string email, string password)
        {
            // Check if account is locked
            if (_lockoutUntil.TryGetValue(email, out var lockoutTime) && DateTime.UtcNow < lockoutTime)
            {
                var remainingTime = lockoutTime - DateTime.UtcNow;
                throw new Exception($"Account is locked due to too many failed attempts. Try again in {remainingTime.TotalMinutes:F0} minutes.");
            }

            var requestBody = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
            _logger.LogDebug("Login request: {Url} for {Email}", url, email);
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
                _logger.LogInformation("Login succeeded for {Email}, uid={Uid}", email, result?.localId);
                _currentUser = new AppUser
                {
                    Uid = result.localId,
                    Email = result.email,
                    LastLoginDate = DateTime.UtcNow,
                    AccountStatus = AccountStatus.Active
                };

                // store id token for authenticated calls
                _idToken = result?.idToken;

                // Reset failed attempts on successful login
                _failedLoginAttempts.Remove(email);
                _lockoutUntil.Remove(email);

                return _currentUser;
            }
            else
            {
                // Increment failed attempts
                _failedLoginAttempts[email] = _failedLoginAttempts.GetValueOrDefault(email) + 1;

                // Lock account if max attempts reached
                if (_failedLoginAttempts[email] >= MaxFailedAttempts)
                {
                    _lockoutUntil[email] = DateTime.UtcNow.Add(LockoutDuration);
                    throw new Exception($"Account locked due to too many failed attempts. Try again in {LockoutDuration.TotalMinutes} minutes.");
                }

                var error = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
                _logger.LogWarning("Login failed for {Email}: {Error}", email, error?.error?.message);
                throw new Exception(error?.error?.message ?? "Unknown error");
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _idToken = null;
            return Task.CompletedTask;
        }

        public Task<string?> GetIdTokenAsync()
        {
            // In a real implementation we'd refresh tokens when expired; for now return what we have.
            return Task.FromResult(_idToken);
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            var requestBody = new
            {
                requestType = "PASSWORD_RESET",
                email
            };

            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_apiKey}";
            _logger.LogDebug("SendPasswordResetEmail request: {Url} for {Email}", url, email);
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<FirebaseErrorResponse>();
                _logger.LogError("SendPasswordResetEmail failed for {Email}: {Error}", email, error?.error?.message);
                throw new Exception(error?.error?.message ?? "Unknown error");
            }
        }

        public AppUser GetCurrentUser() => _currentUser;

        private class FirebaseAuthResponse
        {
            public string kind { get; set; }
            public string localId { get; set; }
            public string email { get; set; }
            public string displayName { get; set; }
            public string idToken { get; set; }
            public bool registered { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
        }

        private class FirebaseErrorResponse
        {
            public FirebaseError error { get; set; }
        }

        private class FirebaseError
        {
            public int code { get; set; }
            public string message { get; set; }
        }
    }
}