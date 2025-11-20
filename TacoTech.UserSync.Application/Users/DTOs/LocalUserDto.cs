namespace TacoTech.UserSync.Application.Users.DTOs
{
    /// <summary>
    /// DTO representing a user stored in the local SQLite database.
    /// </summary>
    /// <param name="Id">The user id (from the remote system).</param>
    /// <param name="Username">The username.</param>
    /// <param name="FullName">The full name.</param>
    /// <param name="Email">The email address.</param>
    /// <param name="City">The city.</param>
    public sealed record LocalUserDto(
        int Id,
        string Username,
        string FullName,
        string Email,
        string City);
}
