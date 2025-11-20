using Moq;
using TacoTech.UserSync.Application.Users.DTOs;
using TacoTech.UserSync.Application.Users.Summaries;
using TacoTech.UserSync.Domain.Users.Aggregates;
using TacoTech.UserSync.Domain.Users.UniqueIdentifiers;
using TacoTech.UserSync.Domain.Users.ValueObjects;

namespace TacoTech.UserSync.Application.Tests.Users.SyncUsers
{
    /// <summary>
    /// Base test package describing one SyncUsers scenario:
    /// - how to arrange mocks
    /// - what summary we expect
    /// - what extra verifications should be done
    /// </summary>
    public abstract class SyncUsersCommandHandlerTestPackage
    {
        public string Name { get; }

        protected SyncUsersCommandHandlerTestPackage(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Configure mocks for this scenario.
        /// </summary>
        public abstract void Arrange(SyncUsersCommandHandlerTestSetup setup);

        /// <summary>
        /// Assert the summary returned by the handler matches expectations.
        /// </summary>
        public abstract void AssertSummary(SyncSummary summary);

        /// <summary>
        /// Optional: verify mock interactions (Add/Update/Email, etc.).
        /// </summary>
        public virtual void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
        }
    }

    /// <summary>
    /// Scenario: remote user does not exist locally -> created.
    /// </summary>
    public sealed class CreateNewUserPackage : SyncUsersCommandHandlerTestPackage
    {
        public CreateNewUserPackage()
            : base("Create new user when not exists locally")
        {
        }

