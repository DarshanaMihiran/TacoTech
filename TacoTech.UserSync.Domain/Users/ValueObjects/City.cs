namespace TacoTech.UserSync.Domain.Users.ValueObjects
{
    /// <summary>
    /// Represents a city value object.
    /// </summary>
    /// <param name="Value">The city name.</param>
    public sealed record City(string Value)
    {
        /// <summary>
        /// Returns the city name.
        /// </summary>
        /// <returns>The city string.</returns>
        public override string ToString() => Value;
    }
}
