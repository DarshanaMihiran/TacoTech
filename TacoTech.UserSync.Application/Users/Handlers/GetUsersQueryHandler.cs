using MediatR;
using TacoTech.UserSync.Application.Users.DTOs;
using TacoTech.UserSync.Application.Users.Queries;
using TacoTech.UserSync.Domain.Users.Interfaces.Repositories;

namespace TacoTech.UserSync.Application.Users.Handlers
{
    /// <summary>
    /// Handles the <see cref="GetUsersQuery"/> by reading all users
    /// from the local repository and mapping them to DTOs.
    /// </summary>
    public sealed class GetUsersQueryHandler
        : IRequestHandler<GetUsersQuery, IReadOnlyCollection<LocalUserDto>>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUsersQueryHandler"/> class.
        /// </summary>
        /// <param name="userRepository">Repository used to access local users.</param>
        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<LocalUserDto>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);

            var dtos = users
                .Select(u => new LocalUserDto(
                    Id: u.Id.Value,
                    Username: u.Username,
                    FullName: u.FullName,
                    Email: u.Email.Value,
                    City: u.City.Value))
                .ToList();

            return dtos;
        }
    }
}
