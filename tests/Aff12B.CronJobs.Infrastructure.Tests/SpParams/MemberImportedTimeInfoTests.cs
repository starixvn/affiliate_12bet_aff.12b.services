using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.SpParams;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams
{
    /// <summary>
    /// Unit tests for MemberImportedTimeInfo class.
    /// Business-focused scenarios for member import time tracking settings.
    /// Tests setting name override and inheritance behavior.
    /// </summary>
    public class MemberImportedTimeInfoTests
    {
        [Fact]
        public void MemberImportedTimeInfo_ShouldInitializeWithPrefixAndSettingName()
        {
            // Arrange & Act
            var memberTimeInfo = new MemberImportedTimeInfo("MEMBER_PREFIX", "LastImportTime");

            // Assert - Should initialize with provided values
            memberTimeInfo.Should().NotBeNull();
            memberTimeInfo.SName.Should().Be("LastImportTime");
            memberTimeInfo.Should().BeAssignableTo<SettingInfo>();
            memberTimeInfo.Should().BeAssignableTo<ISettingInfo>();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldSupportBusinessScenario_MemberImportTracking()
        {
            // Arrange - Member import tracking scenario
            var memberTimeInfo = new MemberImportedTimeInfo("MEMBER_IMPORT", "LastSuccessfulImport");

            // Act & Assert - Should support member import time tracking
            memberTimeInfo.SName.Should().Be("LastSuccessfulImport");
            memberTimeInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldSupportBusinessScenario_MultiSiteImport()
        {
            // Arrange - Multi-site member import scenario
            var site1TimeInfo = new MemberImportedTimeInfo("SITE1_MEMBER", "ImportTimestamp");
            var site2TimeInfo = new MemberImportedTimeInfo("SITE2_MEMBER", "ImportTimestamp");

            // Act & Assert - Should support multiple site configurations
            site1TimeInfo.SName.Should().Be("ImportTimestamp");
            site2TimeInfo.SName.Should().Be("ImportTimestamp");

            site1TimeInfo.IsValid().Should().BeTrue();
            site2TimeInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SName_ShouldOverrideBaseProperty()
        {
            // Arrange - Test property override behavior
            var memberTimeInfo = new MemberImportedTimeInfo("TEST_PREFIX", "CustomSettingName");

            // Act & Assert - Should override base SName property
            memberTimeInfo.SName.Should().Be("CustomSettingName");
            memberTimeInfo.SName.Should().NotBeNull();
        }

        [Theory]
        [InlineData("", "EmptyPrefix")]
        [InlineData("SIMPLE", "SimpleSetting")]
        [InlineData("COMPLEX_PREFIX_123", "ComplexSetting_456")]
        public void MemberImportedTimeInfo_ShouldAcceptVariousInputs(string prefixProvider, string settingName)
        {
            // Arrange & Act
            var memberTimeInfo = new MemberImportedTimeInfo(prefixProvider, settingName);

            // Assert - Should accept various input combinations
            memberTimeInfo.SName.Should().Be(settingName);
            memberTimeInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldSupportBusinessScenario_DailyImportSchedule()
        {
            // Arrange - Daily import schedule scenario
            var dailyImportInfo = new MemberImportedTimeInfo("DAILY_IMPORT", "LastDailyImport");

            // Act & Assert - Should support daily import scheduling
            dailyImportInfo.SName.Should().Be("LastDailyImport");
            dailyImportInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldSupportBusinessScenario_IncrementalImport()
        {
            // Arrange - Incremental import scenario
            var incrementalInfo = new MemberImportedTimeInfo("INCREMENTAL", "LastIncrementalSync");

            // Act & Assert - Should support incremental import tracking
            incrementalInfo.SName.Should().Be("LastIncrementalSync");
            incrementalInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldInheritFromSettingInfo()
        {
            // Arrange - Type checking for inheritance
            var memberTimeInfo = new MemberImportedTimeInfo("TEST", "TestSetting");

            // Act & Assert - Should inherit from SettingInfo
            memberTimeInfo.Should().BeAssignableTo<SettingInfo>();
            memberTimeInfo.Should().BeAssignableTo<ISettingInfo>();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldHandleNullOrEmptySettingName()
        {
            // Arrange & Act
            var nullSettingInfo = new MemberImportedTimeInfo("PREFIX", null);
            var emptySettingInfo = new MemberImportedTimeInfo("PREFIX", "");

            // Assert - Should handle null/empty setting names
            nullSettingInfo.SName.Should().BeNull();
            emptySettingInfo.SName.Should().Be("");

            nullSettingInfo.IsValid().Should().BeTrue();
            emptySettingInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MemberImportedTimeInfo_ShouldSupportBusinessScenario_ErrorRecovery()
        {
            // Arrange - Error recovery scenario
            var recoveryInfo = new MemberImportedTimeInfo("ERROR_RECOVERY", "LastFailedImportTime");

            // Act & Assert - Should support error recovery tracking
            recoveryInfo.SName.Should().Be("LastFailedImportTime");
            recoveryInfo.IsValid().Should().BeTrue();
        }
    }
}
