using Aff12B.CronJobs.Infrastructure.Configurations;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Configurations
{
    /// <summary>
    /// Streamlined unit tests for FetchServiceSetting configuration class.
    /// Reduced from 296 lines to focus on meaningful business scenarios.
    /// </summary>
    public class FetchServiceSettingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var setting = new FetchServiceSetting();

            // Assert
            setting.PullInfoCron.Should().BeNull();
            setting.AdjustmentValue.Should().Be(0);
            setting.MaxTimeRange.Should().Be(0);
            setting.SupportedPrefix.Should().BeNull();
            setting.DataSource.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var setting = new FetchServiceSetting();
            const string expectedPullInfoCron = "0 */5 * * * *";
            const int expectedAdjustmentValue = 30;
            const int expectedMaxTimeRange = 7;
            var expectedSupportedPrefix = new[] { "12B", "SB", "AG" };
            const string expectedDataSource = "ProductionDB";

            // Act
            setting.PullInfoCron = expectedPullInfoCron;
            setting.AdjustmentValue = expectedAdjustmentValue;
            setting.MaxTimeRange = expectedMaxTimeRange;
            setting.SupportedPrefix = expectedSupportedPrefix;
            setting.DataSource = expectedDataSource;

            // Assert
            setting.PullInfoCron.Should().Be(expectedPullInfoCron);
            setting.AdjustmentValue.Should().Be(expectedAdjustmentValue);
            setting.MaxTimeRange.Should().Be(expectedMaxTimeRange);
            setting.SupportedPrefix.Should().BeEquivalentTo(expectedSupportedPrefix);
            setting.DataSource.Should().Be(expectedDataSource);
        }

        [Fact]
        public void SupportedPrefix_ShouldHandleCollectionOperations()
        {
            // Arrange
            var setting = new FetchServiceSetting();

            // Act & Assert - Null collection
            setting.SupportedPrefix = null;
            setting.SupportedPrefix.Should().BeNull();

            // Act & Assert - Empty collection
            setting.SupportedPrefix = new string[0];
            setting.SupportedPrefix.Should().BeEmpty();

            // Act & Assert - Multiple items
            var multiplePrefix = new[] { "12B", "SB", "AG", "PT", "MW" };
            setting.SupportedPrefix = multiplePrefix;
            setting.SupportedPrefix.Should().HaveCount(5);
            setting.SupportedPrefix.Should().ContainInOrder("12B", "SB", "AG", "PT", "MW");
        }

        [Theory]
        [InlineData("0 */5 * * * *", 30, 7, "ProductionDB")]
        [InlineData("0 0 */6 * * *", 15, 1, "StagingDB")]
        [InlineData(null, 0, 0, null)]
        [InlineData("", -30, -7, "")]
        public void RealWorldConfigurations_ShouldWork(string cronExpression, int adjustment, int maxRange, string dataSource)
        {
            // Arrange & Act
            var setting = new FetchServiceSetting
            {
                PullInfoCron = cronExpression,
                AdjustmentValue = adjustment,
                MaxTimeRange = maxRange,
                SupportedPrefix = new[] { "12B", "SB" },
                DataSource = dataSource
            };

            // Assert
            setting.PullInfoCron.Should().Be(cronExpression);
            setting.AdjustmentValue.Should().Be(adjustment);
            setting.MaxTimeRange.Should().Be(maxRange);
            setting.SupportedPrefix.Should().HaveCount(2);
            setting.DataSource.Should().Be(dataSource);
        }

        [Fact]
        public void PropertyModification_ShouldNotAffectOtherProperties()
        {
            // Arrange
            var setting = new FetchServiceSetting
            {
                PullInfoCron = "0 0 * * * *",
                AdjustmentValue = 15,
                MaxTimeRange = 30,
                SupportedPrefix = new[] { "TEST" },
                DataSource = "TestDB"
            };

            // Act - Modify one property
            setting.AdjustmentValue = 45;

            // Assert - Other properties remain unchanged
            setting.PullInfoCron.Should().Be("0 0 * * * *");
            setting.AdjustmentValue.Should().Be(45);
            setting.MaxTimeRange.Should().Be(30);
            setting.SupportedPrefix.Should().ContainSingle().Which.Should().Be("TEST");
            setting.DataSource.Should().Be("TestDB");
        }
    }
}
