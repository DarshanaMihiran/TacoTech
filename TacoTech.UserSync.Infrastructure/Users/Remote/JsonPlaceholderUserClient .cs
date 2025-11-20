using System.Net.Http.Json;
using TacoTech.UserSync.Application.Users.DTOs;
using TacoTech.UserSync.Application.Users.Interfaces;

namespace TacoTech.UserSync.Infrastructure.Users.Remote
{
    /// <summary>
    /// HTTP client for retrieving users from the JSONPlaceholder API.
    /// </summary>
    public class JsonPlaceholderUserClient : IRemoteUserClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPlaceholderUserClient"/> class.
        /// </summary>
        /// <param name="httpClient">The configured HTTP client.</param>
        public JsonPlaceholderUserClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<RemoteUserDto>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _httpClient
                .GetFromJsonAsync<List<JsonUserDto>>("users", cancellationToken: ct)
                ?? new List<JsonUserDto>();

            return users
                .Select(u => new RemoteUserDto(
                    u.Id,
                    u.Username,
                    u.Name,
                    u.Email,
                    u.Address.City))
                .ToList();
        }

        /// <summary>
        /// Internal DTO representing the JSONPlaceholder user structure.
        /// </summary>
        private sealed class JsonUserDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = default!;
            public string Name { get; set; } = default!;
            public string Email { get; set; } = default!;
            public AddressDto Address { get; set; } = default!;
        }

        /// <summary>
        /// Internal DTO representing the address part of the JSONPlaceholder user.
        /// </summary>
        private sealed class AddressDto
        {
            public string City { get; set; } = default!;
        }
    }
}
