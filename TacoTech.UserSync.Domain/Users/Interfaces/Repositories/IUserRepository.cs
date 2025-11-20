using TacoTech.UserSync.Domain.Users.Aggregates;

namespace TacoTech.UserSync.Domain.Users.Interfaces.Repositories
{
    /// <summary>
    /// Defines persistence operations for user aggregates.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the local data store.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A collection of users.</returns>
        Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Adds a new user to the data store.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="ct">Cancellation token.</param>
        Task AddAsync(User user, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="ct">Cancellation token.</param>
        Task UpdateAsync(User user, CancellationToken ct = default);
    }
}
