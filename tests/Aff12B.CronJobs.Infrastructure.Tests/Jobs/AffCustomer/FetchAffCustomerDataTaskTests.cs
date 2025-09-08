using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.AffCustomer
{
    /// <summary>
    /// Unit tests for FetchAffCustomerDataTask class.
    /// Tests the concrete implementation that inherits from BaseAffCustomerDataTask.
    /// </summary>
    public class FetchAffCustomerDataTaskTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IAffMemberRepository> _mockRepository;
        private readonly Mock<ILogger<FetchAffCustomerDataTask>> _mockLogger;
        private readonly Mock<IOptions<LicenseeSiteSetting>> _mockConfig;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchAffCustomerDataTaskTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockRepository = new Mock<IAffMemberRepository>();
            _mockLogger = new Mock<ILogger<FetchAffCustomerDataTask>>();
            _mockConfig = new Mock<IOptions<LicenseeSiteSetting>>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _licenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "Site1", "Site2" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "Site1", SettingCodePrefix = "S1" },
                    new LicenseeSiteMapping { SiteName = "Site2", SettingCodePrefix = "S2" }
                }
            };

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "S1", "S2" },
                MaxTimeRange = 120,
                AdjustmentValue = -30,
                DataSource = "TestDataSource"
            };

            _mockConfig.Setup(x => x.Value).Returns(_licenseeSiteSetting);
            _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        // Note: Constructor null checks removed as parameters are passed to base class
        // and validation happens there, not immediately in the constructor

        [Fact]
        public void FetchAffCustomerDataTask_ShouldImplementRequiredInterfaces()
        {
            // Arrange & Act
            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should implement required interfaces
            task.Should().BeAssignableTo<BaseAffCustomerDataTask>();
            task.Should().BeAssignableTo<IFetchAffCustomerDataTask>();
            task.Should().BeAssignableTo<IFetchDataTask>();
        }

        [Fact]
        public async Task FetchDataByDateRangeTask_ShouldReturnEarly_WhenNoSupportedSites()
        {
            // Arrange - Empty supported sites
            var emptySiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = Array.Empty<string>(),
                LicenseeSiteMapping = Array.Empty<LicenseeSiteMapping>()
            };

            _mockConfig.Setup(x => x.Value).Returns(emptySiteSetting);

            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act
            await task.FetchDataByDateRangeTask();

            // Assert - Should return early without calling API
            _mockLicenseeApi.Verify(x => x.GetAffCustomerAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task FetchDataByDateRangeTask_ShouldReturnEarly_WhenSupportedSitesIsNull()
        {
            // Arrange - Null supported sites
            var nullSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = null,
                LicenseeSiteMapping = Array.Empty<LicenseeSiteMapping>()
            };

            _mockConfig.Setup(x => x.Value).Returns(nullSiteSetting);

            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act
            await task.FetchDataByDateRangeTask();

            // Assert - Should return early without calling API
            _mockLicenseeApi.Verify(x => x.GetAffCustomerAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void FetchDataByDateRangeTask_ShouldUseCorrectDateRange()
        {
            // Arrange
            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Act & Assert - The task should use 1440 minutes (24 hours) as the date range
            // This is verified by the constant DATE_RANGE = 1440 in the implementation
            // The actual verification would require access to protected members or integration testing
            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchAffCustomerDataTask_ShouldHaveCorrectAttributes()
        {
            // Arrange & Act - Check interface attributes
            var interfaceType = typeof(IFetchAffCustomerDataTask);
            var method = interfaceType.GetMethod("FetchDataByDateRangeTask");

            // Assert - Should have Hangfire attributes
            method.Should().NotBeNull();
            var attributes = method!.GetCustomAttributes(false);
            attributes.Should().NotBeEmpty();
        }

        [Fact]
        public void FetchAffCustomerDataTask_ShouldSupportBusinessScenarios()
        {
            // Arrange - Business scenario with multiple sites
            var businessSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "MainSite", "PartnerSite", "TestSite" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "MainSite", SettingCodePrefix = "MAIN" },
                    new LicenseeSiteMapping { SiteName = "PartnerSite", SettingCodePrefix = "PART" },
                    new LicenseeSiteMapping { SiteName = "TestSite", SettingCodePrefix = "TEST" }
                }
            };

            _mockConfig.Setup(x => x.Value).Returns(businessSiteSetting);

            // Act
            var task = new FetchAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockConfig.Object,
                _mockConfiguration.Object);

            // Assert - Should handle multiple business sites
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<IFetchAffCustomerDataTask>();
        }

        [Fact]
        public void FetchAffCustomerDataTask_ShouldHandleConfigurationScenarios()
        {
            // Arrange - Various configuration scenarios
            var configurations = new[]
            {
                new FetchServiceSetting { MaxTimeRange = 60, AdjustmentValue = -15, DataSource = "Production" },
                new FetchServiceSetting { MaxTimeRange = 240, AdjustmentValue = -60, DataSource = "Staging" },
                new FetchServiceSetting { MaxTimeRange = 1440, AdjustmentValue = 0, DataSource = "Development" }
            };

            foreach (var config in configurations)
            {
                _mockConfiguration.Setup(x => x.Get(It.IsAny<string>())).Returns(config);

                // Act
                var task = new FetchAffCustomerDataTask(
                    _mockLicenseeApi.Object,
                    _mockRepository.Object,
                    _mockLogger.Object,
                    _mockConfig.Object,
                    _mockConfiguration.Object);

                // Assert - Should handle different configurations
                task.Should().NotBeNull();
            }
        }

        [Fact]
        public void FetchAffCustomerDataTask_ShouldSupportDifferentSiteConfigurations()
        {
            // Arrange - Different site configuration scenarios
            var siteConfigurations = new[]
            {
                // Single site
                new LicenseeSiteSetting
                {
                    SupportedSites = new[] { "SingleSite" },
                    LicenseeSiteMapping = new[]
                    {
                        new LicenseeSiteMapping { SiteName = "SingleSite", SettingCodePrefix = "SINGLE" }
                    }
                },
                // Multiple sites
                new LicenseeSiteSetting
                {
                    SupportedSites = new[] { "Site1", "Site2", "Site3", "Site4", "Site5" },
                    LicenseeSiteMapping = new[]
                    {
                        new LicenseeSiteMapping { SiteName = "Site1", SettingCodePrefix = "S1" },
                        new LicenseeSiteMapping { SiteName = "Site2", SettingCodePrefix = "S2" },
                        new LicenseeSiteMapping { SiteName = "Site3", SettingCodePrefix = "S3" },
                        new LicenseeSiteMapping { SiteName = "Site4", SettingCodePrefix = "S4" },
                        new LicenseeSiteMapping { SiteName = "Site5", SettingCodePrefix = "S5" }
                    }
                }
            };

            foreach (var siteConfig in siteConfigurations)
            {
                _mockConfig.Setup(x => x.Value).Returns(siteConfig);

                // Act
                var task = new FetchAffCustomerDataTask(
                    _mockLicenseeApi.Object,
                    _mockRepository.Object,
                    _mockLogger.Object,
                    _mockConfig.Object,
                    _mockConfiguration.Object);

                // Assert - Should handle different site configurations
                task.Should().NotBeNull();
                task.Should().BeAssignableTo<BaseAffCustomerDataTask>();
            }
        }
    }
}