        public override void Arrange(SyncUsersCommandHandlerTestSetup setup)
        {
            var remoteUsers = new[]
            {
            new RemoteUserDto(
                Id: 1,
                Username: "john",
                Name: "John Doe",
                Email: "john@example.com",
                City: "Colombo")
        };

            setup.RemoteUserClientMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(remoteUsers);

            setup.UserRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            setup.UserRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            setup.EmailNotifierMock
                .Setup(n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public override void AssertSummary(SyncSummary summary)
        {
            Xunit.Assert.Equal(1, summary.UsersCreated);
            Xunit.Assert.Equal(0, summary.UsersUpdated);
            Xunit.Assert.Equal(0, summary.UsersSkipped);
            Xunit.Assert.Equal(0, summary.Errors);
        }

        public override void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
            setup.UserRepositoryMock.Verify(
                r => r.AddAsync(
                    It.Is<User>(u => u.Id.Value == 1),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            setup.UserRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never);

            setup.EmailNotifierMock.Verify(
                n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Scenario: local user exists but data is different -> updated.
    /// </summary>
    public sealed class UpdateExistingUserPackage : SyncUsersCommandHandlerTestPackage
    {
        public UpdateExistingUserPackage()
            : base("Update existing user when data changed")
        {
        }

        public override void Arrange(SyncUsersCommandHandlerTestSetup setup)
        {
            var remoteUsers = new[]
            {
            new RemoteUserDto(
                Id: 1,
                Username: "john",
                Name: "John Doe",
                Email: "new-email@example.com",
                City: "Colombo")
        };

            setup.RemoteUserClientMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(remoteUsers);

            var localUser = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("old-email@example.com"),
                new City("Colombo"));

            setup.UserRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { localUser });

            setup.UserRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            setup.EmailNotifierMock
                .Setup(n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public override void AssertSummary(SyncSummary summary)
        {
            Xunit.Assert.Equal(0, summary.UsersCreated);
            Xunit.Assert.Equal(1, summary.UsersUpdated);
            Xunit.Assert.Equal(0, summary.UsersSkipped);
            Xunit.Assert.Equal(0, summary.Errors);
        }

        public override void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
            setup.UserRepositoryMock.Verify(
                r => r.UpdateAsync(
                    It.Is<User>(u =>
                        u.Id.Value == 1 &&
                        u.Email.Value == "new-email@example.com"),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            setup.UserRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never);

            setup.EmailNotifierMock.Verify(
                n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Scenario: remote and local data identical -> skipped.
    /// </summary>
    public sealed class SkipIdenticalUserPackage : SyncUsersCommandHandlerTestPackage
    {
        public SkipIdenticalUserPackage()
            : base("Skip user when data is identical")
        {
        }

        public override void Arrange(SyncUsersCommandHandlerTestSetup setup)
        {
            var remoteUser = new RemoteUserDto(
                Id: 1,
                Username: "john",
                Name: "John Doe",
                Email: "john@example.com",
                City: "Colombo");

            setup.RemoteUserClientMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { remoteUser });

            var localUser = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("john@example.com"),
                new City("Colombo"));

            setup.UserRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { localUser });

            setup.EmailNotifierMock
                .Setup(n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public override void AssertSummary(SyncSummary summary)
        {
            Xunit.Assert.Equal(0, summary.UsersCreated);
            Xunit.Assert.Equal(0, summary.UsersUpdated);
            Xunit.Assert.Equal(1, summary.UsersSkipped);
            Xunit.Assert.Equal(0, summary.Errors);
        }

        public override void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
            setup.UserRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never);

            setup.UserRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never);

            setup.EmailNotifierMock.Verify(
                n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Scenario: one user fails (e.g. repository throws) -> Errors incremented, others continue.
    /// </summary>
    public sealed class PerUserErrorPackage : SyncUsersCommandHandlerTestPackage
    {
        public PerUserErrorPackage()
            : base("Increment Errors when per-user exception occurs and continue processing")
        {
        }

        public override void Arrange(SyncUsersCommandHandlerTestSetup setup)
        {
            var remoteUsers = new[]
            {
            new RemoteUserDto(
                Id: 1,
                Username: "bad-user",
                Name: "Bad User",
                Email: "bad@example.com",
                City: "Colombo"),
            new RemoteUserDto(
                Id: 2,
                Username: "good-user",
                Name: "Good User",
                Email: "good@example.com",
                City: "Colombo")
        };

            setup.RemoteUserClientMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(remoteUsers);

            setup.UserRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            var callCount = 0;
            setup.UserRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns<User, CancellationToken>((u, _) =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        throw new System.Exception("Simulated per-user failure");
                    }

                    return Task.CompletedTask;
                });

            setup.EmailNotifierMock
                .Setup(n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public override void AssertSummary(SyncSummary summary)
        {
            Xunit.Assert.Equal(1, summary.UsersCreated); // second user
            Xunit.Assert.Equal(0, summary.UsersUpdated);
            Xunit.Assert.Equal(0, summary.UsersSkipped);
            Xunit.Assert.Equal(1, summary.Errors);        // first user failed
        }

        public override void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
            setup.UserRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            setup.EmailNotifierMock.Verify(
                n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Scenario: email notifier throws -> sync still returns summary and does not increase Errors.
    /// </summary>
    public sealed class EmailFailurePackage : SyncUsersCommandHandlerTestPackage
    {
        public EmailFailurePackage()
            : base("Log email failure but do not fail sync")
        {
        }

        public override void Arrange(SyncUsersCommandHandlerTestSetup setup)
        {
            var remoteUsers = new[]
            {
            new RemoteUserDto(
                Id: 1,
                Username: "john",
                Name: "John Doe",
                Email: "john@example.com",
                City: "Colombo")
        };

            setup.RemoteUserClientMock
                .Setup(c => c.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(remoteUsers);

            setup.UserRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            setup.UserRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Email throws
            setup.EmailNotifierMock
                .Setup(n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new System.Exception("SMTP auth failed"));
        }

        public override void AssertSummary(SyncSummary summary)
        {
            Xunit.Assert.Equal(1, summary.UsersCreated);
            Xunit.Assert.Equal(0, summary.UsersUpdated);
            Xunit.Assert.Equal(0, summary.UsersSkipped);
            Xunit.Assert.Equal(0, summary.Errors); // email failure isn't counted
        }

        public override void Verify(SyncUsersCommandHandlerTestSetup setup)
        {
            setup.EmailNotifierMock.Verify(
                n => n.SendUserSyncCompletedAsync(
                    It.IsAny<SyncSummary>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
