using TacoTech.UserSync.Application.Users.Summaries;

namespace TacoTech.UserSync.Application.Users.Interfaces
{
    /// <summary>
    /// Abstraction for sending email notifications about user synchronization results.
    /// </summary>
    public interface IEmailNotifier
    {
        /// <summary>
        /// Sends an email indicating that user synchronization has completed.
        /// </summary>
        /// <param name="summary">The sync summary.</param>
        /// <param name="ct">Cancellation token.</param>
        Task SendUserSyncCompletedAsync(SyncSummary summary, CancellationToken ct = default);
    }
}
