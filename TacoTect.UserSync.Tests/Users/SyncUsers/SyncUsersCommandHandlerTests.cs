namespace TacoTech.UserSync.Application.Tests.Users.SyncUsers
{
    /// <summary>
    /// High-level tests for SyncUsersCommandHandler using package/data pattern.
    /// </summary>
    public class SyncUsersCommandHandlerTests
    {
        [Theory]
        [ClassData(typeof(SyncUsersCommandHandlerTestData))]
        public async Task SyncUsersCommandHandler_Scenarios(SyncUsersCommandHandlerTestPackage package)
        {
            // Arrange
            var setup = new SyncUsersCommandHandlerTestSetup();
            package.Arrange(setup);

            // Act
            var summary = await setup.ExecuteAsync(CancellationToken.None);

            // Assert
            package.AssertSummary(summary);
            package.Verify(setup);
        }
    }
}
