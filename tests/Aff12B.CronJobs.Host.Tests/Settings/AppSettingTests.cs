using Aff12B.CronJobs.Host.Settings;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Host.Tests.Settings
{
    /// <summary>
    /// Unit tests for AppSetting class.
    /// Business-focused scenarios for application configuration settings.
    /// Tests property initialization, configuration scenarios, and data validation.
    /// </summary>
    public class AppSettingTests
    {
        [Fact]
        public void AppSetting_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var appSetting = new AppSetting();

            // Assert - Should have default values
            appSetting.LicenseeDataAPI.Should().BeNull();
            appSetting.IsApiDataModifiable.Should().BeFalse();
            appSetting.SupportDBPrefix.Should().BeNull();
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_ProductionConfiguration()
        {
            // Arrange - Production configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://api.production.12b.com/v1",
                IsApiDataModifiable = false,
                SupportDBPrefix = new[] { "PROD_DB1", "PROD_DB2", "PROD_DB3" }
            };

            // Act & Assert - Should support production configuration
            appSetting.LicenseeDataAPI.Should().StartWith("https://");
            appSetting.LicenseeDataAPI.Should().Contain("production");
            appSetting.IsApiDataModifiable.Should().BeFalse("because production data should not be modifiable");
            appSetting.SupportDBPrefix.Should().HaveCount(3);
            appSetting.SupportDBPrefix.Should().AllSatisfy(prefix => prefix.Should().StartWith("PROD_"));
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_DevelopmentConfiguration()
        {
            // Arrange - Development configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://api.dev.12b.com/v1",
                IsApiDataModifiable = true,
                SupportDBPrefix = new[] { "DEV_DB1", "TEST_DB1" }
            };

            // Act & Assert - Should support development configuration
            appSetting.LicenseeDataAPI.Should().Contain("dev");
            appSetting.IsApiDataModifiable.Should().BeTrue("because development data can be modified");
            appSetting.SupportDBPrefix.Should().HaveCount(2);
            appSetting.SupportDBPrefix.Should().Contain("DEV_DB1");
            appSetting.SupportDBPrefix.Should().Contain("TEST_DB1");
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_StagingConfiguration()
        {
            // Arrange - Staging configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://api.staging.12b.com/v1",
                IsApiDataModifiable = true,
                SupportDBPrefix = new[] { "STAGING_DB1", "STAGING_DB2" }
            };

            // Act & Assert - Should support staging configuration
            appSetting.LicenseeDataAPI.Should().Contain("staging");
            appSetting.IsApiDataModifiable.Should().BeTrue("because staging allows data modification for testing");
            appSetting.SupportDBPrefix.Should().HaveCount(2);
            appSetting.SupportDBPrefix.Should().AllSatisfy(prefix => prefix.Should().StartWith("STAGING_"));
        }

        [Fact]
        public void AppSetting_ShouldSupportPropertyModification()
        {
            // Arrange
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://initial-api.com",
                IsApiDataModifiable = false,
                SupportDBPrefix = new[] { "INITIAL_DB" }
            };

            // Act - Modify properties
            appSetting.LicenseeDataAPI = "https://updated-api.com";
            appSetting.IsApiDataModifiable = true;
            appSetting.SupportDBPrefix = new[] { "UPDATED_DB1", "UPDATED_DB2" };

            // Assert - Should support property modification
            appSetting.LicenseeDataAPI.Should().Be("https://updated-api.com");
            appSetting.IsApiDataModifiable.Should().BeTrue();
            appSetting.SupportDBPrefix.Should().HaveCount(2);
            appSetting.SupportDBPrefix.Should().Contain("UPDATED_DB1");
            appSetting.SupportDBPrefix.Should().Contain("UPDATED_DB2");
        }

        [Theory]
        [InlineData("https://api.local.com", true, new[] { "LOCAL_DB" })]
        [InlineData("https://api.test.com", false, new[] { "TEST_DB1", "TEST_DB2" })]
        [InlineData("https://api.uat.com", true, new[] { "UAT_DB1", "UAT_DB2", "UAT_DB3" })]
        public void AppSetting_ShouldAcceptVariousConfigurations(string apiUrl, bool isModifiable, string[] dbPrefixes)
        {
            // Arrange & Act
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = apiUrl,
                IsApiDataModifiable = isModifiable,
                SupportDBPrefix = dbPrefixes
            };

            // Assert - Should accept various configurations
            appSetting.LicenseeDataAPI.Should().Be(apiUrl);
            appSetting.IsApiDataModifiable.Should().Be(isModifiable);
            appSetting.SupportDBPrefix.Should().BeEquivalentTo(dbPrefixes);
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_MultiTenantConfiguration()
        {
            // Arrange - Multi-tenant configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://api.multi-tenant.12b.com/v1",
                IsApiDataModifiable = false,
                SupportDBPrefix = new[] { "TENANT_A", "TENANT_B", "TENANT_C", "SHARED_DB" }
            };

            // Act & Assert - Should support multi-tenant configuration
            appSetting.LicenseeDataAPI.Should().Contain("multi-tenant");
            appSetting.IsApiDataModifiable.Should().BeFalse();
            appSetting.SupportDBPrefix.Should().HaveCount(4);
            appSetting.SupportDBPrefix.Should().Contain("SHARED_DB");
            appSetting.SupportDBPrefix.Where(p => p.StartsWith("TENANT_")).Should().HaveCount(3);
        }

        [Fact]
        public void AppSetting_ShouldHandleNullValues()
        {
            // Arrange & Act
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = null,
                IsApiDataModifiable = false,
                SupportDBPrefix = null
            };

            // Assert - Should handle null values gracefully
            appSetting.LicenseeDataAPI.Should().BeNull();
            appSetting.IsApiDataModifiable.Should().BeFalse();
            appSetting.SupportDBPrefix.Should().BeNull();
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_EmptyDatabasePrefixes()
        {
            // Arrange - Empty database prefixes scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://api.12b.com/v1",
                IsApiDataModifiable = true,
                SupportDBPrefix = new string[0]
            };

            // Act & Assert - Should support empty database prefixes
            appSetting.LicenseeDataAPI.Should().NotBeNullOrEmpty();
            appSetting.IsApiDataModifiable.Should().BeTrue();
            appSetting.SupportDBPrefix.Should().NotBeNull();
            appSetting.SupportDBPrefix.Should().BeEmpty();
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_SecurityConfiguration()
        {
            // Arrange - Security-focused configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://secure-api.12b.com/v1",
                IsApiDataModifiable = false,
                SupportDBPrefix = new[] { "SECURE_DB", "AUDIT_DB" }
            };

            // Act & Assert - Should support security configuration
            appSetting.LicenseeDataAPI.Should().StartWith("https://secure");
            appSetting.IsApiDataModifiable.Should().BeFalse("because security requires read-only access");
            appSetting.SupportDBPrefix.Should().Contain("SECURE_DB");
            appSetting.SupportDBPrefix.Should().Contain("AUDIT_DB");
        }

        [Fact]
        public void AppSetting_ShouldSupportBusinessScenario_DisasterRecoveryConfiguration()
        {
            // Arrange - Disaster recovery configuration scenario
            var appSetting = new AppSetting
            {
                LicenseeDataAPI = "https://dr-api.12b.com/v1",
                IsApiDataModifiable = true,
                SupportDBPrefix = new[] { "PRIMARY_DB", "BACKUP_DB1", "BACKUP_DB2" }
            };

            // Act & Assert - Should support disaster recovery configuration
            appSetting.LicenseeDataAPI.Should().Contain("dr-");
            appSetting.IsApiDataModifiable.Should().BeTrue("because DR scenarios may require data modification");
            appSetting.SupportDBPrefix.Should().Contain("PRIMARY_DB");
            appSetting.SupportDBPrefix.Where(p => p.StartsWith("BACKUP_")).Should().HaveCount(2);
        }
    }
}
