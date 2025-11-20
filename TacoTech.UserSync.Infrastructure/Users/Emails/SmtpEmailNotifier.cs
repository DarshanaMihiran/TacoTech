using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TacoTech.UserSync.Application.Users.Interfaces;
using TacoTech.UserSync.Application.Users.Summaries;
using TacoTech.UserSync.Infrastructure.Users.Emails;

namespace TacoTech.UserSync.Infrastructure.Users.Emails
{
    /// <summary>
    /// SMTP-based implementation of <see cref="IEmailNotifier"/>.
    /// </summary>
    public class SmtpEmailNotifier : IEmailNotifier
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailNotifier> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailNotifier"/> class.
        /// </summary>
        /// <param name="options">Email settings.</param>
        /// <param name="logger">Logger instance.</param>
        public SmtpEmailNotifier(
            IOptions<EmailSettings> options,
            ILogger<SmtpEmailNotifier> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendUserSyncCompletedAsync(SyncSummary summary, CancellationToken ct = default)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            var body =
                $"User sync completed.{Environment.NewLine}{Environment.NewLine}" +
                $"Users created: {summary.UsersCreated}{Environment.NewLine}" +
                $"Users updated: {summary.UsersUpdated}{Environment.NewLine}" +
                $"Users skipped: {summary.UsersSkipped}{Environment.NewLine}" +
                $"Errors:        {summary.Errors}{Environment.NewLine}";

            var message = new MailMessage(
                from: _settings.From,
                to: _settings.To,
                subject: "User Sync Completed",
                body: body);

            try
            {
                // CancellationToken is not directly supported in SendMailAsync,
                // but we can throw if cancellation requested before/after call.
                ct.ThrowIfCancellationRequested();
                await client.SendMailAsync(message);
                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Email sending was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification.");
                throw;
            }
        }
    }
}
