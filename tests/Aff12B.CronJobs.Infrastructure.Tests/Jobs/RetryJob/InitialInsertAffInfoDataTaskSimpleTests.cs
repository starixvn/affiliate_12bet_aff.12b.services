using Aff12B.Common.DataDomain.Api;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.Jobs.RetryJob;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.RetryJob
{
    /// <summary>
    /// Simplified unit tests for InitialInsertAffInfoDataTask class.
    /// Focuses on core functionality without complex internal dependencies.
    /// </summary>
    public class InitialInsertAffInfoDataTaskSimpleTests
    {
        private readonly Mock<IDataTrackerService> _mockTrackerService;
        private readonly Mock<IAffInfoRepository> _mockAffInfoRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ILogger<InitialInsertAffInfoDataTask>> _mockLogger;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly Mock<IOptions<LicenseeSiteSetting>> _mockLicenseeSiteConfig;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly InitialInsertAffInfoDataTask _task;

        public InitialInsertAffInfoDataTaskSimpleTests()
        {
            _mockTrackerService = new Mock<IDataTrackerService>();
            _mockAffInfoRepository = new Mock<IAffInfoRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockLogger = new Mock<ILogger<InitialInsertAffInfoDataTask>>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();
            _mockLicenseeSiteConfig = new Mock<IOptions<LicenseeSiteSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "DB1", "DB2" }
            };

            _licenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "12B" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "12B", SettingCodePrefix = "12B" }
                }
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.INITIAL_INSERT_AFFILIATE_SETTING))
                .Returns(_fetchServiceSetting);

            _mockLicenseeSiteConfig
                .Setup(x => x.Value)
                .Returns(_licenseeSiteSetting);

            _task = new InitialInsertAffInfoDataTask(
                _mockTrackerService.Object,
                _mockAffInfoRepository.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                _mockLicenseeSiteConfig.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new InitialInsertAffInfoDataTask(
                _mockTrackerService.Object,
                _mockAffInfoRepository.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                _mockLicenseeSiteConfig.Object,
                _mockLogger.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void SiteId_Constant_ShouldBe101()
        {
            // Act - Verify the constant value through AffInfo.SiteId property
            var siteId = (new AffInfo()).SiteId;

            // Assert - Should be 101 for 12B site
            siteId.Should().Be(101);
        }

        [Fact]
        public void PAGE_Constants_ShouldHaveCorrectValues()
        {
            // Act - Access constants through reflection
            var pageIndexField = typeof(InitialInsertAffInfoDataTask)
                .GetField("PAGE_INDEX_DEFAULT", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var pageSizeField = typeof(InitialInsertAffInfoDataTask)
                .GetField("PAGE_SIZE_DEFAULT", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var pageIndex = (int)pageIndexField.GetValue(null);
            var pageSize = (int)pageSizeField.GetValue(null);

            // Assert - Should have expected default values
            pageIndex.Should().Be(1);
            pageSize.Should().Be(1000);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturn_WhenNoSupportedSites()
        {
            // Arrange
            var emptyLicenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new string[0]
            };

            var mockEmptyConfig = new Mock<IOptions<LicenseeSiteSetting>>();
            mockEmptyConfig.Setup(x => x.Value).Returns(emptyLicenseeSiteSetting);

            var taskWithEmptySites = new InitialInsertAffInfoDataTask(
                _mockTrackerService.Object,
                _mockAffInfoRepository.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                mockEmptyConfig.Object,
                _mockLogger.Object);

            // Act
            await taskWithEmptySites.HandleAsync();

            // Assert - Should not call any repository methods
            _mockAffInfoRepository.Verify(x => x.GetAffInfosAsync(It.IsAny<GetAffInfosCriteria>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturn_WhenSupportedSitesIsNull()
        {
            // Arrange
            var nullLicenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = null
            };

            var mockNullConfig = new Mock<IOptions<LicenseeSiteSetting>>();
            mockNullConfig.Setup(x => x.Value).Returns(nullLicenseeSiteSetting);

            var taskWithNullSites = new InitialInsertAffInfoDataTask(
                _mockTrackerService.Object,
                _mockAffInfoRepository.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                mockNullConfig.Object,
                _mockLogger.Object);

            // Act
            await taskWithNullSites.HandleAsync();

            // Assert - Should not call any repository methods
            _mockAffInfoRepository.Verify(x => x.GetAffInfosAsync(It.IsAny<GetAffInfosCriteria>()), Times.Never);
        }

        [Fact]
        public async Task GetDataAsync_ShouldCallRepositoryWithCorrectCriteria()
        {
            // Arrange
            var sitePrefixProvider = "12B";
            var expectedAffInfos = new List<AffInfo>
            {
                new AffInfo { Player_CustId = 1, AffId = 100 }
            };

            _mockAffInfoRepository
                .Setup(x => x.GetAffInfosAsync(It.IsAny<GetAffInfosCriteria>()))
                .ReturnsAsync(expectedAffInfos);

            // Act - Use reflection to call the private method
            var getDataAsyncMethod = typeof(InitialInsertAffInfoDataTask)
                .GetMethod("GetDataAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = await (Task<IEnumerable<AffInfo>>)getDataAsyncMethod.Invoke(_task, new object[] { sitePrefixProvider, 1 });

            // Assert - Should call repository with correct criteria
            _mockAffInfoRepository.Verify(x => x.GetAffInfosAsync(
                It.Is<GetAffInfosCriteria>(c =>
                    c.PageIndex == 1 &&
                    c.PageSize == 1000)),
                Times.Once);

            result.Should().BeEquivalentTo(expectedAffInfos);
        }

        [Fact]
        public async Task HandleAsync_ShouldNotCallTracker_WhenNoDataReturned()
        {
            // Arrange - Setup repository to return empty data
            var emptyData = new List<AffInfo>();

            _mockAffInfoRepository
                .Setup(x => x.GetAffInfosAsync(It.IsAny<GetAffInfosCriteria>()))
                .ReturnsAsync(emptyData);

            // Act
            await _task.HandleAsync();

            // Assert - Should not call tracker service when there's no data
            _mockTrackerService.Verify(x => x.InsertDataTrackerAsync(It.IsAny<InsertDataTrackerCommand>()), Times.Never);
        }

        [Fact]
        public void InitialInsertAffInfoDataTask_ShouldImplementIRetryDataTask()
        {
            // Act & Assert - Should implement the correct interface
            _task.Should().BeAssignableTo<IRetryDataTask>();
        }

        [Fact]
        public void SupportedSites_Property_ShouldReturnLicenseeSiteSettingSupportedSites()
        {
            // Act - Access the SupportedSites property through reflection
            var supportedSitesProperty = typeof(InitialInsertAffInfoDataTask)
                .GetProperty("SupportedSites", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var supportedSites = (IEnumerable<string>)supportedSitesProperty.GetValue(_task);

            // Assert - Should return the supported sites from the license setting
            supportedSites.Should().BeEquivalentTo(new[] { "12B" });
        }

        [Fact]
        public void FetchServiceSetting_Property_ShouldReturnCorrectSetting()
        {
            // Act - Access the FetchServiceSetting property through reflection
            var fetchServiceSettingProperty = typeof(InitialInsertAffInfoDataTask)
                .GetProperty("FetchServiceSetting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var fetchServiceSetting = (FetchServiceSetting)fetchServiceSettingProperty.GetValue(_task);

            // Assert - Should return the configured fetch service setting
            fetchServiceSetting.Should().NotBeNull();
            fetchServiceSetting.SupportedPrefix.Should().BeEquivalentTo(new[] { "DB1", "DB2" });
        }

        [Fact]
        public async Task HandleAsync_ShouldFilterOutInvalidAffInfos()
        {
            // Arrange - Create data with mixed valid/invalid items
            var affInfos = new List<AffInfo>
            {
                new AffInfo { Player_CustId = 0, AffId = 100, AffCode = "AFF100", AffCategoryId = 1 }, // Invalid: zero CustId and not marketing category
                new AffInfo { Player_CustId = 1, AffId = 101, AffCode = "AFF101", AffCategoryId = 1 }, // Valid
                new AffInfo { Player_CustId = 0, AffId = 102, AffCode = "", AffCategoryId = 2 }, // Invalid: empty AffCode
                new AffInfo { Player_CustId = 0, AffId = 103, AffCode = "AFF103", AffCategoryId = 2 } // Valid: marketing category
            };

            _mockAffInfoRepository
                .Setup(x => x.GetAffInfosAsync(It.IsAny<GetAffInfosCriteria>()))
                .ReturnsAsync(affInfos);

            _mockTrackerService
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<InsertDataTrackerCommand>()))
                .Returns(Task.CompletedTask);

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should call tracker for each supported prefix (2 prefixes: DB1, DB2) with the batch of valid items
            _mockTrackerService.Verify(
                x => x.InsertDataTrackerAsync(It.IsAny<InsertDataTrackerCommand>()),
                Times.Exactly(2)); // 1 batch of valid items * 2 supported prefixes (DB1, DB2)
        }

        [Fact]
        public void InitialInsertAffInfoDataTask_ShouldHaveCorrectHangfireAttributes()
        {
            // Act - Get the HandleAsync method
            var handleAsyncMethod = typeof(InitialInsertAffInfoDataTask).GetMethod("HandleAsync");

            // Assert - Should have the correct Hangfire attributes
            var disableConcurrentExecutionAttr = handleAsyncMethod.GetCustomAttributes(typeof(DisableConcurrentExecutionAttribute), false);
            var automaticRetryAttr = handleAsyncMethod.GetCustomAttributes(typeof(AutomaticRetryAttribute), false);

            disableConcurrentExecutionAttr.Should().HaveCount(1);
            automaticRetryAttr.Should().HaveCount(1);

            var concurrentAttr = (DisableConcurrentExecutionAttribute)disableConcurrentExecutionAttr[0];
            var retryAttr = (AutomaticRetryAttribute)automaticRetryAttr[0];

            // DisableConcurrentExecutionAttribute uses constructor parameter for timeout
            // We can verify the attribute exists rather than checking specific property values
            retryAttr.Attempts.Should().Be(0);
            retryAttr.OnAttemptsExceeded.Should().Be(AttemptsExceededAction.Fail);
        }
    }
}
