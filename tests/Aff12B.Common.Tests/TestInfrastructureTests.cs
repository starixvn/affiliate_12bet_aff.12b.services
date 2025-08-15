using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.Tests
{
    /// <summary>
    /// Tests to verify the test infrastructure is working correctly
    /// </summary>
    public class TestInfrastructureTests
    {
        [Fact]
        public void TestUtilities_CreateFixture_ShouldReturnConfiguredFixture()
        {
            // Act
            var fixture = TestUtilities.CreateFixture();

            // Assert
            fixture.Should().NotBeNull();
            fixture.Should().BeOfType<Fixture>();
        }

        [Fact]
        public void TestUtilities_CreateTestTokenManagement_ShouldReturnValidConfiguration()
        {
            // Act
            var tokenManagement = TestUtilities.CreateTestTokenManagement();

            // Assert
            tokenManagement.Should().NotBeNull();
            tokenManagement.Secret.Should().NotBeNullOrEmpty();
            tokenManagement.Issuer.Should().Be("TestIssuer");
            tokenManagement.Audience.Should().Be("TestAudience");
            tokenManagement.AccessExpiration.Should().Be(60);
        }

        [Fact]
        public void TestUtilities_CreateTestUsers_ShouldReturnValidUsers()
        {
            // Act
            var users = TestUtilities.CreateTestUsers();

            // Assert
            users.Should().NotBeNull();
            users.Should().HaveCount(2);
            users.Should().Contain(u => u.UserName == "testuser");
            users.Should().Contain(u => u.UserName == "admin");
        }

        [Fact]
        public void TestUtilities_CreateDateRange_ShouldReturnValidDateRange()
        {
            // Act
            var (from, to) = TestUtilities.CreateDateRange(7, 1);

            // Assert
            from.Should().BeBefore(to);
            to.Should().BeAfter(from);
            (to - from).TotalDays.Should().Be(1);
        }
    }
}
