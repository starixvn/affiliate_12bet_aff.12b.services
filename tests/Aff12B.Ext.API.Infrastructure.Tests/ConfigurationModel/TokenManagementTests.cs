using Aff12B.Ext.API.Infastructure.ConfigurationModel;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.ConfigurationModel
{
    public class TokenManagementTests
    {
        #region Property Tests

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var tokenManagement = new TokenManagement();
            const string expectedSecret = "test-secret-key";
            const string expectedIssuer = "test-issuer";
            const string expectedAudience = "test-audience";
            const int expectedAccessExpiration = 30;
            const int expectedRefreshExpiration = 60;

            // Act
            tokenManagement.Secret = expectedSecret;
            tokenManagement.Issuer = expectedIssuer;
            tokenManagement.Audience = expectedAudience;
            tokenManagement.AccessExpiration = expectedAccessExpiration;
            tokenManagement.RefreshExpiration = expectedRefreshExpiration;

            // Assert
            tokenManagement.Secret.Should().Be(expectedSecret);
            tokenManagement.Issuer.Should().Be(expectedIssuer);
            tokenManagement.Audience.Should().Be(expectedAudience);
            tokenManagement.AccessExpiration.Should().Be(expectedAccessExpiration);
            tokenManagement.RefreshExpiration.Should().Be(expectedRefreshExpiration);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(30)]
        [InlineData(60)]
        [InlineData(120)]
        [InlineData(1440)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void AccessExpiration_ShouldHandleVariousValues(int expirationValue)
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act
            tokenManagement.AccessExpiration = expirationValue;

            // Assert
            tokenManagement.AccessExpiration.Should().Be(expirationValue);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(60)]
        [InlineData(120)]
        [InlineData(1440)]
        [InlineData(10080)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void RefreshExpiration_ShouldHandleVariousValues(int expirationValue)
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act
            tokenManagement.RefreshExpiration = expirationValue;

            // Assert
            tokenManagement.RefreshExpiration.Should().Be(expirationValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("short")]
        [InlineData("very-long-secret-key-that-is-suitable-for-hmac-sha256-algorithm")]
        [InlineData("special-chars-!@#$%^&*()")]
        [InlineData("0426a7e37f6525c2d1c5913c42ba7dfe3d01618f5750b9495f6cdb75d16fbf79")]
        public void Secret_ShouldHandleVariousValues(string secretValue)
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act
            tokenManagement.Secret = secretValue;

            // Assert
            tokenManagement.Secret.Should().Be(secretValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12B_API")]
        [InlineData("TestIssuer")]
        [InlineData("https://localhost:5001")]
        [InlineData("special-issuer-!@#")]
        public void Issuer_ShouldHandleVariousValues(string issuerValue)
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act
            tokenManagement.Issuer = issuerValue;

            // Assert
            tokenManagement.Issuer.Should().Be(issuerValue);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12B_Services")]
        [InlineData("TestAudience")]
        [InlineData("https://localhost:5001")]
        [InlineData("multiple,audiences,separated")]
        public void Audience_ShouldHandleVariousValues(string audienceValue)
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act
            tokenManagement.Audience = audienceValue;

            // Assert
            tokenManagement.Audience.Should().Be(audienceValue);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void TokenManagement_DefaultValues_ShouldBeNullOrZero()
        {
            // Arrange & Act
            var tokenManagement = new TokenManagement();

            // Assert
            tokenManagement.Secret.Should().BeNull();
            tokenManagement.Issuer.Should().BeNull();
            tokenManagement.Audience.Should().BeNull();
            tokenManagement.AccessExpiration.Should().Be(0);
            tokenManagement.RefreshExpiration.Should().Be(0);
        }

        [Fact]
        public void TokenManagement_RealWorldConfiguration_ShouldWork()
        {
            // Arrange & Act
            var tokenManagement = new TokenManagement
            {
                Secret = "0426a7e37f6525c2d1c5913c42ba7dfe3d01618f5750b9495f6cdb75d16fbf79",
                Issuer = "12B_API",
                Audience = "12B_Services",
                AccessExpiration = 30,
                RefreshExpiration = 60
            };

            // Assert
            tokenManagement.Secret.Should().HaveLength(64);
            tokenManagement.Issuer.Should().Be("12B_API");
            tokenManagement.Audience.Should().Be("12B_Services");
            tokenManagement.AccessExpiration.Should().Be(30);
            tokenManagement.RefreshExpiration.Should().Be(60);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void TokenManagement_AllPropertiesSet_ShouldMaintainValues()
        {
            // Arrange
            var tokenManagement = new TokenManagement();

            // Act - Set all properties
            tokenManagement.Secret = "test-secret";
            tokenManagement.Issuer = "test-issuer";
            tokenManagement.Audience = "test-audience";
            tokenManagement.AccessExpiration = 15;
            tokenManagement.RefreshExpiration = 30;

            // Modify one property
            tokenManagement.AccessExpiration = 45;

            // Assert - All other properties should remain unchanged
            tokenManagement.Secret.Should().Be("test-secret");
            tokenManagement.Issuer.Should().Be("test-issuer");
            tokenManagement.Audience.Should().Be("test-audience");
            tokenManagement.AccessExpiration.Should().Be(45);
            tokenManagement.RefreshExpiration.Should().Be(30);
        }

        #endregion
    }
}
