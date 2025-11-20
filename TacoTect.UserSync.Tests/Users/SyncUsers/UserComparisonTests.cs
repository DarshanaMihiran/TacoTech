using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TacoTech.UserSync.Domain.Users.Aggregates;
using TacoTech.UserSync.Domain.Users.UniqueIdentifiers;
using TacoTech.UserSync.Domain.Users.ValueObjects;

namespace TacoTech.UserSync.Application.Tests.Users.SyncUsers
{
    /// <summary>
    /// Tests for the User.HasSameDataAs comparison logic.
    /// </summary>
    public class UserComparisonTests
    {
        [Fact]
        public void HasSameDataAs_Returns_True_For_Identical_Values()
        {
            var user1 = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("john@example.com"),
                new City("Colombo"));

            var user2 = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("john@example.com"),
                new City("Colombo"));

            Assert.True(user1.HasSameDataAs(user2));
        }

        [Fact]
        public void HasSameDataAs_Returns_False_When_Email_Differs()
        {
            var user1 = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("john@example.com"),
                new City("Colombo"));

            var user2 = new User(
                new UserId(1),
                "john",
                "John Doe",
                new Email("john+new@example.com"),
                new City("Colombo"));

            Assert.False(user1.HasSameDataAs(user2));
        }
    }
}
