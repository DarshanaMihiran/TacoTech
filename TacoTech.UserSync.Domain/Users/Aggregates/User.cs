using TacoTech.UserSync.Domain.Users.UniqueIdentifiers;
using TacoTech.UserSync.Domain.Users.ValueObjects;

namespace TacoTech.UserSync.Domain.Users.Aggregates
{
    /// <summary>
    /// Represents the User aggregate root stored in the local database.
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public UserId Id { get; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the full name of the user.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Gets the city of the user.
        /// </summary>
        public City City { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="username">The username.</param>
        /// <param name="fullName">The full name.</param>
        /// <param name="email">The email.</param>
        /// <param name="city">The city.</param>
        public User(UserId id, string username, string fullName, Email email, City city)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Change(username, fullName, email, city);
        }

        /// <summary>
        /// Updates the mutable properties of the user.
        /// </summary>
        /// <param name="username">The new username.</param>
        /// <param name="fullName">The new full name.</param>
        /// <param name="email">The new email.</param>
        /// <param name="city">The new city.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when any parameter is null.
        /// </exception>
        public void Change(string username, string fullName, Email email, City city)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            City = city ?? throw new ArgumentNullException(nameof(city));
        }

        /// <summary>
        /// Determines whether this user has the same data as another user
        /// (used for idempotency checks: update vs skip).
        /// </summary>
        /// <param name="other">The other user to compare.</param>
        /// <returns>True if all relevant fields are equal; otherwise false.</returns>
        public bool HasSameDataAs(User other)
        {
            if (other is null) return false;

            return Username == other.Username &&
                   FullName == other.FullName &&
                   Email.Value == other.Email.Value &&
                   City.Value == other.City.Value;
        }
    }
}
