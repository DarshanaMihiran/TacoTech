using TacoTech.UserSync.Application.Users.DTOs;

namespace TacoTech.UserSync.Application.Users.Interfaces
{
    /// <summary>
    /// Abstraction for a client that retrieves users from the remote system.
    /// </summary>
    public interface IRemoteUserClient
    {
        /// <summary>
        /// Retrieves all users from the remote system.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of remote user DTOs.</returns>
        Task<IReadOnlyCollection<RemoteUserDto>> GetAllAsync(CancellationToken ct = default);
    }
}
