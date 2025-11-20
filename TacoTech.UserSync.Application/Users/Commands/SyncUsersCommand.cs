using MediatR;
using TacoTech.UserSync.Application.Users.Summaries;

namespace TacoTech.UserSync.Application.Users.Commands
{
    /// <summary>
    /// Represents a request to run the user synchronization process.
    /// </summary>
    public sealed record SyncUsersCommand : IRequest<SyncSummary>;
}
