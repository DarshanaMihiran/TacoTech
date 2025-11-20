using MediatR;
using TacoTech.UserSync.Application.Users.DTOs;

namespace TacoTech.UserSync.Application.Users.Queries
{
    /// <summary>
    /// Query used to retrieve all users from the local database.
    /// </summary>
    public sealed record GetUsersQuery : IRequest<IReadOnlyCollection<LocalUserDto>>;
}
