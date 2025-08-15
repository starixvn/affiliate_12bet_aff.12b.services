using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using Aff12B.Ext.API.Infastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for UserManagementService class
    /// Tests user validation and authentication functionality
    /// Target: 100% code coverage
    /// </summary>
    public class UserManagementServiceTests
    {
        private readonly Mock<IOptions<List<User>>> _mockUserListAccessor;
        private readonly List<User> _testUsers;

        public UserManagementServiceTests()
        {
            _mockUserListAccessor = new Mock<IOptions<List<User>>>();
            _testUsers = CreateTestUsers();
            _mockUserListAccessor.Setup(x => x.Value).Returns(_testUsers);
        }

        [Fact]
        public void Constructor_ValidUserList_ShouldCreateInstance()
        {
            // Act
            var service = new UserManagementService(_mockUserListAccessor.Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IUserManagementService>();
        }

        [Fact]
        public void Constructor_NullUserListAccessor_ShouldThrowNullReferenceException()
        {
            // Act & Assert
            var act = () => new UserManagementService(null);
            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Constructor_NullUserListValue_ShouldCreateInstanceWithEmptyUsers()
        {
            // Arrange
            var mockAccessor = new Mock<IOptions<List<User>>>();
            mockAccessor.Setup(x => x.Value).Returns((List<User>)null);

            // Act
            var service = new UserManagementService(mockAccessor.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void IsValidUser_ValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "testuser";
            var password = "password123"; // This will be hashed to match stored hash

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidUser_InvalidUsername_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "nonexistentuser";
            var password = "password123";

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_InvalidPassword_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "testuser";
            var password = "wrongpassword";

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_NullUsername_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            string username = null;
            var password = "password123";

            // Act
            var result = service.IsValidUser(username!, password);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_NullPassword_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "testuser";
            string password = null;

            // Act
            var result = service.IsValidUser(username, password!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_EmptyCredentials_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "";
            var password = "";

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("testuser", true)]
        [InlineData("TESTUSER", false)]
        [InlineData("TestUser", false)]
        [InlineData("adminuser", true)]
        [InlineData("ADMINUSER", false)]
        public void IsValidUser_CaseSensitiveUsername_ShouldWork(string username, bool expectedResult)
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var password = username == "testuser" ? "password123" :
                          username == "adminuser" ? "admin456" : "password123";

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsValidUser_ShouldHashPasswordWithSha256()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);
            var username = "testuser";
            var password = "password123";

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().BeTrue("because the password should be hashed with SHA256 and compared");
        }

        [Fact]
        public void IsValidUser_MultipleValidUsers_ShouldValidateCorrectly()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);

            // Act & Assert
            service.IsValidUser("testuser", "password123").Should().BeTrue();
            service.IsValidUser("adminuser", "admin456").Should().BeTrue();
            service.IsValidUser("guestuser", "guest789").Should().BeTrue();
        }

        [Fact]
        public void IsValidUser_EmptyUserList_ShouldReturnFalse()
        {
            // Arrange
            var emptyUserList = new List<User>();
            var mockAccessor = new Mock<IOptions<List<User>>>();
            mockAccessor.Setup(x => x.Value).Returns(emptyUserList);
            var service = new UserManagementService(mockAccessor.Object);

            // Act
            var result = service.IsValidUser("anyuser", "anypassword");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_WhitespaceCredentials_ShouldReturnFalse()
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);

            // Act & Assert
            service.IsValidUser("   ", "password123").Should().BeFalse();
            service.IsValidUser("testuser", "   ").Should().BeFalse();
            service.IsValidUser("   ", "   ").Should().BeFalse();
        }

        [Theory]
        [InlineData("testuser", "password123", true)]
        [InlineData("testuser", "password124", false)]
        [InlineData("testuser1", "password123", false)]
        [InlineData("adminuser", "admin456", true)]
        [InlineData("adminuser", "admin457", false)]
        public void IsValidUser_VariousCredentialCombinations_ShouldValidateCorrectly(
            string username, string password, bool expected)
        {
            // Arrange
            var service = new UserManagementService(_mockUserListAccessor.Object);

            // Act
            var result = service.IsValidUser(username, password);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidUser_SpecialCharactersInCredentials_ShouldHandleCorrectly()
        {
            // Arrange
            var specialUsers = new List<User>
            {
                new User { UserName = "user@domain.com", Password = "p@ssw0rd!".Sha256() },
                new User { UserName = "user#123", Password = "pa$$word".Sha256() }
            };
            var mockAccessor = new Mock<IOptions<List<User>>>();
            mockAccessor.Setup(x => x.Value).Returns(specialUsers);
            var service = new UserManagementService(mockAccessor.Object);

            // Act & Assert
            service.IsValidUser("user@domain.com", "p@ssw0rd!").Should().BeTrue();
            service.IsValidUser("user#123", "pa$$word").Should().BeTrue();
            service.IsValidUser("user@domain.com", "wrongpass").Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_UnicodeCredentials_ShouldHandleCorrectly()
        {
            // Arrange
            var unicodeUsers = new List<User>
            {
                new User { UserName = "用户", Password = "密码123".Sha256() },
                new User { UserName = "пользователь", Password = "пароль456".Sha256() }
            };
            var mockAccessor = new Mock<IOptions<List<User>>>();
            mockAccessor.Setup(x => x.Value).Returns(unicodeUsers);
            var service = new UserManagementService(mockAccessor.Object);

            // Act & Assert
            service.IsValidUser("用户", "密码123").Should().BeTrue();
            service.IsValidUser("пользователь", "пароль456").Should().BeTrue();
            service.IsValidUser("用户", "wrong").Should().BeFalse();
        }

        [Fact]
        public void IsValidUser_DuplicateUsernames_ShouldReturnTrueForAnyMatchingPassword()
        {
            // Arrange
            var duplicateUsers = new List<User>
            {
                new User { UserName = "duplicate", Password = "pass1".Sha256() },
                new User { UserName = "duplicate", Password = "pass2".Sha256() }
            };
            var mockAccessor = new Mock<IOptions<List<User>>>();
            mockAccessor.Setup(x => x.Value).Returns(duplicateUsers);
            var service = new UserManagementService(mockAccessor.Object);

            // Act & Assert
            service.IsValidUser("duplicate", "pass1").Should().BeTrue();
            service.IsValidUser("duplicate", "pass2").Should().BeTrue();
            service.IsValidUser("duplicate", "pass3").Should().BeFalse();
        }

        /// <summary>
        /// Creates test users with properly hashed passwords for testing
        /// </summary>
        private static List<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User
                {
                    UserName = "testuser",
                    Password = "password123".Sha256() // Hash the password as the service expects
                },
                new User
                {
                    UserName = "adminuser",
                    Password = "admin456".Sha256()
                },
                new User
                {
                    UserName = "guestuser",
                    Password = "guest789".Sha256()
                }
            };
        }
    }
}

// Extension method to make testing easier - mirrors the actual implementation
internal static class StringExtensions
{
    public static string Sha256(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
