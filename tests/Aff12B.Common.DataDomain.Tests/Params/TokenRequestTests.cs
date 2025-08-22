using Aff12B.Common.DataDomain.Params;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Params
{
    /// <summary>
    /// Unit tests for TokenRequest class.
    /// Business-focused scenarios for API authentication token request parameters.
    /// Tests property initialization, default values, and business scenarios.
    /// </summary>
    public class TokenRequestTests
    {
        [Fact]
        public void TokenRequest_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest();

            // Assert - Should have default cache timeout
            tokenRequest.CacheTimeOut.Should().Be(20);
            tokenRequest.Username.Should().BeNull();
            tokenRequest.Password.Should().BeNull();
        }

        [Fact]
        public void TokenRequest_ShouldSupportBusinessScenario_ApiAuthentication()
        {
            // Arrange - API authentication scenario
            var tokenRequest = new TokenRequest
            {
                Username = "api_user_12b",
                Password = "secure_api_password_2024",
                CacheTimeOut = 30
            };

            // Act & Assert - Should support API authentication parameters
            tokenRequest.Username.Should().Be("api_user_12b");
            tokenRequest.Password.Should().Be("secure_api_password_2024");
            tokenRequest.CacheTimeOut.Should().Be(30);
        }

        [Fact]
        public void TokenRequest_ShouldSupportBusinessScenario_ShortLivedToken()
        {
            // Arrange - Short-lived token scenario (high security)
            var tokenRequest = new TokenRequest
            {
                Username = "high_security_user",
                Password = "complex_password_123!",
                CacheTimeOut = 5 // 5 minutes for high security
            };

            // Act & Assert - Should support short-lived tokens
            tokenRequest.CacheTimeOut.Should().Be(5);
            tokenRequest.Username.Should().NotBeNullOrEmpty();
            tokenRequest.Password.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TokenRequest_ShouldSupportBusinessScenario_LongLivedToken()
        {
            // Arrange - Long-lived token scenario (development/testing)
            var tokenRequest = new TokenRequest
            {
                Username = "dev_user",
                Password = "dev_password",
                CacheTimeOut = 120 // 2 hours for development
            };

            // Act & Assert - Should support long-lived tokens
            tokenRequest.CacheTimeOut.Should().Be(120);
            tokenRequest.Username.Should().Be("dev_user");
            tokenRequest.Password.Should().Be("dev_password");
        }

        [Theory]
        [InlineData("", "password", 30)]
        [InlineData("username", "", 30)]
        [InlineData("username", "password", 0)]
        [InlineData("username", "password", -1)]
        public void TokenRequest_ShouldAcceptVariousInputs_ForFlexibility(string username, string password, int cacheTimeout)
        {
            // Arrange & Act - Various input scenarios
            var tokenRequest = new TokenRequest
            {
                Username = username,
                Password = password,
                CacheTimeOut = cacheTimeout
            };

            // Assert - Should accept various inputs (validation happens elsewhere)
            tokenRequest.Username.Should().Be(username);
            tokenRequest.Password.Should().Be(password);
            tokenRequest.CacheTimeOut.Should().Be(cacheTimeout);
        }

        [Fact]
        public void TokenRequest_ShouldSupportPropertyModification()
        {
            // Arrange
            var tokenRequest = new TokenRequest
            {
                Username = "initial_user",
                Password = "initial_password",
                CacheTimeOut = 10
            };

            // Act - Modify properties
            tokenRequest.Username = "updated_user";
            tokenRequest.Password = "updated_password";
            tokenRequest.CacheTimeOut = 60;

            // Assert - Should support property modification
            tokenRequest.Username.Should().Be("updated_user");
            tokenRequest.Password.Should().Be("updated_password");
            tokenRequest.CacheTimeOut.Should().Be(60);
        }

        [Fact]
        public void TokenRequest_ShouldSupportBusinessScenario_ProductionConfiguration()
        {
            // Arrange - Production configuration scenario
            var tokenRequest = new TokenRequest
            {
                Username = "prod_service_account",
                Password = "prod_secure_key_2024!@#",
                CacheTimeOut = 45 // 45 minutes for production balance
            };

            // Act & Assert - Should support production configuration
            tokenRequest.Username.Should().StartWith("prod_");
            tokenRequest.Password.Should().Contain("2024");
            tokenRequest.CacheTimeOut.Should().BeInRange(30, 60, "production tokens should have reasonable cache times");
        }

        [Fact]
        public void TokenRequest_ShouldHandleNullValues()
        {
            // Arrange & Act
            var tokenRequest = new TokenRequest
            {
                Username = null,
                Password = null,
                CacheTimeOut = 20
            };

            // Assert - Should handle null values gracefully
            tokenRequest.Username.Should().BeNull();
            tokenRequest.Password.Should().BeNull();
            tokenRequest.CacheTimeOut.Should().Be(20);
        }
    }
}
