using Aff12B.Ext.API.Core.Models;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Aff12B.Ext.API.Core.Tests.Models
{
    /// <summary>
    /// Unit tests for Token model class
    /// Tests property validation, JSON serialization, edge cases, and OAuth scenarios
    /// </summary>
    public class TokenTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var token = new Token();

            // Assert
            token.AccessToken.Should().BeNull();
            token.TokenType.Should().BeNull();
            token.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var token = new Token();
            const string expectedAccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            const string expectedTokenType = "Bearer";
            const int expectedExpiresIn = 3600;

            // Act
            token.AccessToken = expectedAccessToken;
            token.TokenType = expectedTokenType;
            token.ExpiresIn = expectedExpiresIn;

            // Assert
            token.AccessToken.Should().Be(expectedAccessToken);
            token.TokenType.Should().Be(expectedTokenType);
            token.ExpiresIn.Should().Be(expectedExpiresIn);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("simple-token")]
        [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
        [InlineData("very-long-token-string-that-exceeds-typical-length-limits-but-should-still-be-handled-correctly")]
        public void AccessToken_ShouldHandleVariousTokenFormats(string accessToken)
        {
            // Arrange
            var token = new Token();

            // Act
            token.AccessToken = accessToken;

            // Assert
            token.AccessToken.Should().Be(accessToken);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Bearer")]
        [InlineData("Basic")]
        [InlineData("JWT")]
        [InlineData("OAuth")]
        [InlineData("bearer")] // lowercase
        [InlineData("BEARER")] // uppercase
        public void TokenType_ShouldHandleVariousTokenTypes(string tokenType)
        {
            // Arrange
            var token = new Token();

            // Act
            token.TokenType = tokenType;

            // Assert
            token.TokenType.Should().Be(tokenType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]        // 1 minute
        [InlineData(300)]       // 5 minutes
        [InlineData(900)]       // 15 minutes
        [InlineData(1800)]      // 30 minutes
        [InlineData(3600)]      // 1 hour
        [InlineData(7200)]      // 2 hours
        [InlineData(86400)]     // 24 hours
        [InlineData(604800)]    // 1 week
        [InlineData(-1)]        // negative (invalid but should be handled)
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void ExpiresIn_ShouldHandleVariousExpirationValues(int expiresIn)
        {
            // Arrange
            var token = new Token();

            // Act
            token.ExpiresIn = expiresIn;

            // Assert
            token.ExpiresIn.Should().Be(expiresIn);
        }

        [Fact]
        public void AccessToken_ShouldHandleNullValue()
        {
            // Arrange
            var token = new Token();

            // Act
            token.AccessToken = null;

            // Assert
            token.AccessToken.Should().BeNull();
        }

        [Fact]
        public void TokenType_ShouldHandleNullValue()
        {
            // Arrange
            var token = new Token();

            // Act
            token.TokenType = null;

            // Assert
            token.TokenType.Should().BeNull();
        }

        [Fact]
        public void Token_ShouldSupportCompleteOAuthScenario()
        {
            // Arrange & Act
            var token = new Token
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            // Assert
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.TokenType.Should().Be("Bearer");
            token.ExpiresIn.Should().BePositive();
        }

        [Fact]
        public void Token_ShouldSerializeToJsonCorrectly()
        {
            // Arrange
            var token = new Token
            {
                AccessToken = "test-access-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            // Act
            var json = JsonSerializer.Serialize(token);

            // Assert
            json.Should().Contain("\"access_token\":\"test-access-token\"");
            json.Should().Contain("\"token_type\":\"Bearer\"");
            json.Should().Contain("\"expires_in\":3600");
        }

        [Fact]
        public void Token_ShouldDeserializeFromJsonCorrectly()
        {
            // Arrange
            const string json = "{\"access_token\":\"test-token\",\"token_type\":\"Bearer\",\"expires_in\":7200}";

            // Act
            var token = JsonSerializer.Deserialize<Token>(json);

            // Assert
            token.Should().NotBeNull();
            token!.AccessToken.Should().Be("test-token");
            token.TokenType.Should().Be("Bearer");
            token.ExpiresIn.Should().Be(7200);
        }

        [Fact]
        public void Token_ShouldHandleJsonSerializationWithNullValues()
        {
            // Arrange
            var token = new Token
            {
                AccessToken = null,
                TokenType = null,
                ExpiresIn = 0
            };

            // Act
            var json = JsonSerializer.Serialize(token);
            var deserializedToken = JsonSerializer.Deserialize<Token>(json);

            // Assert
            deserializedToken.Should().NotBeNull();
            deserializedToken!.AccessToken.Should().BeNull();
            deserializedToken.TokenType.Should().BeNull();
            deserializedToken.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void Token_ShouldHandleJsonDeserializationWithMissingProperties()
        {
            // Arrange
            const string json = "{\"access_token\":\"partial-token\"}";

            // Act
            var token = JsonSerializer.Deserialize<Token>(json);

            // Assert
            token.Should().NotBeNull();
            token!.AccessToken.Should().Be("partial-token");
            token.TokenType.Should().BeNull();
            token.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void Token_ShouldHandleJsonPropertyNameMapping()
        {
            // Arrange
            var token = new Token
            {
                AccessToken = "mapped-token",
                TokenType = "Bearer",
                ExpiresIn = 1800
            };

            // Act
            var json = JsonSerializer.Serialize(token);

            // Assert - Should use JsonPropertyName attributes
            json.Should().Contain("access_token"); // not AccessToken
            json.Should().Contain("token_type");   // not TokenType
            json.Should().Contain("expires_in");   // not ExpiresIn
        }

        [Fact]
        public void AccessToken_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var token = new Token();
            const string unicodeToken = "令牌_José_García_🔑";

            // Act
            token.AccessToken = unicodeToken;

            // Assert
            token.AccessToken.Should().Be(unicodeToken);
        }

        [Fact]
        public void TokenType_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var token = new Token();
            const string unicodeTokenType = "类型_José_García_🏷️";

            // Act
            token.TokenType = unicodeTokenType;

            // Assert
            token.TokenType.Should().Be(unicodeTokenType);
        }

        [Fact]
        public void AccessToken_ShouldHandleLargeTokenString()
        {
            // Arrange
            var token = new Token();
            var largeToken = new string('A', 10000); // 10KB token

            // Act
            token.AccessToken = largeToken;

            // Assert
            token.AccessToken.Should().Be(largeToken);
            token.AccessToken.Length.Should().Be(10000);
        }

        [Fact]
        public void Token_ShouldHandleSpecialCharactersInTokens()
        {
            // Arrange
            var token = new Token();
            const string specialCharsToken = "token!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            token.AccessToken = specialCharsToken;

            // Assert
            token.AccessToken.Should().Be(specialCharsToken);
        }

        [Theory]
        [InlineData("Bearer")]
        [InlineData("Basic")]
        [InlineData("JWT")]
        [InlineData("MAC")]
        [InlineData("Custom")]
        public void TokenType_ShouldSupportVariousAuthenticationSchemes(string tokenType)
        {
            // Arrange
            var token = new Token();

            // Act
            token.TokenType = tokenType;

            // Assert
            token.TokenType.Should().Be(tokenType);
            // Note: Description is for documentation purposes
        }

        [Fact]
        public void Token_ShouldHandleShortLivedTokenScenario()
        {
            // Arrange & Act
            var token = new Token
            {
                AccessToken = "short-lived-token",
                TokenType = "Bearer",
                ExpiresIn = 60 // 1 minute
            };

            // Assert
            token.ExpiresIn.Should().BeLessThan(300); // Less than 5 minutes
            token.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public void Token_ShouldHandleLongLivedTokenScenario()
        {
            // Arrange & Act
            var token = new Token
            {
                AccessToken = "long-lived-token",
                TokenType = "Bearer",
                ExpiresIn = 604800 // 1 week
            };

            // Assert
            token.ExpiresIn.Should().BeGreaterThan(86400); // Greater than 24 hours
            token.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public void Token_ShouldHandleRefreshTokenScenario()
        {
            // Arrange & Act - Simulating a refresh token response
            var token = new Token
            {
                AccessToken = "new-refreshed-access-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            };

            // Assert
            token.AccessToken.Should().NotBeNullOrEmpty();
            token.TokenType.Should().Be("Bearer");
            token.ExpiresIn.Should().BePositive();
        }

        [Fact]
        public void Token_ShouldHandleJsonSerializationRoundTrip()
        {
            // Arrange
            var originalToken = new Token
            {
                AccessToken = "roundtrip-test-token",
                TokenType = "Bearer",
                ExpiresIn = 2400
            };

            // Act
            var json = JsonSerializer.Serialize(originalToken);
            var deserializedToken = JsonSerializer.Deserialize<Token>(json);

            // Assert
            deserializedToken.Should().NotBeNull();
            deserializedToken!.AccessToken.Should().Be(originalToken.AccessToken);
            deserializedToken.TokenType.Should().Be(originalToken.TokenType);
            deserializedToken.ExpiresIn.Should().Be(originalToken.ExpiresIn);
        }

        [Fact]
        public void Token_ShouldHandleEmptyJsonObject()
        {
            // Arrange
            const string emptyJson = "{}";

            // Act
            var token = JsonSerializer.Deserialize<Token>(emptyJson);

            // Assert
            token.Should().NotBeNull();
            token!.AccessToken.Should().BeNull();
            token.TokenType.Should().BeNull();
            token.ExpiresIn.Should().Be(0);
        }

        [Fact]
        public void Token_ShouldPreserveCaseInTokenType()
        {
            // Arrange
            var token = new Token();

            // Act & Assert - Uppercase
            token.TokenType = "BEARER";
            token.TokenType.Should().Be("BEARER");

            // Act & Assert - Lowercase
            token.TokenType = "bearer";
            token.TokenType.Should().Be("bearer");

            // Act & Assert - Mixed case
            token.TokenType = "Bearer";
            token.TokenType.Should().Be("Bearer");
        }

        [Fact]
        public void ExpiresIn_ShouldHandleZeroExpiration()
        {
            // Arrange & Act
            var token = new Token
            {
                AccessToken = "immediate-expiry-token",
                TokenType = "Bearer",
                ExpiresIn = 0
            };

            // Assert
            token.ExpiresIn.Should().Be(0);
            token.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Token_ShouldHandleMinimalValidScenario()
        {
            // Arrange & Act
            var token = new Token
            {
                AccessToken = "minimal-token"
                // TokenType and ExpiresIn left as default
            };

            // Assert
            token.AccessToken.Should().Be("minimal-token");
            token.TokenType.Should().BeNull();
            token.ExpiresIn.Should().Be(0);
        }
    }
}
