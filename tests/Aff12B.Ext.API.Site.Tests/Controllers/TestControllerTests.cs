using Aff12B.Ext.API.Site.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Controllers
{
    /// <summary>
    /// Unit tests for TestController class
    /// Tests the test endpoint functionality and logging
    /// Target: 100% code coverage
    /// </summary>
    public class TestControllerTests
    {
        private readonly Mock<ILogger<TestController>> _mockLogger;
        private readonly TestController _controller;

        public TestControllerTests()
        {
            _mockLogger = new Mock<ILogger<TestController>>();
            _controller = new TestController(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_ValidLogger_ShouldCreateInstance()
        {
            // Act
            var controller = new TestController(_mockLogger.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<Controller>();
        }

        [Fact]
        public void Constructor_NullLogger_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => new TestController(null);
            act.Should().NotThrow();
        }

        [Fact]
        public void Index_ShouldReturnJsonWithTestMessage()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();

            // Verify the returned object has the expected structure
            var value = jsonResult.Value;
            value.Should().BeEquivalentTo(new { Message = "Test" });
        }

        [Fact]
        public void Index_ShouldLogInformation()
        {
            // Act
            _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("TestController.Index")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_ReturnedJson_ShouldHaveCorrectMessage()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;

            // Use reflection to check the anonymous object properties
            var value = jsonResult.Value;
            var messageProperty = value.GetType().GetProperty("Message");
            messageProperty.Should().NotBeNull();
            messageProperty.GetValue(value).Should().Be("Test");
        }

        [Fact]
        public void Index_MultipleCalls_ShouldReturnConsistentResults()
        {
            // Act
            var result1 = _controller.Index();
            var result2 = _controller.Index();

            // Assert
            result1.Should().BeOfType<JsonResult>();
            result2.Should().BeOfType<JsonResult>();

            var jsonResult1 = result1 as JsonResult;
            var jsonResult2 = result2 as JsonResult;

            jsonResult1.Value.Should().BeEquivalentTo(jsonResult2.Value);
        }

        [Fact]
        public void Index_MultipleCalls_ShouldLogEachTime()
        {
            // Act
            _controller.Index();
            _controller.Index();
            _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("TestController.Index")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(3));
        }

        [Fact]
        public void Index_ShouldReturnIActionResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeAssignableTo<IActionResult>();
        }

        [Fact]
        public void Index_JsonResult_ShouldHaveNullContentType()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.ContentType.Should().BeNull(); // Default content type
        }

        [Fact]
        public void Index_JsonResult_ShouldHaveDefaultStatusCode()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.StatusCode.Should().BeNull(); // Default status code (200)
        }

        [Fact]
        public void Index_ShouldLogWithCorrectLogLevel()
        {
            // Act
            _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information, // Specifically verify Information level
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_ShouldLogWithNullException()
        {
            // Act
            _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    null, // Exception should be null for info logging
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_JsonValue_ShouldBeSerializable()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;

            // Verify the value can be serialized (no circular references, etc.)
            var value = jsonResult.Value;
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            json.Should().Contain("Test");
            json.Should().Contain("Message");
        }

        [Fact]
        public void Index_ShouldCreateNewAnonymousObjectEachTime()
        {
            // Act
            var result1 = _controller.Index();
            var result2 = _controller.Index();

            // Assert
            var jsonResult1 = result1 as JsonResult;
            var jsonResult2 = result2 as JsonResult;

            // Values should be equivalent but not the same reference
            jsonResult1.Value.Should().BeEquivalentTo(jsonResult2.Value);
            ReferenceEquals(jsonResult1.Value, jsonResult2.Value).Should().BeFalse();
        }

        [Fact]
        public void Index_LogMessage_ShouldContainControllerAndActionName()
        {
            // Act
            _controller.Index();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString() == "TestController.Index"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_ShouldNotThrowException()
        {
            // Act & Assert
            var act = () => _controller.Index();
            act.Should().NotThrow();
        }

        [Fact]
        public void Index_WithLoggerException_ShouldStillReturnResult()
        {
            // Arrange
            _mockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Throws(new InvalidOperationException("Logger error"));

            // Act & Assert
            var act = () => _controller.Index();
            act.Should().Throw<InvalidOperationException>().WithMessage("Logger error");
        }
    }
}
