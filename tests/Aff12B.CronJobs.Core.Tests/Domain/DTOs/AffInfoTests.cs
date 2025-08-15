using Aff12B.CronJobs.Core.Domain.DTOs;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs
{
    /// <summary>
    /// Unit tests for AffInfo DTO class
    /// Tests property validation, edge cases, and data integrity
    /// </summary>
    public class AffInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var affInfo = new AffInfo();

            // Assert
            affInfo.AffId.Should().Be(0);
            affInfo.AffCode.Should().BeNull();
            affInfo.ParentAff.Should().BeNull();
            affInfo.UserName.Should().BeNull();
            affInfo.Player_CustId.Should().Be(0);
            affInfo.Player_Username.Should().BeNull();
            affInfo.FirstName.Should().BeNull();
            affInfo.LastName.Should().BeNull();
            affInfo.Phone.Should().BeNull();
            affInfo.Email.Should().BeNull();
            affInfo.Status.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var affInfo = new AffInfo();
            const int expectedAffId = 12345;
            const string expectedAffCode = "AFF001";
            const string expectedParentAff = "PARENT001";
            const string expectedUserName = "testuser";
            const int expectedPlayerCustId = 67890;
            const string expectedPlayerUsername = "player123";
            const string expectedFirstName = "John";
            const string expectedLastName = "Doe";
            const string expectedPhone = "+1-555-0123";
            const string expectedEmail = "john.doe@example.com";
            const string expectedStatus = "Active";

            // Act
            affInfo.AffId = expectedAffId;
            affInfo.AffCode = expectedAffCode;
            affInfo.ParentAff = expectedParentAff;
            affInfo.UserName = expectedUserName;
            affInfo.Player_CustId = expectedPlayerCustId;
            affInfo.Player_Username = expectedPlayerUsername;
            affInfo.FirstName = expectedFirstName;
            affInfo.LastName = expectedLastName;
            affInfo.Phone = expectedPhone;
            affInfo.Email = expectedEmail;
            affInfo.Status = expectedStatus;

            // Assert
            affInfo.AffId.Should().Be(expectedAffId);
            affInfo.AffCode.Should().Be(expectedAffCode);
            affInfo.ParentAff.Should().Be(expectedParentAff);
            affInfo.UserName.Should().Be(expectedUserName);
            affInfo.Player_CustId.Should().Be(expectedPlayerCustId);
            affInfo.Player_Username.Should().Be(expectedPlayerUsername);
            affInfo.FirstName.Should().Be(expectedFirstName);
            affInfo.LastName.Should().Be(expectedLastName);
            affInfo.Phone.Should().Be(expectedPhone);
            affInfo.Email.Should().Be(expectedEmail);
            affInfo.Status.Should().Be(expectedStatus);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void AffId_ShouldHandleVariousIntegerValues(int affId)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.AffId = affId;

            // Assert
            affInfo.AffId.Should().Be(affId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void Player_CustId_ShouldHandleVariousIntegerValues(int custId)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.Player_CustId = custId;

            // Assert
            affInfo.Player_CustId.Should().Be(custId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("AFF001")]
        [InlineData("AFFILIATE_CODE_WITH_UNDERSCORES")]
        [InlineData("123456789")]
        [InlineData("Special!@#$%")]
        public void AffCode_ShouldHandleVariousStringValues(string affCode)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.AffCode = affCode;

            // Assert
            affInfo.AffCode.Should().Be(affCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("john.doe@example.com")]
        [InlineData("user+tag@domain.co.uk")]
        [InlineData("invalid-email")]
        [InlineData("test@")]
        [InlineData("@domain.com")]
        public void Email_ShouldHandleVariousEmailFormats(string email)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.Email = email;

            // Assert
            affInfo.Email.Should().Be(email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("+1-555-0123")]
        [InlineData("555-0123")]
        [InlineData("(555) 012-3456")]
        [InlineData("555.012.3456")]
        [InlineData("5550123456")]
        [InlineData("+44 20 7946 0958")]
        public void Phone_ShouldHandleVariousPhoneFormats(string phone)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.Phone = phone;

            // Assert
            affInfo.Phone.Should().Be(phone);
        }

        [Theory]
        [InlineData("Active")]
        [InlineData("Inactive")]
        [InlineData("Suspended")]
        [InlineData("Pending")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ACTIVE")]
        [InlineData("inactive")]
        public void Status_ShouldHandleVariousStatusValues(string status)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.Status = status;

            // Assert
            affInfo.Status.Should().Be(status);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.AffCode = null;
            affInfo.ParentAff = null;
            affInfo.UserName = null;
            affInfo.Player_Username = null;
            affInfo.FirstName = null;
            affInfo.LastName = null;
            affInfo.Phone = null;
            affInfo.Email = null;
            affInfo.Status = null;

            // Assert
            affInfo.AffCode.Should().BeNull();
            affInfo.ParentAff.Should().BeNull();
            affInfo.UserName.Should().BeNull();
            affInfo.Player_Username.Should().BeNull();
            affInfo.FirstName.Should().BeNull();
            affInfo.LastName.Should().BeNull();
            affInfo.Phone.Should().BeNull();
            affInfo.Email.Should().BeNull();
            affInfo.Status.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var affInfo = new AffInfo();
            const string unicodeText = "测试用户名 José García Müller 🎯";

            // Act
            affInfo.AffCode = unicodeText;
            affInfo.ParentAff = unicodeText;
            affInfo.UserName = unicodeText;
            affInfo.Player_Username = unicodeText;
            affInfo.FirstName = unicodeText;
            affInfo.LastName = unicodeText;
            affInfo.Phone = unicodeText;
            affInfo.Email = unicodeText;
            affInfo.Status = unicodeText;

            // Assert
            affInfo.AffCode.Should().Be(unicodeText);
            affInfo.ParentAff.Should().Be(unicodeText);
            affInfo.UserName.Should().Be(unicodeText);
            affInfo.Player_Username.Should().Be(unicodeText);
            affInfo.FirstName.Should().Be(unicodeText);
            affInfo.LastName.Should().Be(unicodeText);
            affInfo.Phone.Should().Be(unicodeText);
            affInfo.Email.Should().Be(unicodeText);
            affInfo.Status.Should().Be(unicodeText);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleLargeStrings()
        {
            // Arrange
            var affInfo = new AffInfo();
            var largeString = new string('A', 5000); // 5KB string

            // Act & Assert - Should not throw
            affInfo.AffCode = largeString;
            affInfo.ParentAff = largeString;
            affInfo.UserName = largeString;
            affInfo.Player_Username = largeString;
            affInfo.FirstName = largeString;
            affInfo.LastName = largeString;
            affInfo.Phone = largeString;
            affInfo.Email = largeString;
            affInfo.Status = largeString;

            // Verify assignment worked
            affInfo.AffCode.Should().Be(largeString);
            affInfo.ParentAff.Should().Be(largeString);
            affInfo.UserName.Should().Be(largeString);
            affInfo.Player_Username.Should().Be(largeString);
            affInfo.FirstName.Should().Be(largeString);
            affInfo.LastName.Should().Be(largeString);
            affInfo.Phone.Should().Be(largeString);
            affInfo.Email.Should().Be(largeString);
            affInfo.Status.Should().Be(largeString);
        }

        [Fact]
        public void AffInfo_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var affInfo = new AffInfo
            {
                AffId = 12345,
                AffCode = "AFF001",
                ParentAff = "PARENT001",
                UserName = "affiliate_user",
                Player_CustId = 67890,
                Player_Username = "player123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "+1-555-0123",
                Email = "john.doe@example.com",
                Status = "Active"
            };

            // Assert
            affInfo.AffId.Should().Be(12345);
            affInfo.AffCode.Should().Be("AFF001");
            affInfo.ParentAff.Should().Be("PARENT001");
            affInfo.UserName.Should().Be("affiliate_user");
            affInfo.Player_CustId.Should().Be(67890);
            affInfo.Player_Username.Should().Be("player123");
            affInfo.FirstName.Should().Be("John");
            affInfo.LastName.Should().Be("Doe");
            affInfo.Phone.Should().Be("+1-555-0123");
            affInfo.Email.Should().Be("john.doe@example.com");
            affInfo.Status.Should().Be("Active");
        }

        [Fact]
        public void AffInfo_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var affInfo = new AffInfo
            {
                AffId = 12345,
                AffCode = "AFF001",
                Status = "Active"
                // Other properties left as default/null
            };

            // Assert
            affInfo.AffId.Should().Be(12345);
            affInfo.AffCode.Should().Be("AFF001");
            affInfo.Status.Should().Be("Active");
            affInfo.ParentAff.Should().BeNull();
            affInfo.UserName.Should().BeNull();
            affInfo.Player_CustId.Should().Be(0);
            affInfo.Player_Username.Should().BeNull();
            affInfo.FirstName.Should().BeNull();
            affInfo.LastName.Should().BeNull();
            affInfo.Phone.Should().BeNull();
            affInfo.Email.Should().BeNull();
        }

        [Theory]
        [InlineData("ACTIVE", "Active")]
        [InlineData("active", "Active")]
        [InlineData("INACTIVE", "Inactive")]
        [InlineData("inactive", "Inactive")]
        public void Status_ShouldPreserveCase(string inputStatus, string expectedStatus)
        {
            // Arrange
            var affInfo = new AffInfo();

            // Act
            affInfo.Status = inputStatus;

            // Assert - Status should preserve original casing, not normalize
            affInfo.Status.Should().Be(inputStatus);
            affInfo.Status.Should().NotBe(expectedStatus, "because case should be preserved");
        }
    }
}
