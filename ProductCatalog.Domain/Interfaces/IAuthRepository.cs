using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<string?> GetTokenAsync(string username, string password);
        Task<User?> GetCurrentUserAsync(string token);
    }
}
