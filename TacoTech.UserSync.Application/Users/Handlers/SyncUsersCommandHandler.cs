using MediatR;
using Microsoft.Extensions.Logging;
using TacoTech.UserSync.Application.Users.Commands;
using TacoTech.UserSync.Application.Users.DTOs;
using TacoTech.UserSync.Application.Users.Interfaces;
using TacoTech.UserSync.Application.Users.Summaries;
using TacoTech.UserSync.Domain.Users.Aggregates;
using TacoTech.UserSync.Domain.Users.Interfaces.Repositories;
using TacoTech.UserSync.Domain.Users.UniqueIdentifiers;
using TacoTech.UserSync.Domain.Users.ValueObjects;

namespace TacoTech.UserSync.Application.Users.Handlers
{
    /// <summary>
    /// Handles execution of the user synchronization process:
    /// - fetch remote users
    /// - compare with local users
    /// - create/update/skip accordingly
    /// - send email notification
    /// </summary>
    public sealed class SyncUsersCommandHandler
        : IRequestHandler<SyncUsersCommand, SyncSummary>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRemoteUserClient _remoteUserClient;
        private readonly IEmailNotifier _emailNotifier;
        private readonly ILogger<SyncUsersCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncUsersCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">Repository used for local user persistence.</param>
        /// <param name="remoteUserClient">Client used to fetch remote users.</param>
        /// <param name="emailNotifier">Notifier used to send completion emails.</param>
        /// <param name="logger">Logger instance.</param>
        public SyncUsersCommandHandler(
            IUserRepository userRepository,
            IRemoteUserClient remoteUserClient,
            IEmailNotifier emailNotifier,
            ILogger<SyncUsersCommandHandler> logger)
        {
            _userRepository = userRepository;
            _remoteUserClient = remoteUserClient;
            _emailNotifier = emailNotifier;
            _logger = logger;
        }

        /// <summary>
        /// Runs the user synchronization process and returns a summary.
        /// </summary>
        /// <param name="request">The sync command (no payload).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The synchronization summary.</returns>
        public async Task<SyncSummary> Handle(SyncUsersCommand request, CancellationToken ct)
        {
            var usersCreated = 0;
            var usersUpdated = 0;
            var usersSkipped = 0;
            var errors = 0;

            var remoteUsers = await _remoteUserClient.GetAllAsync(ct);
            var localUsers = (await _userRepository.GetAllAsync(ct))
                .ToDictionary(u => u.Id.Value);

            foreach (var remote in remoteUsers)
            {
                try
                {
                    var remoteDomainUser = MapToDomain(remote);

                    if (!localUsers.TryGetValue(remote.Id, out var localUser))
                    {
                        await _userRepository.AddAsync(remoteDomainUser, ct);
                        usersCreated++;
                    }
                    else
                    {
                        if (!localUser.HasSameDataAs(remoteDomainUser))
                        {
                            localUser.Change(
                                remoteDomainUser.Username,
                                remoteDomainUser.FullName,
                                remoteDomainUser.Email,
                                remoteDomainUser.City);

                            await _userRepository.UpdateAsync(localUser, ct);
                            usersUpdated++;
                        }
                        else
                        {
                            usersSkipped++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error synchronizing user {RemoteUserId}", remote.Id);
                    errors++;
                }
            }

            var summary = new SyncSummary(
                UsersCreated: usersCreated,
                UsersUpdated: usersUpdated,
                UsersSkipped: usersSkipped,
                Errors: errors);

            try
            {
                await _emailNotifier.SendUserSyncCompletedAsync(summary, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send synchronization email notification");
                // Requirement: email failure should not erase DB changes.
            }

            return summary;
        }

        /// <summary>
        /// Maps a remote user DTO to a domain <see cref="User"/> instance.
        /// </summary>
        /// <param name="dto">The remote user DTO.</param>
        /// <returns>A domain user object.</returns>
        private static User MapToDomain(RemoteUserDto dto) =>
            new(
                new UserId(dto.Id),
                dto.Username,
                dto.Name,
                new Email(dto.Email),
                new City(dto.City));
    }
}
