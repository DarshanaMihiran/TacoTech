namespace TacoTech.UserSync.Infrastructure.Users.Persistence.Entities
{
    /// <summary>
    /// EF Core entity for persisting users in SQLite.
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// Gets or sets the user id (from remote system).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; } = default!;

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string FullName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; } = default!;
    }
}
