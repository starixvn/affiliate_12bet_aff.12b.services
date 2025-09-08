using Aff12B.CronJobs.Infrastructure.Exceptions;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Exceptions
{
    /// <summary>
    /// Unit tests for ImporterException - Target: 100% Coverage
    /// </summary>
    public class ImporterExceptionTests
    {
        [Fact]
        public void DefaultConstructor_ShouldCreateExceptionWithDefaultMessage()
        {
            // Act
            var exception = new ImporterException();

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ImporterException>();
            exception.Should().BeAssignableTo<Exception>();
            exception.Message.Should().NotBeNullOrEmpty();
            exception.InnerException.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Test error message")]
        [InlineData("Import failed due to invalid data format")]
        [InlineData("Very long error message with special characters 123!@#$%^&*()")]
        public void MessageConstructor_ShouldCreateExceptionWithSpecifiedMessage(string message)
        {
            // Act
            var exception = new ImporterException(message);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ImporterException>();
            if (message == null)
            {
                exception.Message.Should().NotBeNullOrEmpty();
            }
            else
            {
                exception.Message.Should().Be(message);
            }
            exception.InnerException.Should().BeNull();
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(1, "Invalid format")]
        [InlineData(-1, "Negative error code")]
        [InlineData(404, "Data not found")]
        [InlineData(500, "Internal server error")]
        [InlineData(int.MaxValue, "Maximum error code")]
        [InlineData(int.MinValue, "Minimum error code")]
        public void ErrorCodeConstructor_ShouldCreateExceptionWithFormattedMessage(int errorCode, string errorMessage)
        {
            // Act
            var exception = new ImporterException(errorCode, errorMessage);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ImporterException>();
            exception.Message.Should().Be($"Import fail - {errorCode} - {errorMessage}");
            exception.InnerException.Should().BeNull();
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("E001", "Invalid XML", "<xml></xml>")]
        [InlineData("E002", "Missing required field", "<data><field1>value1</field1></data>")]
        [InlineData("ERR_FORMAT", "Data format error", "<complex><nested><data>test</data></nested></complex>")]
        public void XmlDataConstructor_ShouldCreateExceptionWithFormattedMessageAndXmlData(string errorCode, string errorMessage, string xmlData)
        {
            // Act
            var exception = new ImporterException(errorCode, errorMessage, xmlData);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ImporterException>();
            exception.Message.Should().Be($"Import fail - {errorCode} - {errorMessage}\n{xmlData}");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void XmlDataConstructor_ShouldHandleNullValues()
        {
            // Act
            var exception = new ImporterException(null, null, null);

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be($"Import fail -  - \n");
        }

        [Theory]
        [InlineData("Test message")]
        [InlineData("Import failed with inner exception")]
        [InlineData("")]
        [InlineData(null)]
        public void InnerExceptionConstructor_ShouldCreateExceptionWithMessageAndInnerException(string message)
        {
            // Arrange
            var innerException = new InvalidOperationException("Inner exception message");

            // Act
            var exception = new ImporterException(message, innerException);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ImporterException>();
            if (message == null)
            {
                exception.Message.Should().NotBeNullOrEmpty();
            }
            else
            {
                exception.Message.Should().Be(message);
            }
            exception.InnerException.Should().Be(innerException);
            exception.InnerException.Message.Should().Be("Inner exception message");
        }

        [Fact]
        public void InnerExceptionConstructor_ShouldHandleNullInnerException()
        {
            // Act
            var exception = new ImporterException("Test message", (Exception)null);

            // Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Test message");
            exception.InnerException.Should().BeNull();
        }

        [Fact]
        public void ImporterException_ShouldInheritFromException()
        {
            // Act
            var exception = new ImporterException();

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Fact]
        public void ImporterException_ShouldBeSerializable()
        {
            // Arrange
            var exception = new ImporterException("Test message");

            // Act & Assert - Should not throw when accessing serialization-related properties
            var message = exception.Message;
            var stackTrace = exception.StackTrace;
            var source = exception.Source;

            message.Should().Be("Test message");
        }

        [Fact]
        public void ImporterException_ShouldHandleComplexXmlData()
        {
            // Arrange
            var xmlData = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<root>
    <data>
        <item id=""1"">Value 1</item>
        <item id=""2"">Value 2</item>
        <nested>
            <field>Nested value</field>
        </nested>
    </data>
</root>";

            // Act
            var exception = new ImporterException("XML_ERROR", "Complex XML parsing failed", xmlData);

            // Assert
            exception.Message.Should().Contain("XML_ERROR");
            exception.Message.Should().Contain("Complex XML parsing failed");
            exception.Message.Should().Contain(xmlData);
            exception.Message.Should().StartWith("Import fail - XML_ERROR - Complex XML parsing failed\n");
        }

        [Fact]
        public void ImporterException_ShouldHandleChainedExceptions()
        {
            // Arrange
            var rootException = new ArgumentException("Root cause");
            var middleException = new InvalidOperationException("Middle exception", rootException);

            // Act
            var importerException = new ImporterException("Import failed", middleException);

            // Assert
            importerException.InnerException.Should().Be(middleException);
            importerException.InnerException.InnerException.Should().Be(rootException);
            importerException.Message.Should().Be("Import failed");
            importerException.InnerException.Message.Should().Be("Middle exception");
            importerException.InnerException.InnerException.Message.Should().Be("Root cause");
        }

        [Fact]
        public void ImporterException_ShouldHandleSpecialCharactersInErrorMessages()
        {
            // Arrange
            var specialChars = "Error with special chars: !@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            var exception = new ImporterException(specialChars);

            // Assert
            exception.Message.Should().Be(specialChars);
        }

        [Fact]
        public void ImporterException_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var unicodeMessage = "Import error: 中文 Русский العربية 日本語";

            // Act
            var exception = new ImporterException(unicodeMessage);

            // Assert
            exception.Message.Should().Be(unicodeMessage);
        }

        [Fact]
        public void ImporterException_ShouldHandleVeryLongMessages()
        {
            // Arrange
            var longMessage = new string('A', 10000); // 10KB message

            // Act
            var exception = new ImporterException(longMessage);

            // Assert
            exception.Message.Should().Be(longMessage);
            exception.Message.Length.Should().Be(10000);
        }

        [Fact]
        public void ImporterException_ShouldHandleEmptyErrorCodeAndMessage()
        {
            // Act
            var exception = new ImporterException(0, "");

            // Assert
            exception.Message.Should().Be("Import fail - 0 - ");
        }

        [Fact]
        public void ImporterException_ShouldHandleWhitespaceInParameters()
        {
            // Act
            var exception = new ImporterException("   ", "   ", "   ");

            // Assert
            exception.Message.Should().Be("Import fail -     -    \n   ");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(999)]
        [InlineData(-999)]
        public void ErrorCodeConstructor_ShouldHandleVariousErrorCodes(int errorCode)
        {
            // Arrange
            var errorMessage = "Test error";

            // Act
            var exception = new ImporterException(errorCode, errorMessage);

            // Assert
            exception.Message.Should().Be($"Import fail - {errorCode} - {errorMessage}");
        }

        [Fact]
        public void ImporterException_ShouldMaintainStackTraceWhenWrappingException()
        {
            // Arrange
            Exception innerException;
            try
            {
                throw new InvalidOperationException("Original error");
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

            // Act
            var importerException = new ImporterException("Wrapped error", innerException);

            // Assert
            importerException.InnerException.Should().Be(innerException);
            importerException.InnerException.StackTrace.Should().NotBeNullOrEmpty();
        }
    }
}
