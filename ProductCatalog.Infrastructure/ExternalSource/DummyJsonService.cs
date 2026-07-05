using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Models;
using ProductCatalog.Domain.Exceptions;
using ProductCatalog.Infrastructure.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProductCatalog.Infrastructure.ExternalSource
{
    public class DummyJsonService : IAuthService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public DummyJsonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<DummyJsonProductDto>> GetAllProductsAsync()
        {
            var wrapper = await GetAsync<DummyJsonProductListResponse>("products?limit=0");
            return wrapper.Products;
        }

        public async Task<IReadOnlyList<DummyJsonCategoryDto>> GetCategoriesAsync()
        {
            return await GetAsync<List<DummyJsonCategoryDto>>("products/categories");
        }

        public async Task<string?> GetTokenAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new
            {
                username,
                password,
                expiresInMins = 30
            });

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DummyJsonLoginResponse>(JsonOptions);
            return result?.AccessToken;
        }

        public async Task<User?> GetCurrentUserAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "auth/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

      
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<DummyJsonUserDto>(JsonOptions);

            return new User
            {
                Id = dto!.Id,
                Username = dto.Username,
                Role = dto.Role
            };
        }

        private async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);

            if (result is null)
                throw new ExternalServiceUnavailableException(
                    "External service returned empty response.",
                    new Exception($"Response body is null for '{url}'."));

            return result;
        }
    }
}
