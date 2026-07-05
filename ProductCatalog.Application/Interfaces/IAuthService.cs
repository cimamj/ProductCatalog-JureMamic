using ProductCatalog.Application.Models;

namespace ProductCatalog.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> GetTokenAsync(string username, string password);
        Task<User?> GetCurrentUserAsync(string token);
    }
}
