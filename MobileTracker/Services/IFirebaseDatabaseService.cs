using System.Threading.Tasks;

namespace MobileTracker.Services
{
    public interface IFirebaseDatabaseService
    {
        // Read a value at the given relative path under the current user's node.
        Task<T?> GetAsync<T>(string relativePath);

        // Overwrite the value at the given relative path under the current user's node.
        Task SetAsync<T>(string relativePath, T value);

        // Push a new child under the given relative path and return the generated key.
        Task<string> PushAsync<T>(string relativePath, T value);

        // Delete the value at the given relative path under the current user's node.
        Task DeleteAsync(string relativePath);
    }
}
