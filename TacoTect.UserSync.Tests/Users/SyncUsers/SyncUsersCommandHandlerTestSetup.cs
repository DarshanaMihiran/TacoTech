using Microsoft.Extensions.Logging;
using Moq;
using TacoTech.UserSync.Application.Users.Commands;
using TacoTech.UserSync.Application.Users.Handlers;
using TacoTech.UserSync.Application.Users.Interfaces;
using TacoTech.UserSync.Application.Users.Summaries;
using TacoTech.UserSync.Domain.Users.Interfaces.Repositories;

namespace TacoTech.UserSync.Application.Tests.Users.SyncUsers
{
    /// <summary>
    /// Common test setup for SyncUsersCommandHandler tests.
    /// Holds mocks and provides helper methods to create the handler
    /// and send the command.
    /// </summary>
    public class SyncUsersCommandHandlerTestSetup
    {
        public Mock<IUserRepository> UserRepositoryMock { get; } = new();
        public Mock<IRemoteUserClient> RemoteUserClientMock { get; } = new();
        public Mock<IEmailNotifier> EmailNotifierMock { get; } = new();
        public Mock<ILogger<SyncUsersCommandHandler>> LoggerMock { get; } = new();

        /// <summary>
        /// Creates a new instance of the handler under test.
        /// </summary>
        public SyncUsersCommandHandler CreateHandler()
            => new(
                UserRepositoryMock.Object,
                RemoteUserClientMock.Object,
                EmailNotifierMock.Object,
                LoggerMock.Object);

        /// <summary>
        /// Helper to execute the SyncUsersCommand using the current handler.
        /// </summary>
        public Task<SyncSummary> ExecuteAsync(CancellationToken ct = default)
        {
            var handler = CreateHandler();
            return handler.Handle(new SyncUsersCommand(), ct);
        }
    }
}
