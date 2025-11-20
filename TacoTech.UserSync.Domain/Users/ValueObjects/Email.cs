namespace TacoTech.UserSync.Domain.Users.ValueObjects
{
    /// <summary>
    /// Represents an email address value object with basic validation.
    /// </summary>
    public sealed record Email
    {
        /// <summary>
        /// Gets the underlying email string value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Email"/> record.
        /// </summary>
        /// <param name="value">The email string.</param>
        /// <exception cref="ArgumentException">Thrown when value is null, empty, or not a basic email format.</exception>
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new ArgumentException("Invalid email.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Returns the email string.
        /// </summary>
        /// <returns>The email string.</returns>
        public override string ToString() => Value;
    }
}
