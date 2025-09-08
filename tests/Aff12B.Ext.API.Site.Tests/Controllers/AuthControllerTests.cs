using Aff12B.Ext.API.Core.Models;
using Aff12B.Ext.API.Core.Params;
using Aff12B.Ext.API.Core.Services;
using Aff12B.Ext.API.Site.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Controllers
{
    /// <summary>
    /// Unit tests for AuthController class
    /// Tests authentication endpoint and token request handling
    /// Target: 100% code coverage
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public void Constructor_ValidAuthService_ShouldCreateInstance()
        {
            // Act
            var controller = new AuthController(_mockAuthService.Object);

            // Assert
            controller.Should().NotBeNull();
            controller.Should().BeAssignableTo<ControllerBase>();
        }

        [Fact]
        public void Constructor_NullAuthService_ShouldNotThrowException()
        {
            // Act & Assert - The actual implementation allows null auth service
            var act = () => new AuthController(null);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task RequestToken_ValidRequest_ShouldReturnOkWithToken()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "password" };
            var expectedToken = new Token
            {
                AccessToken = "test-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(true, expectedToken));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(expectedToken);
        }

        [Fact]
        public async Task RequestToken_InvalidCredentials_ShouldReturnBadRequestWithMessage()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "wrongpassword" };
            var emptyToken = new Token();

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(false, emptyToken));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Wrong username or password");
        }

        [Fact]
        public async Task RequestToken_InvalidModelState_ShouldReturnBadRequestWithInvalidMessage()
        {
            // Arrange
            var request = new TokenRequest { Username = "", Password = "" };
            _controller.ModelState.AddModelError("Username", "Username is required");
            _controller.ModelState.AddModelError("Password", "Password is required");

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid Request");

            // Verify AuthService was not called
            _mockAuthService.Verify(x => x.IsAuthenticated(It.IsAny<TokenRequest>()), Times.Never);
        }

        [Fact]
        public async Task RequestToken_AuthServiceReturnsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "password" };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(false, (Token)null));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Wrong username or password");
        }

        [Fact]
        public async Task RequestToken_AuthServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "password" };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ThrowsAsync(new InvalidOperationException("Auth service error"));

            // Act & Assert
            var act = async () => await _controller.RequestToken(request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Auth service error");
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("username", null)]
        [InlineData(null, null)]
        [InlineData("", "password")]
        [InlineData("username", "")]
        [InlineData("", "")]
        public async Task RequestToken_VariousInvalidInputs_ShouldReturnBadRequest(string username, string password)
        {
            // Arrange
            var request = new TokenRequest { Username = username, Password = password };

            // Simulate model validation failure
            if (string.IsNullOrEmpty(username))
                _controller.ModelState.AddModelError("Username", "Username is required");
            if (string.IsNullOrEmpty(password))
                _controller.ModelState.AddModelError("Password", "Password is required");

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid Request");
        }

        [Fact]
        public async Task RequestToken_NullRequest_ShouldHandleGracefully()
        {
            // Arrange
            TokenRequest request = null;
            _controller.ModelState.AddModelError("", "Request cannot be null");

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid Request");
        }

        [Fact]
        public async Task RequestToken_ValidRequestWithSpecialCharacters_ShouldWork()
        {
            // Arrange
            var request = new TokenRequest { Username = "user@domain.com", Password = "p@ssw0rd!" };
            var expectedToken = new Token
            {
                AccessToken = "special-token",
                TokenType = "Bearer",
                ExpiresIn = 1800
            };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(true, expectedToken));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(expectedToken);
        }

        [Fact]
        public async Task RequestToken_ValidRequestWithUnicodeCharacters_ShouldWork()
        {
            // Arrange
            var request = new TokenRequest { Username = "用户", Password = "密码123" };
            var expectedToken = new Token
            {
                AccessToken = "unicode-token",
                TokenType = "Bearer",
                ExpiresIn = 7200
            };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(true, expectedToken));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(expectedToken);
        }

        [Fact]
        public async Task RequestToken_MultipleCalls_ShouldCallAuthServiceEachTime()
        {
            // Arrange
            var request1 = new TokenRequest { Username = "user1", Password = "pass1" };
            var request2 = new TokenRequest { Username = "user2", Password = "pass2" };
            var token = new Token { AccessToken = "token", TokenType = "Bearer", ExpiresIn = 3600 };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(It.IsAny<TokenRequest>()))
                .ReturnsAsync(Tuple.Create(true, token));

            // Act
            await _controller.RequestToken(request1);
            await _controller.RequestToken(request2);

            // Assert
            _mockAuthService.Verify(x => x.IsAuthenticated(It.IsAny<TokenRequest>()), Times.Exactly(2));
        }

        [Fact]
        public async Task RequestToken_LongCredentials_ShouldHandleCorrectly()
        {
            // Arrange
            var longUsername = new string('a', 1000);
            var longPassword = new string('b', 1000);
            var request = new TokenRequest { Username = longUsername, Password = longPassword };
            var token = new Token { AccessToken = "long-token", TokenType = "Bearer", ExpiresIn = 3600 };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(true, token));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockAuthService.Verify(x => x.IsAuthenticated(request), Times.Once);
        }

        [Fact]
        public async Task RequestToken_WhitespaceCredentials_ShouldTreatAsInvalid()
        {
            // Arrange
            var request = new TokenRequest { Username = "   ", Password = "   " };
            _controller.ModelState.AddModelError("Username", "Username cannot be whitespace");
            _controller.ModelState.AddModelError("Password", "Password cannot be whitespace");

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Invalid Request");
        }

        [Fact]
        public async Task RequestToken_AuthServiceReturnsFalseWithValidToken_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "wrongpass" };
            var token = new Token { AccessToken = "token", TokenType = "Bearer", ExpiresIn = 3600 };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(false, token)); // False authentication but with token

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Wrong username or password");
        }

        [Fact]
        public async Task RequestToken_AuthServiceReturnsTrueWithNullToken_ShouldReturnOk()
        {
            // Arrange
            var request = new TokenRequest { Username = "testuser", Password = "password" };

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(true, (Token)null)); // True authentication but null token

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeNull();
        }

        [Fact]
        public async Task RequestToken_ModelStateWithMultipleErrors_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new TokenRequest { Username = "", Password = "" };
            _controller.ModelState.AddModelError("Username", "Username is required");
            _controller.ModelState.AddModelError("Password", "Password is required");
            _controller.ModelState.AddModelError("", "General validation error");

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState.ErrorCount.Should().Be(3);
        }

        [Fact]
        public async Task RequestToken_ValidModelStateButEmptyRequest_ShouldCallAuthService()
        {
            // Arrange - Valid model state but empty values (simulating different validation scenarios)
            var request = new TokenRequest { Username = "", Password = "" };
            var emptyToken = new Token();

            _mockAuthService
                .Setup(x => x.IsAuthenticated(request))
                .ReturnsAsync(Tuple.Create(false, emptyToken));

            // Act
            var result = await _controller.RequestToken(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockAuthService.Verify(x => x.IsAuthenticated(request), Times.Once);
        }
    }
}
