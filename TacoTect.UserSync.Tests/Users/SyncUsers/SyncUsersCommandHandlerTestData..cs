using System.Collections;
    
namespace TacoTech.UserSync.Application.Tests.Users.SyncUsers
{
    /// <summary>
    /// xUnit ClassData provider for SyncUsersCommandHandler tests.
    /// Yields different packages for create/update/skip/error/email failure.
    /// </summary>
    public class SyncUsersCommandHandlerTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new CreateNewUserPackage() };
            yield return new object[] { new UpdateExistingUserPackage() };
            yield return new object[] { new SkipIdenticalUserPackage() };
            yield return new object[] { new PerUserErrorPackage() };
            yield return new object[] { new EmailFailurePackage() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
