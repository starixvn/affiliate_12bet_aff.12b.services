using Aff12B.Common.DataDomain.Api;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Api
{
    /// <summary>
    /// Unit tests for ResultInfo - Target: 95% Coverage
    /// </summary>
    public class ResultInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var resultInfo = new ResultInfo();

            // Assert
            resultInfo.Should().NotBeNull();
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.IsSuccess = true;
            resultInfo.ErrMsg = "Test error message";

            // Assert
            resultInfo.IsSuccess.Should().BeTrue();
            resultInfo.ErrMsg.Should().Be("Test error message");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSuccess_ShouldHandleBooleanValues(bool isSuccess)
        {
            // Arrange
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.IsSuccess = isSuccess;

            // Assert
            resultInfo.IsSuccess.Should().Be(isSuccess);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Success")]
        [InlineData("Error occurred")]
        [InlineData("Validation failed: Invalid input")]
        [InlineData("Very long error message with detailed information about what went wrong")]
        public void ErrMsg_ShouldHandleStringValues(string errMsg)
        {
            // Arrange
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = errMsg;

            // Assert
            resultInfo.ErrMsg.Should().Be(errMsg);
        }

        [Fact]
        public void ResultInfo_ShouldHandleSuccessScenario()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = true,
                ErrMsg = null
            };

            // Assert
            resultInfo.IsSuccess.Should().BeTrue();
            resultInfo.ErrMsg.Should().BeNull();
        }

        [Fact]
        public void ResultInfo_ShouldHandleFailureScenario()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Operation failed due to invalid parameters"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Be("Operation failed due to invalid parameters");
        }

        [Fact]
        public void ResultInfo_ShouldHandleValidationError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Validation error: Email address is required"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Validation error");
            resultInfo.ErrMsg.Should().Contain("Email address is required");
        }

        [Fact]
        public void ResultInfo_ShouldHandleAuthenticationError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Authentication failed: Invalid credentials"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Authentication failed");
        }

        [Fact]
        public void ResultInfo_ShouldHandleAuthorizationError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Authorization error: Insufficient permissions"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Authorization error");
        }

        [Fact]
        public void ResultInfo_ShouldHandleNetworkError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Network error: Connection timeout"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Network error");
        }

        [Fact]
        public void ResultInfo_ShouldHandleDatabaseError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Database error: Connection string is invalid"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Database error");
        }

        [Fact]
        public void ResultInfo_ShouldHandleBusinessLogicError()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = "Business rule violation: Account balance is insufficient"
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().Contain("Business rule violation");
        }

        [Fact]
        public void ResultInfo_ShouldHandleSpecialCharactersInErrorMessage()
        {
            // Arrange
            var specialChars = "Error with special chars: !@#$%^&*()_+-=[]{}|;:,.<>?";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = specialChars;

            // Assert
            resultInfo.ErrMsg.Should().Be(specialChars);
        }

        [Fact]
        public void ResultInfo_ShouldHandleUnicodeCharactersInErrorMessage()
        {
            // Arrange
            var unicodeMessage = "Error message: 中文 Русский العربية 日本語";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = unicodeMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(unicodeMessage);
        }

        [Fact]
        public void ResultInfo_ShouldHandleVeryLongErrorMessage()
        {
            // Arrange
            var longMessage = new string('E', 5000); // 5KB error message
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = longMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(longMessage);
            resultInfo.ErrMsg.Length.Should().Be(5000);
        }

        [Fact]
        public void ResultInfo_ShouldHandleMultilineErrorMessage()
        {
            // Arrange
            var multilineMessage = @"Error occurred:
Line 1: First error detail
Line 2: Second error detail
Line 3: Third error detail";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = multilineMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(multilineMessage);
            resultInfo.ErrMsg.Should().Contain("\n");
        }

        [Fact]
        public void ResultInfo_ShouldHandleJsonLikeErrorMessage()
        {
            // Arrange
            var jsonLikeMessage = @"{""error"": ""Invalid request"", ""code"": 400, ""details"": ""Missing required field""}";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = jsonLikeMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(jsonLikeMessage);
            resultInfo.ErrMsg.Should().Contain("error");
            resultInfo.ErrMsg.Should().Contain("code");
        }

        [Fact]
        public void ResultInfo_ShouldHandleXmlLikeErrorMessage()
        {
            // Arrange
            var xmlLikeMessage = @"<error><code>500</code><message>Internal server error</message></error>";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = xmlLikeMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(xmlLikeMessage);
            resultInfo.ErrMsg.Should().Contain("<error>");
            resultInfo.ErrMsg.Should().Contain("</error>");
        }

        [Fact]
        public void ResultInfo_ShouldHandleWhitespaceInErrorMessage()
        {
            // Arrange
            var whitespaceMessage = "   Error with leading and trailing spaces   ";
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = whitespaceMessage;

            // Assert
            resultInfo.ErrMsg.Should().Be(whitespaceMessage);
        }

        [Fact]
        public void ResultInfo_ShouldHandleEmptyStringErrorMessage()
        {
            // Arrange
            var resultInfo = new ResultInfo();

            // Act
            resultInfo.ErrMsg = "";

            // Assert
            resultInfo.ErrMsg.Should().Be("");
            resultInfo.ErrMsg.Should().BeEmpty();
        }

        [Fact]
        public void ResultInfo_ShouldHandleSuccessWithEmptyErrorMessage()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = true,
                ErrMsg = ""
            };

            // Assert
            resultInfo.IsSuccess.Should().BeTrue();
            resultInfo.ErrMsg.Should().Be("");
        }

        [Fact]
        public void ResultInfo_ShouldHandleFailureWithNullErrorMessage()
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = false,
                ErrMsg = null
            };

            // Assert
            resultInfo.IsSuccess.Should().BeFalse();
            resultInfo.ErrMsg.Should().BeNull();
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(true, "")]
        [InlineData(true, "Success message")]
        [InlineData(false, "Error message")]
        [InlineData(false, null)]
        [InlineData(false, "")]
        public void ResultInfo_ShouldHandleVariousCombinations(bool isSuccess, string errMsg)
        {
            // Arrange & Act
            var resultInfo = new ResultInfo
            {
                IsSuccess = isSuccess,
                ErrMsg = errMsg
            };

            // Assert
            resultInfo.IsSuccess.Should().Be(isSuccess);
            resultInfo.ErrMsg.Should().Be(errMsg);
        }
    }
}
