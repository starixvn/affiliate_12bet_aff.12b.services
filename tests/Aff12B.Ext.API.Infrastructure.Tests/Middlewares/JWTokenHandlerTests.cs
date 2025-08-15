using Aff12B.Ext.API.Infastructure.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Middlewares
{
    public class JWTokenHandlerTests
    {
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly JWTokenHandler _tokenHandler;
        private readonly DefaultHttpContext _httpContext;

        public JWTokenHandlerTests()
        {
            _mockNext = new Mock<RequestDelegate>();
            _tokenHandler = new JWTokenHandler(_mockNext.Object);
            _httpContext = new DefaultHttpContext();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidDelegate_ShouldCreateInstance()
        {
            // Arrange, Act & Assert
            var handler = new JWTokenHandler(_mockNext.Object);
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullDelegate_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The JWTokenHandler constructor doesn't validate the delegate parameter
            var act = () => new JWTokenHandler(null!);
            act.Should().NotThrow();
        }

        #endregion

        #region Invoke Tests

        [Fact]
        public async Task Invoke_HasAuthHeader_ShouldNotAddToken()
        {
            // Arrange
            _httpContext.Request.Headers["Authorization"] = "Bearer existing-token";
            _httpContext.Request.QueryString = new QueryString("?access_token=query-token");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer existing-token");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_NoAuthHeader_WithQueryToken_ShouldAddBearer()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?access_token=test-token-123");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer test-token-123");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_NoAuthHeader_NoQueryToken_ShouldContinue()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?other_param=value");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers.ContainsKey("Authorization").Should().BeFalse();
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_EmptyQueryToken_ShouldNotAddHeader()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?access_token=");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers.ContainsKey("Authorization").Should().BeFalse();
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Theory]
        [InlineData("simple-token", "Bearer simple-token")]
        [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
        [InlineData("token-with-special-chars!@#$%", "Bearer token-with-special-chars!@#$%")]
        [InlineData("12345", "Bearer 12345")]
        public async Task Invoke_VariousTokenFormats_ShouldHandle(string token, string expectedHeader)
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString($"?access_token={token}");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be(expectedHeader);
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_MultipleQueryParams_WithToken_ShouldAddHeader()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?param1=value1&access_token=my-token&param2=value2");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer my-token");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_CaseInsensitiveAuthHeader_ShouldNotAddToken()
        {
            // Arrange
            _httpContext.Request.Headers["authorization"] = "Bearer existing-token"; // lowercase
            _httpContext.Request.QueryString = new QueryString("?access_token=query-token");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // Should not add token since Authorization header already exists (case-insensitive)
            _httpContext.Request.Headers["Authorization"].Count.Should().Be(1);
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer existing-token");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhitespaceToken_ShouldAddHeader()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?access_token=   ");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // The implementation uses IsNullOrEmpty, so whitespace-only tokens are considered valid
            _httpContext.Request.Headers.ContainsKey("Authorization").Should().BeTrue();
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer    ");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        #endregion

        #region GetToken Tests (via Invoke behavior)

        [Fact]
        public async Task GetToken_ValidQueryParam_ShouldReturnToken()
        {
            // Arrange
            const string expectedToken = "test-token-value";
            _httpContext.Request.QueryString = new QueryString($"?access_token={expectedToken}");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // We can't directly test GetToken since it's private, but we can verify its behavior
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be($"Bearer {expectedToken}");
        }

        [Fact]
        public async Task GetToken_NoQueryParam_ShouldReturnNull()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?other_param=value");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // No Authorization header should be added when no access_token is present
            _httpContext.Request.Headers.ContainsKey("Authorization").Should().BeFalse();
        }

        [Fact]
        public async Task GetToken_MultipleAccessTokens_ShouldReturnFirst()
        {
            // Arrange
            _httpContext.Request.QueryString = new QueryString("?access_token=first-token&access_token=second-token");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // Should use the first token when multiple are present
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer first-token");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Invoke_RealWorldScenario_ApiCallWithToken_ShouldWork()
        {
            // Arrange
            _httpContext.Request.Method = "GET";
            _httpContext.Request.Path = "/api/customers";
            _httpContext.Request.QueryString = new QueryString("?access_token=real-jwt-token-here&page=1&limit=10");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer real-jwt-token-here");
            _mockNext.Verify(x => x(_httpContext), Times.Once);
        }

        [Fact]
        public async Task Invoke_NextThrowsException_ShouldPropagateException()
        {
            // Arrange
            var testException = new InvalidOperationException("Next middleware failed");
            _httpContext.Request.QueryString = new QueryString("?access_token=test-token");
            _mockNext.Setup(x => x(_httpContext)).ThrowsAsync(testException);

            // Act & Assert
            var act = async () => await _tokenHandler.Invoke(_httpContext);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Next middleware failed");

            // Token should still be added before the exception
            _httpContext.Request.Headers["Authorization"].ToString().Should().Be("Bearer test-token");
        }

        [Fact]
        public async Task Invoke_UrlEncodedToken_ShouldHandleCorrectly()
        {
            // Arrange
            // URL encoded token with special characters
            _httpContext.Request.QueryString = new QueryString("?access_token=token%2Bwith%2Bplus%2Bsigns");
            _mockNext.Setup(x => x(_httpContext)).Returns(Task.CompletedTask);

            // Act
            await _tokenHandler.Invoke(_httpContext);

            // Assert
            // The QueryString should handle URL decoding automatically
            _httpContext.Request.Headers["Authorization"].ToString().Should().Contain("Bearer");
        }

        #endregion
    }
}
