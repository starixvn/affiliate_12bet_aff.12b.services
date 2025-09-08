using Aff12B.CronJobs.Infrastructure.Configurations;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Configurations
{
    /// <summary>
    /// Streamlined unit tests for TimeTrackerSetting class.
    /// Reduced from 265 lines to focus on meaningful business scenarios.
    /// Simple configuration class for time tracking with source prefixes and default time.
    /// </summary>
    public class TimeTrackerSettingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var setting = new TimeTrackerSetting();

            // Assert
            setting.Should().NotBeNull();
            setting.SourcePrefix.Should().BeNull();
            setting.DefaultTime.Should().Be(default);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var sourcePrefixes = new[] { "12B", "SB", "LC" };
            var defaultTime = DateTime.UtcNow.AddDays(-30);

            // Act
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = sourcePrefixes,
                DefaultTime = defaultTime
            };

            // Assert
            setting.SourcePrefix.Should().BeEquivalentTo(sourcePrefixes);
            setting.DefaultTime.Should().Be(defaultTime);
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportSingleSourcePrefix()
        {
            // Arrange - Single source configuration
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "12B" },
                DefaultTime = DateTime.UtcNow.AddDays(-7)
            };

            // Assert
            setting.SourcePrefix.Should().HaveCount(1);
            setting.SourcePrefix.Should().Contain("12B");
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportMultipleSourcePrefixes()
        {
            // Arrange - Multiple source configuration
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "12B", "SB", "LC", "PC", "MC" },
                DefaultTime = DateTime.UtcNow.AddMonths(-1)
            };

            // Assert
            setting.SourcePrefix.Should().HaveCount(5);
            setting.SourcePrefix.Should().Contain(new[] { "12B", "SB", "LC", "PC", "MC" });
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow.AddDays(-20));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportCasinoSourcesScenario()
        {
            // Arrange - Casino-specific sources
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "CASINO_A", "CASINO_B", "CASINO_VIP" },
                DefaultTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            // Assert - Casino sources validation
            setting.SourcePrefix.Should().HaveCount(3);
            setting.SourcePrefix.Should().AllSatisfy(prefix => prefix.Should().StartWith("CASINO"));
            setting.DefaultTime.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportSportsSourcesScenario()
        {
            // Arrange - Sports betting sources
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "SPORTS_EU", "SPORTS_US", "SPORTS_ASIA" },
                DefaultTime = DateTime.UtcNow.AddDays(-14)
            };

            // Assert - Sports sources validation
            setting.SourcePrefix.Should().HaveCount(3);
            setting.SourcePrefix.Should().AllSatisfy(prefix => prefix.Should().StartWith("SPORTS"));
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow.AddDays(-10));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportRegionalSourcesScenario()
        {
            // Arrange - Regional source configuration
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "US_EAST", "US_WEST", "EU_CENTRAL", "ASIA_PAC" },
                DefaultTime = DateTime.UtcNow.AddHours(-24)
            };

            // Assert - Regional sources validation
            setting.SourcePrefix.Should().HaveCount(4);
            setting.SourcePrefix.Should().Contain(prefix => prefix.Contains("US"));
            setting.SourcePrefix.Should().Contain(prefix => prefix.Contains("EU"));
            setting.SourcePrefix.Should().Contain(prefix => prefix.Contains("ASIA"));
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow.AddHours(-20));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportEmptySourcePrefix()
        {
            // Arrange - Empty source prefix
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new string[0],
                DefaultTime = DateTime.UtcNow
            };

            // Assert
            setting.SourcePrefix.Should().BeEmpty();
            setting.DefaultTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportCollectionOperations()
        {
            // Arrange
            var prefixes = new[] { "PREFIX_1", "PREFIX_2", "PREFIX_3", "PREFIX_4" };
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = prefixes,
                DefaultTime = DateTime.UtcNow.AddDays(-5)
            };

            // Act & Assert
            setting.SourcePrefix.Should().HaveCount(4);
            setting.SourcePrefix.Should().Contain("PREFIX_2");
            setting.SourcePrefix.Should().NotContain("PREFIX_5");
            setting.SourcePrefix.Where(p => p.StartsWith("PREFIX")).Should().HaveCount(4);
            setting.SourcePrefix.First().Should().Be("PREFIX_1");
            setting.SourcePrefix.Last().Should().Be("PREFIX_4");
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportTimeZoneScenarios()
        {
            // Arrange - Different time zone scenarios
            var utcTime = DateTime.UtcNow.AddDays(-1);
            var localTime = DateTime.Now.AddDays(-1);

            var utcSetting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "UTC_SOURCE" },
                DefaultTime = utcTime
            };

            var localSetting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "LOCAL_SOURCE" },
                DefaultTime = localTime
            };

            // Assert - Time zone handling
            utcSetting.DefaultTime.Should().Be(utcTime);
            localSetting.DefaultTime.Should().Be(localTime);
            utcSetting.DefaultTime.Should().BeBefore(DateTime.UtcNow);
            localSetting.DefaultTime.Should().BeBefore(DateTime.Now);
        }

        [Theory]
        [InlineData("12B")]
        [InlineData("SB")]
        [InlineData("LC")]
        [InlineData("PC")]
        public void TimeTrackerSetting_ShouldSupportCommonPrefixes(string prefix)
        {
            // Act
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { prefix },
                DefaultTime = DateTime.UtcNow.AddDays(-1)
            };

            // Assert
            setting.SourcePrefix.Should().Contain(prefix);
            setting.SourcePrefix.Should().HaveCount(1);
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportBusinessHoursScenario()
        {
            // Arrange - Business hours default time
            var businessHoursStart = DateTime.Today.AddHours(9); // 9 AM today
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "BUSINESS_HOURS" },
                DefaultTime = businessHoursStart
            };

            // Assert - Business hours validation
            setting.DefaultTime.Hour.Should().Be(9);
            setting.DefaultTime.Minute.Should().Be(0);
            setting.DefaultTime.Second.Should().Be(0);
            setting.SourcePrefix.Should().Contain("BUSINESS_HOURS");
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportMaintenanceWindowScenario()
        {
            // Arrange - Maintenance window configuration
            var maintenanceTime = DateTime.Today.AddHours(2); // 2 AM today
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "MAINTENANCE", "BACKUP", "SYNC" },
                DefaultTime = maintenanceTime
            };

            // Assert - Maintenance window validation
            setting.DefaultTime.Hour.Should().Be(2); // Early morning maintenance
            setting.SourcePrefix.Should().Contain(new[] { "MAINTENANCE", "BACKUP", "SYNC" });
            setting.SourcePrefix.Should().HaveCount(3);
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportHistoricalDataScenario()
        {
            // Arrange - Historical data tracking
            var historicalDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "HISTORICAL", "ARCHIVE", "LEGACY" },
                DefaultTime = historicalDate
            };

            // Assert - Historical data validation
            setting.DefaultTime.Should().Be(historicalDate);
            setting.DefaultTime.Should().BeBefore(DateTime.UtcNow.AddMonths(-6));
            setting.SourcePrefix.Should().Contain("HISTORICAL");
            setting.SourcePrefix.Should().Contain("ARCHIVE");
            setting.SourcePrefix.Should().Contain("LEGACY");
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportRealtimeDataScenario()
        {
            // Arrange - Real-time data tracking
            var realtimeDefault = DateTime.UtcNow.AddMinutes(-5); // 5 minutes ago
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "REALTIME", "LIVE", "STREAMING" },
                DefaultTime = realtimeDefault
            };

            // Assert - Real-time data validation
            setting.DefaultTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(10));
            setting.SourcePrefix.Should().Contain("REALTIME");
            setting.SourcePrefix.Should().Contain("LIVE");
            setting.SourcePrefix.Should().Contain("STREAMING");
        }

        [Fact]
        public void TimeTrackerSetting_ShouldHandleNullSourcePrefix()
        {
            // Act
            var setting = new TimeTrackerSetting
            {
                SourcePrefix = null,
                DefaultTime = DateTime.UtcNow
            };

            // Assert
            setting.SourcePrefix.Should().BeNull();
            setting.DefaultTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void TimeTrackerSetting_ShouldSupportConfigurationComparison()
        {
            // Arrange
            var setting1 = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "A", "B" },
                DefaultTime = DateTime.UtcNow.Date
            };

            var setting2 = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "A", "B" },
                DefaultTime = DateTime.UtcNow.Date
            };

            var setting3 = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "C", "D" },
                DefaultTime = DateTime.UtcNow.Date.AddDays(-1)
            };

            // Assert - Configuration comparison
            setting1.SourcePrefix.Should().BeEquivalentTo(setting2.SourcePrefix);
            setting1.DefaultTime.Should().Be(setting2.DefaultTime);
            setting1.SourcePrefix.Should().NotBeEquivalentTo(setting3.SourcePrefix);
            setting1.DefaultTime.Should().NotBe(setting3.DefaultTime);
        }
    }
}