namespace TacoTech.UserSync.Application.Users.Summaries
{
    /// <summary>
    /// Represents the summary result of a user synchronization operation.
    /// </summary>
    /// <param name="UsersCreated">Number of users created.</param>
    /// <param name="UsersUpdated">Number of users updated.</param>
    /// <param name="UsersSkipped">Number of users skipped.</param>
    /// <param name="Errors">Number of errors encountered.</param>
    public sealed record SyncSummary(
        int UsersCreated,
        int UsersUpdated,
        int UsersSkipped,
        int Errors);
}
