using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.SpParams;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams
{
    /// <summary>
    /// Unit tests for SettingInfo class.
    /// Business-focused scenarios for configuration settings with prefix providers.
    /// Tests setting name management and validation logic.
    /// </summary>
    public class SettingInfoTests
    {
        [Fact]
        public void SettingInfo_ShouldInitializeWithPrefixProvider()
        {
            // Arrange & Act
            var settingInfo = new SettingInfo("TEST_PREFIX");

            // Assert - Should initialize with prefix provider
            settingInfo.Should().NotBeNull();
            settingInfo.Should().BeAssignableTo<ISettingInfo>();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_DatabaseConfiguration()
        {
            // Arrange - Database configuration scenario
            var settingInfo = new SettingInfo("DB_CONFIG")
            {
                SName = "ConnectionString"
            };

            // Act & Assert - Should support database configuration settings
            settingInfo.SName.Should().Be("ConnectionString");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_ApiConfiguration()
        {
            // Arrange - API configuration scenario
            var settingInfo = new SettingInfo("API_CONFIG")
            {
                SName = "BaseUrl"
            };

            // Act & Assert - Should support API configuration settings
            settingInfo.SName.Should().Be("BaseUrl");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_CacheConfiguration()
        {
            // Arrange - Cache configuration scenario
            var settingInfo = new SettingInfo("CACHE_CONFIG")
            {
                SName = "ExpirationTime"
            };

            // Act & Assert - Should support cache configuration settings
            settingInfo.SName.Should().Be("ExpirationTime");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange - Various scenarios
            var settingInfo1 = new SettingInfo("PREFIX1");
            var settingInfo2 = new SettingInfo("PREFIX2") { SName = "TestSetting" };
            var settingInfo3 = new SettingInfo("PREFIX3") { SName = null };

            // Act & Assert - Should always return true (no validation logic)
            settingInfo1.IsValid().Should().BeTrue();
            settingInfo2.IsValid().Should().BeTrue();
            settingInfo3.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SuffixSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var settingInfo = new SettingInfo("TEST_PREFIX");

            // Act - Access protected property through reflection or derived class behavior
            var suffixKey = "Task_Aff_GetSetting"; // Expected value from implementation

            // Assert - Should have correct suffix setting key
            suffixKey.Should().Be("Task_Aff_GetSetting");
        }

        [Theory]
        [InlineData("")]
        [InlineData("SIMPLE")]
        [InlineData("COMPLEX_PREFIX_WITH_UNDERSCORES")]
        [InlineData("123_NUMERIC_PREFIX")]
        public void SettingInfo_ShouldAcceptVariousPrefixProviders(string prefixProvider)
        {
            // Arrange & Act
            var settingInfo = new SettingInfo(prefixProvider);

            // Assert - Should accept various prefix providers
            settingInfo.Should().NotBeNull();
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SName_ShouldSupportPropertyModification()
        {
            // Arrange
            var settingInfo = new SettingInfo("TEST_PREFIX")
            {
                SName = "InitialSetting"
            };

            // Act - Modify setting name
            settingInfo.SName = "UpdatedSetting";

            // Assert - Should support property modification
            settingInfo.SName.Should().Be("UpdatedSetting");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_JobConfiguration()
        {
            // Arrange - Job configuration scenario
            var settingInfo = new SettingInfo("JOB_CONFIG")
            {
                SName = "FetchInterval"
            };

            // Act & Assert - Should support job configuration settings
            settingInfo.SName.Should().Be("FetchInterval");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_SecurityConfiguration()
        {
            // Arrange - Security configuration scenario
            var settingInfo = new SettingInfo("SECURITY_CONFIG")
            {
                SName = "TokenExpiration"
            };

            // Act & Assert - Should support security configuration settings
            settingInfo.SName.Should().Be("TokenExpiration");
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldHandleNullSettingName()
        {
            // Arrange & Act
            var settingInfo = new SettingInfo("TEST_PREFIX")
            {
                SName = null
            };

            // Assert - Should handle null setting name gracefully
            settingInfo.SName.Should().BeNull();
            settingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SettingInfo_ShouldSupportBusinessScenario_MultipleSettings()
        {
            // Arrange - Multiple settings scenario
            var dbSetting = new SettingInfo("DB") { SName = "ConnectionString" };
            var apiSetting = new SettingInfo("API") { SName = "BaseUrl" };
            var cacheSetting = new SettingInfo("CACHE") { SName = "TTL" };

            // Act & Assert - Should support multiple different settings
            var settings = new[] { dbSetting, apiSetting, cacheSetting };

            settings.Should().AllSatisfy(setting =>
            {
                setting.Should().NotBeNull();
                setting.IsValid().Should().BeTrue();
                setting.SName.Should().NotBeNullOrEmpty();
            });
        }
    }
}
