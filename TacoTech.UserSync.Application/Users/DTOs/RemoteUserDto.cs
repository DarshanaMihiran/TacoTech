namespace TacoTech.UserSync.Application.Users.DTOs
{
    /// <summary>
    /// DTO representing a user as received from the remote JSONPlaceholder API.
    /// </summary>
    /// <param name="Id">User id from the remote API.</param>
    /// <param name="Username">Username.</param>
    /// <param name="Name">Full name.</param>
    /// <param name="Email">Email address.</param>
    /// <param name="City">City.</param>
    public sealed record RemoteUserDto(
        int Id,
        string Username,
        string Name,
        string Email,
        string City);
}
