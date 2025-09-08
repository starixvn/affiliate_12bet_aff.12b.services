using Aff12B.Ext.API.Core.Params;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Aff12B.Ext.API.Core.Tests.Params
{
    /// <summary>
    /// Unit tests for TokenRequest parameter class
    /// Tests property validation, data annotations, edge cases, and authentication scenarios
    /// </summary>
    public class TokenRequestTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var tokenRequest = new TokenRequest();

            // Assert
            tokenRequest.Username.Should().BeNull();
            tokenRequest.Password.Should().BeNull();
            tokenRequest.IsShowIpvn.Should().BeTrue(); // Default value
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tokenRequest = new TokenRequest();
            const string expectedUsername = "testuser";
            const string expectedPassword = "password123";
            const bool expectedIsShowIpvn = false;

            // Act
            tokenRequest.Username = expectedUsername;
            tokenRequest.Password = expectedPassword;
            tokenRequest.IsShowIpvn = expectedIsShowIpvn;

            // Assert
            tokenRequest.Username.Should().Be(expectedUsername);
            tokenRequest.Password.Should().Be(expectedPassword);
            tokenRequest.IsShowIpvn.Should().Be(expectedIsShowIpvn);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("user")]
        [InlineData("testuser")]
        [InlineData("user123")]
        [InlineData("user_with_underscores")]
        [InlineData("user-with-dashes")]
        [InlineData("user.with.dots")]
        [InlineData("user@domain.com")]
        [InlineData("UPPERCASE_USER")]
        [InlineData("MixedCaseUser")]
        public void Username_ShouldHandleVariousUsernameFormats(string username)
        {
            // Arrange
            var tokenRequest = new TokenRequest();

            // Act
            tokenRequest.Username = username;

            // Assert
            tokenRequest.Username.Should().Be(username);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("pass")]
        [InlineData("password")]
        [InlineData("password123")]
        [InlineData("Pass123!")]
        [InlineData("P@ssw0rd")]
        [InlineData("1234567890123456")] // Exactly 16 characters (max length)
        public void Password_ShouldHandleVariousPasswordFormats(string password)
        {
            // Arrange
            var tokenRequest = new TokenRequest();

            // Act
            tokenRequest.Password = password;

            // Assert
            tokenRequest.Password.Should().Be(password);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsShowIpvn_ShouldHandleBooleanValues(bool isShowIpvn)
        {
            // Arrange
            var tokenRequest = new TokenRequest();

            // Act
            tokenRequest.IsShowIpvn = isShowIpvn;

            // Assert
            tokenRequest.IsShowIpvn.Should().Be(isShowIpvn);
        }

        [Fact]
        public void Username_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TokenRequest).GetProperty("Username");

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

            // Assert
            requiredAttribute.Should().NotBeNull();
        }

        [Fact]
        public void Password_ShouldHaveRequiredAttribute()
        {
            // Arrange
            var property = typeof(TokenRequest).GetProperty("Password");

            // Act
            var requiredAttribute = property?.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() as RequiredAttribute;

            // Assert
            requiredAttribute.Should().NotBeNull();
        }

        [Fact]
        public void Username_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var property = typeof(TokenRequest).GetProperty("Username");

            // Act
            var maxLengthAttribute = property?.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;

            // Assert
            maxLengthAttribute.Should().NotBeNull();
            maxLengthAttribute!.Length.Should().Be(50);
        }

        [Fact]
        public void Password_ShouldHaveMaxLengthAttribute()
        {
            // Arrange
            var property = typeof(TokenRequest).GetProperty("Password");

            // Act
            var maxLengthAttribute = property?.GetCustomAttributes(typeof(MaxLengthAttribute), false).FirstOrDefault() as MaxLengthAttribute;

            // Assert
            maxLengthAttribute.Should().NotBeNull();
            maxLengthAttribute!.Length.Should().Be(16);
        }

        [Fact]
        public void TokenRequest_ShouldValidateRequiredUsername()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = null, // Required field missing
                Password = "password123",
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Username"));
        }

        [Fact]
        public void TokenRequest_ShouldValidateRequiredPassword()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "testuser",
                Password = null, // Required field missing
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Password"));
        }

        [Fact]
        public void TokenRequest_ShouldValidateUsernameMaxLength()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = new string('a', 51), // Exceeds max length of 50
                Password = "password123",
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Username"));
        }

        [Fact]
        public void TokenRequest_ShouldValidatePasswordMaxLength()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "testuser",
                Password = new string('a', 17), // Exceeds max length of 16
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Password"));
        }

        [Fact]
        public void TokenRequest_ShouldPassValidationWithValidData()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "validuser",
                Password = "validpass123",
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void TokenRequest_ShouldSupportCompleteAuthenticationScenario()
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest
            {
                Username = "admin@company.com",
                Password = "SecureP@ss123",
                IsShowIpvn = false
            };

            // Assert
            tokenRequest.Username.Should().Contain("@");
            tokenRequest.Password.Should().NotBeNullOrEmpty();
            tokenRequest.IsShowIpvn.Should().BeFalse();
        }

        [Fact]
        public void Username_ShouldHandleEmailFormatUsernames()
        {
            // Arrange
            var tokenRequest = new TokenRequest();
            const string emailUsername = "user@example.com";

            // Act
            tokenRequest.Username = emailUsername;

            // Assert
            tokenRequest.Username.Should().Be(emailUsername);
            tokenRequest.Username.Should().Contain("@");
        }

        [Fact]
        public void Username_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var tokenRequest = new TokenRequest();
            const string unicodeUsername = "用户名_José_García";

            // Act
            tokenRequest.Username = unicodeUsername;

            // Assert
            tokenRequest.Username.Should().Be(unicodeUsername);
        }

        [Fact]
        public void Password_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var tokenRequest = new TokenRequest();
            const string specialPassword = "P@ss!#$%^&*()";

            // Act
            tokenRequest.Password = specialPassword;

            // Assert
            tokenRequest.Password.Should().Be(specialPassword);
        }

        [Fact]
        public void TokenRequest_ShouldHandleMaxLengthUsernameValidScenario()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = new string('u', 50), // Exactly max length
                Password = "password123",
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().BeEmpty();
            tokenRequest.Username.Length.Should().Be(50);
        }

        [Fact]
        public void TokenRequest_ShouldHandleMaxLengthPasswordValidScenario()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "testuser",
                Password = new string('p', 16), // Exactly max length
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().BeEmpty();
            tokenRequest.Password.Length.Should().Be(16);
        }

        [Fact]
        public void IsShowIpvn_ShouldDefaultToTrue()
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest
            {
                Username = "testuser",
                Password = "password123"
                // IsShowIpvn not explicitly set
            };

            // Assert
            tokenRequest.IsShowIpvn.Should().BeTrue();
        }

        [Theory]
        [InlineData("admin", "admin123", true)]
        [InlineData("user@domain.com", "P@ssw0rd", false)]
        [InlineData("testuser", "test123", true)]
        [InlineData("api_user", "api_pass", false)]
        public void TokenRequest_ShouldHandleMultipleAuthenticationScenarios(string username, string password, bool isShowIpvn)
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest
            {
                Username = username,
                Password = password,
                IsShowIpvn = isShowIpvn
            };

            // Assert
            tokenRequest.Username.Should().Be(username);
            tokenRequest.Password.Should().Be(password);
            tokenRequest.IsShowIpvn.Should().Be(isShowIpvn);
        }

        [Fact]
        public void TokenRequest_ShouldHandleEmptyStringsAsInvalidForRequiredFields()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "", // Empty string should be invalid for required field
                Password = "", // Empty string should be invalid for required field
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().NotBeEmpty();
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Username"));
            validationResults.Should().Contain(vr => vr.MemberNames.Contains("Password"));
        }

        [Fact]
        public void TokenRequest_ShouldHandleWhitespaceOnlyStringsAsInvalidForRequiredFields()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "   ", // Whitespace only should be invalid
                Password = "   ", // Whitespace only should be invalid
                IsShowIpvn = true
            };

            // Act
            var validationResults = ValidateModel(tokenRequest);

            // Assert
            validationResults.Should().NotBeEmpty();
        }

        [Fact]
        public void TokenRequest_ShouldHandleCaseSensitiveCredentials()
        {
            // Arrange
            var tokenRequest1 = new TokenRequest
            {
                Username = "TestUser",
                Password = "TestPass123",
                IsShowIpvn = true
            };

            var tokenRequest2 = new TokenRequest
            {
                Username = "testuser",
                Password = "testpass123",
                IsShowIpvn = true
            };

            // Act & Assert
            tokenRequest1.Username.Should().NotBe(tokenRequest2.Username);
            tokenRequest1.Password.Should().NotBe(tokenRequest2.Password);
        }

        [Fact]
        public void TokenRequest_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest
            {
                Username = "u",
                Password = "p"
                // IsShowIpvn uses default value
            };

            // Assert
            var validationResults = ValidateModel(tokenRequest);
            validationResults.Should().BeEmpty();
            tokenRequest.IsShowIpvn.Should().BeTrue();
        }

        [Fact]
        public void TokenRequest_ShouldHandleIpvnDisplayControlScenario()
        {
            // Arrange & Act - Scenario where IPVN display is disabled
            var tokenRequest = new TokenRequest
            {
                Username = "secureuser",
                Password = "securepass123",
                IsShowIpvn = false
            };

            // Assert
            tokenRequest.IsShowIpvn.Should().BeFalse();
            tokenRequest.Username.Should().NotBeNullOrEmpty();
            tokenRequest.Password.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Helper method to validate model using data annotations
        /// </summary>
        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
