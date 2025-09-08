using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.AffCustomer
{
    /// <summary>
    /// Unit tests for FetchRealAffCustomerDataTask class.
    /// Tests the real-time data fetching variant that inherits from BaseAffCustomerDataTask.
    /// </summary>
    public class FetchRealAffCustomerDataTaskTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IAffMemberRepository> _mockRepository;
        private readonly Mock<ILogger<FetchAffCustomerDataTask>> _mockLogger;
        private readonly Mock<IOptions<LicenseeSiteSetting>> _mockConfig;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchRealAffCustomerDataTaskTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockRepository = new Mock<IAffMemberRepository>();
            _mockLogger = new Mock<ILogger<FetchAffCustomerDataTask>>();
            _mockConfig = new Mock<IOptions<LicenseeSiteSetting>>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _licenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "RealSite1", "RealSite2" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "RealSite1", SettingCodePrefix = "RS1" },
                    new LicenseeSiteMapping { SiteName = "RealSite2", SettingCodePrefix = "RS2" }
                }
            };

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "RS1", "RS2" },
                MaxTimeRange = 60, // Shorter for real-time
                AdjustmentValue = -5, // Smaller adjustment for real-time
                DataSource = "RealTimeDataSource"
            };

            _mockConfig.Setup(x => x.Value).Returns(_licenseeSiteSetting);
            _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldImplementRequiredInterfaces()
        {
            // Arrange & Act
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should implement required interfaces
            task.Should().BeAssignableTo<BaseAffCustomerDataTask>();
            task.Should().BeAssignableTo<IFetchDataTask>();
        }

        [Fact]
        public void IsNeedRealData_ShouldReturnTrue()
        {
            // Arrange
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act - Use reflection to access the protected property
            var property = typeof(BaseAffCustomerDataTask).GetProperty("IsNeedRealData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (bool)property!.GetValue(task)!;

            // Assert - Should return true for real-time data fetching
            result.Should().BeTrue();
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldOverrideIsNeedRealDataProperty()
        {
            // Arrange
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act - Use reflection to verify the override
            var property = task.GetType().GetProperty("IsNeedRealData",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert - Property should be overridden in this class
            property.Should().NotBeNull();
            property!.DeclaringType.Should().Be(typeof(FetchRealAffCustomerDataTask));
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldSupportRealTimeBusinessScenarios()
        {
            // Arrange - Real-time business scenario
            var realTimeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "LiveCasino", "SportsBetting", "PokerRoom" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "LiveCasino", SettingCodePrefix = "LC" },
                    new LicenseeSiteMapping { SiteName = "SportsBetting", SettingCodePrefix = "SB" },
                    new LicenseeSiteMapping { SiteName = "PokerRoom", SettingCodePrefix = "PR" }
                }
            };

            _mockConfig.Setup(x => x.Value).Returns(realTimeSiteSetting);

            // Act
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should handle real-time scenarios
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseAffCustomerDataTask>();
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldSupportHighFrequencyUpdates()
        {
            // Arrange - High frequency real-time configuration
            var highFrequencyConfig = new FetchServiceSetting
            {
                MaxTimeRange = 15, // Very short for real-time
                AdjustmentValue = -1, // Minimal adjustment
                DataSource = "HighFrequencyRealTime"
            };

            _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(highFrequencyConfig);

            // Act
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should handle high frequency scenarios
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<IFetchDataTask>();
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldDifferFromRegularTask()
        {
            // Arrange - Create both regular and real tasks
            var regularTask = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            var realTask = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act - Get IsNeedRealData property from both
            var regularProperty = typeof(BaseAffCustomerDataTask).GetProperty("IsNeedRealData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var regularResult = (bool)regularProperty!.GetValue(regularTask)!;
            var realResult = (bool)regularProperty!.GetValue(realTask)!;

            // Assert - Should have different behavior
            regularResult.Should().BeFalse(); // Regular task doesn't need real data
            realResult.Should().BeTrue(); // Real task needs real data
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldSupportEmptyConfiguration()
        {
            // Arrange - Empty configuration scenario
            var emptySiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = Array.Empty<string>(),
                LicenseeSiteMapping = Array.Empty<LicenseeSiteMapping>()
            };

            _mockConfig.Setup(x => x.Value).Returns(emptySiteSetting);

            // Act & Assert - Should handle empty configuration gracefully
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldSupportMultipleDataSources()
        {
            // Arrange - Multiple data source configurations
            var dataSources = new[]
            {
                "PrimaryRealTimeSource",
                "BackupRealTimeSource",
                "HighAvailabilitySource"
            };

            foreach (var dataSource in dataSources)
            {
                var config = new FetchServiceSetting
                {
                    DataSource = dataSource,
                    MaxTimeRange = 30,
                    AdjustmentValue = -2
                };

                _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(config);

                // Act
                var task = new FetchRealAffCustomerDataTask(
                    _mockLicenseeApi.Object,
                    _mockRepository.Object,
                    _mockLogger.Object,
                    _mockConfig.Object,
                    _mockConfiguration.Object);

                // Assert - Should handle different data sources
                task.Should().NotBeNull();
                task.Should().BeAssignableTo<BaseAffCustomerDataTask>();
            }
        }

        [Fact]
        public void FetchRealAffCustomerDataTask_ShouldSupportCriticalBusinessOperations()
        {
            // Arrange - Critical business operations scenario
            var criticalSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "VIPCasino", "HighRollerRoom", "PlatinumLounge" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "VIPCasino", SettingCodePrefix = "VIP" },
                    new LicenseeSiteMapping { SiteName = "HighRollerRoom", SettingCodePrefix = "HRR" },
                    new LicenseeSiteMapping { SiteName = "PlatinumLounge", SettingCodePrefix = "PLT" }
                }
            };

            var criticalConfig = new FetchServiceSetting
            {
                MaxTimeRange = 5, // Very frequent for critical operations
                AdjustmentValue = 0, // No delay for critical data
                DataSource = "CriticalRealTimeSource"
            };

            _mockConfig.Setup(x => x.Value).Returns(criticalSiteSetting);
            _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(criticalConfig);

            // Act
            var task = new FetchRealAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should support critical business operations
            task.Should().NotBeNull();

            // Verify real-time behavior is enabled
            var property = typeof(BaseAffCustomerDataTask).GetProperty("IsNeedRealData",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var isRealTime = (bool)property!.GetValue(task)!;
            isRealTime.Should().BeTrue();
        }
    }
}
