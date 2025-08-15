using Aff12B.Common.DataDomain.Api;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Api
{
    /// <summary>
    /// Unit tests for TokenInfo - Target: 95% Coverage
    /// </summary>
    public class TokenInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var tokenInfo = new TokenInfo();

            // Assert
            tokenInfo.Should().NotBeNull();
            tokenInfo.AccessToken.Should().BeNull();
            tokenInfo.TokenType.Should().BeNull();
            tokenInfo.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test.token";
            tokenInfo.TokenType = "Bearer";
            tokenInfo.ExpiresIn = 3600;

            // Assert
            tokenInfo.AccessToken.Should().Be("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.test.token");
            tokenInfo.TokenType.Should().Be("Bearer");
            tokenInfo.ExpiresIn.Should().Be(3600);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Bearer")]
        [InlineData("bearer")]
        [InlineData("Basic")]
        [InlineData("JWT")]
        [InlineData("OAuth")]
        public void TokenType_ShouldHandleStringValues(string tokenType)
        {
            // Arrange
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.TokenType = tokenType;

            // Assert
            tokenInfo.TokenType.Should().Be(tokenType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("short.token")]
        [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
        [InlineData("very-long-token-string-that-could-be-used-in-some-authentication-systems")]
        public void AccessToken_ShouldHandleStringValues(string accessToken)
        {
            // Arrange
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.AccessToken = accessToken;

            // Assert
            tokenInfo.AccessToken.Should().Be(accessToken);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(300)]
        [InlineData(3600)]
        [InlineData(86400)]
        [InlineData(int.MaxValue)]
        public void ExpiresIn_ShouldHandleIntegerValues(int expiresIn)
        {
            // Arrange
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.ExpiresIn = expiresIn;

            // Assert
            tokenInfo.ExpiresIn.Should().Be(expiresIn);
        }

        [Fact]
        public void JsonSerialization_ShouldUseCorrectPropertyNames()
        {
            // Arrange
            var tokenInfo = new TokenInfo
            {
                AccessToken = "test-access-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            // Act
            var json = JsonSerializer.Serialize(tokenInfo);
            var deserializedTokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            json.Should().Contain("\"access_token\":");
            json.Should().Contain("\"token_type\":");
            json.Should().Contain("\"expires_in\":");

            deserializedTokenInfo.AccessToken.Should().Be("test-access-token");
            deserializedTokenInfo.TokenType.Should().Be("Bearer");
            deserializedTokenInfo.ExpiresIn.Should().Be(3600);
        }

        [Fact]
        public void JsonDeserialization_ShouldHandleSnakeCasePropertyNames()
        {
            // Arrange
            var json = @"{
                ""access_token"": ""test-token-12345"",
                ""token_type"": ""Bearer"",
                ""expires_in"": 7200
            }";

            // Act
            var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            tokenInfo.Should().NotBeNull();
            tokenInfo.AccessToken.Should().Be("test-token-12345");
            tokenInfo.TokenType.Should().Be("Bearer");
            tokenInfo.ExpiresIn.Should().Be(7200);
        }

        [Fact]
        public void JsonDeserialization_ShouldHandleMissingProperties()
        {
            // Arrange
            var json = @"{}";

            // Act
            var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            tokenInfo.Should().NotBeNull();
            tokenInfo.AccessToken.Should().BeNull();
            tokenInfo.TokenType.Should().BeNull();
            tokenInfo.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void JsonDeserialization_ShouldHandlePartialProperties()
        {
            // Arrange
            var json = @"{
                ""access_token"": ""partial-token""
            }";

            // Act
            var tokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            tokenInfo.Should().NotBeNull();
            tokenInfo.AccessToken.Should().Be("partial-token");
            tokenInfo.TokenType.Should().BeNull();
            tokenInfo.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void TokenInfo_ShouldHandleTypicalOAuthResponse()
        {
            // Arrange & Act
            var tokenInfo = new TokenInfo
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            // Assert
            tokenInfo.AccessToken.Should().StartWith("eyJ"); // JWT tokens typically start with this
            tokenInfo.TokenType.Should().Be("Bearer");
            tokenInfo.ExpiresIn.Should().Be(3600); // 1 hour in seconds
        }

        [Fact]
        public void TokenInfo_ShouldHandleRefreshTokenScenario()
        {
            // Arrange & Act
            var tokenInfo = new TokenInfo
            {
                AccessToken = "new-access-token-after-refresh",
                TokenType = "Bearer",
                ExpiresIn = 1800 // 30 minutes
            };

            // Assert
            tokenInfo.AccessToken.Should().Be("new-access-token-after-refresh");
            tokenInfo.TokenType.Should().Be("Bearer");
            tokenInfo.ExpiresIn.Should().Be(1800);
        }

        [Fact]
        public void TokenInfo_ShouldHandleShortLivedToken()
        {
            // Arrange & Act
            var tokenInfo = new TokenInfo
            {
                AccessToken = "short-lived-token",
                TokenType = "Bearer",
                ExpiresIn = 300 // 5 minutes
            };

            // Assert
            tokenInfo.ExpiresIn.Should().BeLessThan(3600); // Less than 1 hour
        }

        [Fact]
        public void TokenInfo_ShouldHandleLongLivedToken()
        {
            // Arrange & Act
            var tokenInfo = new TokenInfo
            {
                AccessToken = "long-lived-token",
                TokenType = "Bearer",
                ExpiresIn = 86400 // 24 hours
            };

            // Assert
            tokenInfo.ExpiresIn.Should().BeGreaterThan(3600); // More than 1 hour
        }

        [Fact]
        public void TokenInfo_ShouldHandleSpecialCharactersInToken()
        {
            // Arrange
            var specialToken = "token-with-special-chars-!@#$%^&*()_+-=[]{}|;:,.<>?";
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.AccessToken = specialToken;

            // Assert
            tokenInfo.AccessToken.Should().Be(specialToken);
        }

        [Fact]
        public void TokenInfo_ShouldHandleUnicodeCharactersInToken()
        {
            // Arrange
            var unicodeToken = "token-with-unicode-中文-Русский-العربية";
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.AccessToken = unicodeToken;

            // Assert
            tokenInfo.AccessToken.Should().Be(unicodeToken);
        }

        [Fact]
        public void TokenInfo_ShouldHandleVeryLongToken()
        {
            // Arrange
            var longToken = new string('A', 5000); // 5KB token
            var tokenInfo = new TokenInfo();

            // Act
            tokenInfo.AccessToken = longToken;

            // Assert
            tokenInfo.AccessToken.Should().Be(longToken);
            tokenInfo.AccessToken.Length.Should().Be(5000);
        }

        [Fact]
        public void JsonSerialization_ShouldHandleNullValues()
        {
            // Arrange
            var tokenInfo = new TokenInfo
            {
                AccessToken = null,
                TokenType = null,
                ExpiresIn = 0
            };

            // Act
            var json = JsonSerializer.Serialize(tokenInfo);
            var deserializedTokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            deserializedTokenInfo.AccessToken.Should().BeNull();
            deserializedTokenInfo.TokenType.Should().BeNull();
            deserializedTokenInfo.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void JsonSerialization_ShouldHandleEmptyStrings()
        {
            // Arrange
            var tokenInfo = new TokenInfo
            {
                AccessToken = "",
                TokenType = "",
                ExpiresIn = 0
            };

            // Act
            var json = JsonSerializer.Serialize(tokenInfo);
            var deserializedTokenInfo = JsonSerializer.Deserialize<TokenInfo>(json);

            // Assert
            deserializedTokenInfo.AccessToken.Should().Be("");
            deserializedTokenInfo.TokenType.Should().Be("");
            deserializedTokenInfo.ExpiresIn.Should().Be(0);
        }
    }
}
