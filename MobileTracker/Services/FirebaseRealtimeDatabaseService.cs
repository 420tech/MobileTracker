using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MobileTracker.Services
{
    public class FirebaseRealtimeDatabaseService : IFirebaseDatabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly string _baseUrl;
        private readonly ILogger<FirebaseRealtimeDatabaseService> _logger;

        public FirebaseRealtimeDatabaseService(HttpClient httpClient, IAuthService authService, ILogger<FirebaseRealtimeDatabaseService>? logger = null)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<FirebaseRealtimeDatabaseService>.Instance;

            // Read database url from environment, e.g. https://<project>.firebaseio.com or https://<project>.firebaseio.com/
            var url = Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                // not fatal here, but calls will fail — log for debugging
                _logger.LogWarning("FIREBASE_DATABASE_URL is not set; database calls will fail until configured.");
            }

            _baseUrl = url.TrimEnd('/');
        }

        private async Task<string> BuildUrlForUserAsync(string relativePath)
        {
            var user = _authService.GetCurrentUser();
            if (user == null)
                throw new InvalidOperationException("User not authenticated");

            var token = await _authService.GetIdTokenAsync();

            // Ensure path doesn't start or end with '/'
            var path = relativePath?.Trim('/') ?? string.Empty;

            // Force all data under users/{uid}
            var fullPath = string.IsNullOrEmpty(path) ? $"users/{user.Uid}" : $"users/{user.Uid}/{path}";

            var authQuery = string.IsNullOrEmpty(token) ? string.Empty : $"?auth={Uri.EscapeDataString(token)}";

            return $"{_baseUrl}/{fullPath}.json{authQuery}";
        }

        public async Task<T?> GetAsync<T>(string relativePath)
        {
            var url = await BuildUrlForUserAsync(relativePath);
            _logger.LogDebug("GET {Url}", url);
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET failed {Url} => {Status}", url, response.StatusCode);
                return default;
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return default;

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task SetAsync<T>(string relativePath, T value)
        {
            var url = await BuildUrlForUserAsync(relativePath);
            _logger.LogDebug("PUT {Url}", url);
            var response = await _httpClient.PutAsJsonAsync(url, value);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> PushAsync<T>(string relativePath, T value)
        {
            // Push means POST to collection path
            var url = await BuildUrlForUserAsync(relativePath);
            _logger.LogDebug("POST {Url}", url);
            var response = await _httpClient.PostAsJsonAsync(url, value);
            response.EnsureSuccessStatusCode();

            var doc = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (doc.TryGetProperty("name", out var name) && name.ValueKind == JsonValueKind.String)
                return name.GetString()!;

            throw new Exception("Failed to read push key from Firebase response");
        }

        public async Task DeleteAsync(string relativePath)
        {
            var url = await BuildUrlForUserAsync(relativePath);
            _logger.LogDebug("DELETE {Url}", url);
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
