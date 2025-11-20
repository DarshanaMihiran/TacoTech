namespace TacoTech.UserSync.Infrastructure.Users.Emails
{
    /// <summary>
    /// Configuration settings for SMTP email sending.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>
        /// Gets or sets the SMTP host.
        /// </summary>
        public string Host { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        public int Port { get; init; }

        /// <summary>
        /// Gets or sets the username used for SMTP authentication.
        /// </summary>
        public string Username { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the password used for SMTP authentication.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the "from" email address.
        /// </summary>
        public string From { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the "to" email address.
        /// </summary>
        public string To { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled.
        /// </summary>
        public bool EnableSsl { get; init; } = true;
    }
}
