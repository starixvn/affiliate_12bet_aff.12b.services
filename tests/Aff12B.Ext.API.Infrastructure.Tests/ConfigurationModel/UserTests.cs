using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.ConfigurationModel
{
    public class UserTests
    {
        #region Property Tests

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            const string expectedUserName = "testuser";
            const string expectedPassword = "testpassword";

            // Act
            user.UserName = expectedUserName;
            user.Password = expectedPassword;

            // Assert
            user.UserName.Should().Be(expectedUserName);
            user.Password.Should().Be(expectedPassword);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12B_QC_Test")]
        [InlineData("12B_Aff_User")]
        [InlineData("12B_Aff_Admin")]
        [InlineData("12B_CronJobs")]
        [InlineData("12B_Api_Service")]
        [InlineData("12B_Ext_Api")]
        [InlineData("12B_Ext_Service")]
        [InlineData("user@domain.com")]
        [InlineData("user_with_underscore")]
        [InlineData("user-with-dash")]
        [InlineData("UserWithMixedCase")]
        [InlineData("123456789")]
        [InlineData("special!@#$%")]
        public void UserName_ShouldHandleVariousValues(string userNameValue)
        {
            // Arrange
            var user = new User();

            // Act
            user.UserName = userNameValue;

            // Assert
            user.UserName.Should().Be(userNameValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("plaintext")]
        [InlineData("tFg8A+gX8S4aQ0odNZ+csT4TMYYkNG0eEfAf+rE6FKc=")]
        [InlineData("Nuj0X0jBH95J/HYzXI7rxyIguUB4UAURiql14QkPRME=")]
        [InlineData("7gmM615HkNLQb4ZMomTJPG6TufCxWBvi7gkyGBNJ5xI=")]
        [InlineData("K6/4p65JTE+S1k4Tog0Z8Al2r+RdxiaQxm1wq2q7x/o=")]
        [InlineData("HDOFkNIIaID3fBK2IpaYRvp6vJfgza9mSg30qse5EaU=")]
        [InlineData("4IAi1CXY406EfUHmM1RlLgG2bno3Ns+p9Ph1DdayqQE=")]
        [InlineData("aHzYH/LZMl1//2Qk1PSFdnTf7asbPiKAJ21tTYUCzJY=")]
        [InlineData("very-long-password-that-might-be-used-in-some-systems")]
        [InlineData("password123!@#")]
        public void Password_ShouldHandleVariousValues(string passwordValue)
        {
            // Arrange
            var user = new User();

            // Act
            user.Password = passwordValue;

            // Assert
            user.Password.Should().Be(passwordValue);
        }

        #endregion

        #region Serialization Tests

        [Fact]
        public void User_ShouldBeSerializable()
        {
            // Arrange
            var user = new User
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };

            // Act & Assert
            // The [Serializable] attribute should make this serializable
            user.Should().BeAssignableTo<User>();
            typeof(User).Should().BeDecoratedWith<SerializableAttribute>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void User_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.UserName.Should().BeNull();
            user.Password.Should().BeNull();
        }

        [Fact]
        public void User_RealWorldConfiguration_ShouldWork()
        {
            // Arrange & Act
            var users = new List<User>
            {
                new User { UserName = "12B_QC_Test", Password = "tFg8A+gX8S4aQ0odNZ+csT4TMYYkNG0eEfAf+rE6FKc=" },
                new User { UserName = "12B_Aff_User", Password = "Nuj0X0jBH95J/HYzXI7rxyIguUB4UAURiql14QkPRME=" },
                new User { UserName = "12B_Aff_Admin", Password = "7gmM615HkNLQb4ZMomTJPG6TufCxWBvi7gkyGBNJ5xI=" }
            };

            // Assert
            users.Should().HaveCount(3);
            users.Should().AllSatisfy(user =>
            {
                user.UserName.Should().NotBeNullOrEmpty();
                user.Password.Should().NotBeNullOrEmpty();
            });
        }

        [Fact]
        public void User_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var user1 = new User { UserName = "User1", Password = "Password1" };
            var user2 = new User { UserName = "User2", Password = "Password2" };

            // Act
            user1.UserName = "ModifiedUser1";

            // Assert
            user1.UserName.Should().Be("ModifiedUser1");
            user1.Password.Should().Be("Password1");
            user2.UserName.Should().Be("User2");
            user2.Password.Should().Be("Password2");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void User_PropertyAssignment_ShouldOverwritePreviousValues()
        {
            // Arrange
            var user = new User
            {
                UserName = "InitialUser",
                Password = "InitialPassword"
            };

            // Act
            user.UserName = "UpdatedUser";
            user.Password = "UpdatedPassword";

            // Assert
            user.UserName.Should().Be("UpdatedUser");
            user.Password.Should().Be("UpdatedPassword");
        }

        [Fact]
        public void User_NullAssignments_ShouldAcceptNullValues()
        {
            // Arrange
            var user = new User
            {
                UserName = "TestUser",
                Password = "TestPassword"
            };

            // Act
            user.UserName = null;
            user.Password = null;

            // Assert
            user.UserName.Should().BeNull();
            user.Password.Should().BeNull();
        }

        #endregion
    }
}
