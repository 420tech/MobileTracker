using System;
using System.Net.Http;
using System.Threading.Tasks;
using MobileTracker.Models;
using MobileTracker.Services;
using Xunit;

namespace MobileTracker.UnitTests.Integration
{
    // These are integration tests that REQUIRE a real Firebase database and a valid ID token.
    // They are skipped unless the environment variables below are set.
    public class FirebaseRealtimeDatabaseIntegrationTests
    {
        private static bool IsIntegrationEnabled() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FIREBASE_INTEGRATION_TEST"));

        private static IAuthService? BuildAuthServiceFromEnv()
        {
            var token = Environment.GetEnvironmentVariable("FIREBASE_TEST_ID_TOKEN");
            var uid = Environment.GetEnvironmentVariable("FIREBASE_TEST_UID");
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(uid))
                return null;

            return new TestAuthService(uid!, token!);
        }

        [Fact]
        public async Task Integration_SetGetDelete_Works()
        {
            if (!IsIntegrationEnabled() || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")))
            {
                Console.WriteLine("Integration test skipped: required env vars FIREBASE_INTEGRATION_TEST and FIREBASE_DATABASE_URL not set");
                return;
            }

            var auth = BuildAuthServiceFromEnv();
            var baseUrl = Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")!;

            if (auth is null)
            {
                Console.WriteLine("Integration test skipped: FIREBASE_TEST_ID_TOKEN or FIREBASE_TEST_UID not set");
                return;
            }

            var http = new HttpClient();
            Environment.SetEnvironmentVariable("FIREBASE_DATABASE_URL", baseUrl);
            var svc = new FirebaseRealtimeDatabaseService(http, auth);

            var path = "integration/tests/simple";
            await svc.SetAsync(path, new { message = "hello" });

            var got = await svc.GetAsync<System.Collections.Generic.Dictionary<string, string>>(path);
            Assert.NotNull(got);
            Assert.True(got.ContainsKey("message") || got.Count > 0);

            await svc.DeleteAsync(path);
            var after = await svc.GetAsync<object>(path);
            Assert.Null(after);
        }

        [Fact]
        public async Task Integration_Push_Get_DeleteCollection_Works()
        {
            if (!IsIntegrationEnabled() || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")))
            {
                Console.WriteLine("Integration test skipped: required env vars FIREBASE_INTEGRATION_TEST and FIREBASE_DATABASE_URL not set");
                return;
            }

            var auth = BuildAuthServiceFromEnv();
            var baseUrl = Environment.GetEnvironmentVariable("FIREBASE_DATABASE_URL")!;

            if (auth is null)
            {
                Console.WriteLine("Integration test skipped: FIREBASE_TEST_ID_TOKEN or FIREBASE_TEST_UID not set");
                return;
            }

            var http = new HttpClient();
            Environment.SetEnvironmentVariable("FIREBASE_DATABASE_URL", baseUrl);
            var svc = new FirebaseRealtimeDatabaseService(http, auth);

            var model = new Client { Name = "IntegrationClient", ContactEmail = "int@example.com" };
            var key = await svc.PushAsync("clients", model);
            Assert.False(string.IsNullOrWhiteSpace(key));

            var all = await svc.GetAsync<System.Collections.Generic.Dictionary<string, Client>>("clients");
            Assert.NotNull(all);
            Assert.Contains(key, all.Keys);

            await svc.DeleteAsync($"clients/{key}");
            var after = await svc.GetAsync<System.Collections.Generic.Dictionary<string, Client>>("clients");
            if (after != null)
                Assert.False(after.ContainsKey(key));
        }

        // Minimal test auth implementation using env-provided token/uid
        private class TestAuthService : IAuthService
        {
            private readonly string _uid;
            private readonly string _token;

            public TestAuthService(string uid, string token)
            {
                _uid = uid;
                _token = token;
            }

            public bool IsAuthenticated => true;

            public System.Threading.Tasks.Task<AppUser> LoginAsync(string email, string password) => throw new NotImplementedException();
            public System.Threading.Tasks.Task SendPasswordResetEmailAsync(string email) => throw new NotImplementedException();
            public System.Threading.Tasks.Task<AppUser> RegisterAsync(string email, string password) => throw new NotImplementedException();
            public System.Threading.Tasks.Task LogoutAsync() => throw new NotImplementedException();

            public Task<string?> GetIdTokenAsync() => Task.FromResult<string?>(_token);

            public AppUser GetCurrentUser() => new AppUser { Uid = _uid, Email = "integration@test" };
        }
    }
}
