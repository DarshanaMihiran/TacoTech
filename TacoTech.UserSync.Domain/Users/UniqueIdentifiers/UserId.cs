namespace TacoTech.UserSync.Domain.Users.UniqueIdentifiers
{
    /// <summary>
    /// Represents the unique identifier of a user as provided by the external system.
    /// </summary>
    /// <param name="Value">The integer value of the user id.</param>
    public sealed record UserId(int Value);
}
