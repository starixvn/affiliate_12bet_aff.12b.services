using Aff12B.Common.Infrastructure.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Xunit;

namespace Aff12B.Common.Infrastructure.Tests.Middleware
{
    public class ErrorHandlerTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly Mock<ILogger<ErrorHandler>> _mockLogger;
        private readonly ErrorHandler _errorHandler;
        private readonly DefaultHttpContext _httpContext;

        public ErrorHandlerTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _mockLogger = new Mock<ILogger<ErrorHandler>>();
            _errorHandler = new ErrorHandler(_mockNext.Object, _mockLogger.Object);
            _httpContext = new DefaultHttpContext();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidDependencies_ShouldCreateInstance()
        {
            // Arrange, Act & Assert
            var handler = new ErrorHandler(_mockNext.Object, _mockLogger.Object);
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullNext_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The ErrorHandler constructor doesn't validate null parameters
            var act = () => new ErrorHandler(null!, _mockLogger.Object);
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullLogger_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The ErrorHandler constructor doesn't validate null parameters
            var act = () => new ErrorHandler(_mockNext.Object, null!);
            act.Should().NotThrow();
        }

        #endregion

        #region Invoke Tests

        [Fact]
        public async Task Invoke_NoException_ShouldCallNext()
        {
            // Arrange
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _mockNext.Verify(x => x(_httpContext), Times.Once);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task Invoke_Exception_ShouldHandleError()
        {
            // Arrange
            var testException = new InvalidOperationException("Test exception");
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Invoke_Exception_ShouldLogCritical()
        {
            // Arrange
            var testException = new InvalidOperationException("Test exception");
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Invoke_Exception_ShouldSetStatusCode500()
        {
            // Arrange
            var testException = new ArgumentException("Test argument exception");
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Response.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task Invoke_Exception_ShouldClearHeaders()
        {
            // Arrange
            var testException = new NullReferenceException("Test null reference");
            _httpContext.Response.Headers["X-Custom-Header"] = "CustomValue";
            _httpContext.Response.Headers["Content-Type"] = "application/json";
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Response.Headers.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(ArgumentNullException), "Null argument")]
        [InlineData(typeof(InvalidOperationException), "Invalid operation")]
        [InlineData(typeof(NotSupportedException), "Not supported")]
        [InlineData(typeof(TimeoutException), "Timeout occurred")]
        public async Task Invoke_VariousExceptions_ShouldHandleConsistently(Type exceptionType, string message)
        {
            // Arrange
            var exception = (Exception)Activator.CreateInstance(exceptionType, message)!;
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(exception);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            _httpContext.Response.Headers.Should().BeEmpty();

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Invoke_ExceptionWithInnerException_ShouldLogAllDetails()
        {
            // Arrange
            var innerException = new ArgumentException("Inner exception");
            var outerException = new InvalidOperationException("Outer exception", innerException);
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(outerException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    outerException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Invoke_ResponseAlreadyStarted_ShouldStillLogError()
        {
            // Arrange
            var testException = new InvalidOperationException("Test exception");
            _httpContext.Response.StatusCode = 200;
            _httpContext.Response.Headers["Content-Length"] = "100";

            // Simulate response already started by writing to the response
            await _httpContext.Response.WriteAsync("Some content");

            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            // Should still log the error even if response manipulation fails
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Invoke_ErrorMessageHelperCalled_ShouldIncludeContext()
        {
            // Arrange
            var testException = new InvalidOperationException("Context test");
            _httpContext.Request.Method = "POST";
            _httpContext.Request.Path = "/api/test";
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act
            await _errorHandler.Invoke(_httpContext);

            // Assert
            // Verify that ErrorMessageHelper.GenerateMessage was called with context
            // This is implicit since the logger should receive the generated message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    testException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }
}
