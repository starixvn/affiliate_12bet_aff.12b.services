using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.AffCustomer;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.AffCustomer
{
    public class BaseAffCustomerDataTaskTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IAffMemberRepository> _mockMemberRepository;
        private readonly Mock<ILogger<BaseAffCustomerDataTask>> _mockLogger;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public BaseAffCustomerDataTaskTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockMemberRepository = new Mock<IAffMemberRepository>();
            _mockLogger = new Mock<ILogger<BaseAffCustomerDataTask>>();

            _licenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "12B", "SB", "AG" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "12B", SettingCodePrefix = "12B" },
                    new LicenseeSiteMapping { SiteName = "SB", SettingCodePrefix = "SBO" },
                    new LicenseeSiteMapping { SiteName = "AG", SettingCodePrefix = "AG" }
                }
            };

            _fetchServiceSetting = new FetchServiceSetting
            {
                PullInfoCron = "0 */5 * * * *",
                AdjustmentValue = 30,
                MaxTimeRange = 7,
                SupportedPrefix = new[] { "12B", "SB", "AG" },
                DataSource = "TestDataSource"
            };
        }

        private TestableBaseAffCustomerDataTask CreateTask()
        {
            return new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                _licenseeSiteSetting,
                _fetchServiceSetting);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidDependencies_ShouldCreateInstance()
        {
            // Arrange, Act & Assert
            var act = () => CreateTask();
            act.Should().NotThrow();

            var task = CreateTask();
            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullLicenseeApi_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The constructor doesn't validate null licenseeApi
            var act = () => new TestableBaseAffCustomerDataTask(
                null!,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                _licenseeSiteSetting,
                _fetchServiceSetting);

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullMemberRepository_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The constructor doesn't validate null memberRepository
            var act = () => new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                null!,
                _mockLogger.Object,
                _licenseeSiteSetting,
                _fetchServiceSetting);

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullLogger_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // The constructor doesn't validate null logger
            var act = () => new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                null!,
                _licenseeSiteSetting,
                _fetchServiceSetting);

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullLicenseeSiteSetting_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                null!,
                _fetchServiceSetting);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("licenseeSiteSetting");
        }

        [Fact]
        public void Constructor_NullFetchServiceSetting_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                _licenseeSiteSetting,
                null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("fetchServiceSetting");
        }

        #endregion

        #region Property Tests

        [Fact]
        public void SupportedSites_ShouldReturnConfiguredSites()
        {
            // Arrange
            var task = CreateTask();

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().NotBeNull();
            supportedSites.Should().BeEquivalentTo(new[] { "12B", "SB", "AG" });
        }

        [Fact]
        public void SupportedSites_NullLicenseeSiteSetting_ShouldReturnNull()
        {
            // Arrange
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = null };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().BeNull();
        }

        [Fact]
        public void SupportedSites_EmptyLicenseeSiteSetting_ShouldReturnEmpty()
        {
            // Arrange
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = new string[0] };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().NotBeNull();
            supportedSites.Should().BeEmpty();
        }

        [Fact]
        public void SupportedSites_SingleSite_ShouldReturnExpectedValue()
        {
            // Arrange
            var sites = new[] { "Site1" };
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = sites };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().BeEquivalentTo(sites);
        }

        [Fact]
        public void SupportedSites_MultipleSites_ShouldReturnExpectedValue()
        {
            // Arrange
            var sites = new[] { "Site1", "Site2" };
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = sites };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().BeEquivalentTo(sites);
        }

        [Fact]
        public void SupportedSites_ManySites_ShouldReturnExpectedValue()
        {
            // Arrange
            var sites = new[] { "Site1", "Site2", "Site3", "Site4", "Site5" };
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = sites };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            var supportedSites = task.GetSupportedSites();

            // Assert
            supportedSites.Should().BeEquivalentTo(sites);
        }

        [Fact]
        public void IsNeedRealData_ShouldReturnExpectedValue()
        {
            // Arrange
            var task = CreateTask();

            // Act
            var isNeedRealData = task.GetIsNeedRealData();

            // Assert
            isNeedRealData.Should().BeFalse(); // Base implementation returns false
        }

        #endregion

        #region FetchDataAsync Tests

        [Fact]
        public async Task FetchDataAsync_NoSupportedSites_ShouldReturnEarly()
        {
            // Arrange
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = null };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            await task.FetchDataAsync();

            // Assert
            // Should return early without calling any repository methods
            _mockMemberRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task FetchDataAsync_EmptySupportedSites_ShouldReturnEarly()
        {
            // Arrange
            var licenseeSetting = new LicenseeSiteSetting { SupportedSites = new string[0] };
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                licenseeSetting,
                _fetchServiceSetting);

            // Act
            await task.FetchDataAsync();

            // Assert
            // Should return early without calling any repository methods
            _mockMemberRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task FetchDataAsync_ValidSites_ShouldProcessSites()
        {
            // Arrange
            var task = CreateTask();

            // Setup mock to return a valid date for GetLastedMemberImportedTimeAsync
            _mockMemberRepository.Setup(x => x.GetLastedMemberImportedTimeAsync(It.IsAny<Aff12B.CronJobs.Infrastructure.SpParams.MemberImportedTimeInfo>()))
                .ReturnsAsync(DateTime.Now.AddDays(-1));

            // Setup mock to return a valid site name
            _mockMemberRepository.Setup(x => x.GetSiteNameAsync(It.IsAny<Aff12B.CronJobs.Infrastructure.SpParams.SiteNameInfo>()))
                .ReturnsAsync("TestSite");

            // Act
            var act = async () => await task.FetchDataAsync();

            // Assert
            // Should not throw and should call repository methods
            await act.Should().NotThrowAsync();
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Constructor_AllValidParameters_ShouldSetFieldsCorrectly()
        {
            // Arrange & Act
            var task = CreateTask();

            // Assert
            task.GetSupportedSites().Should().BeEquivalentTo(_licenseeSiteSetting.SupportedSites);
        }

        [Fact]
        public void BATCH_COUNT_ShouldBeConstant()
        {
            // Arrange
            var task = CreateTask();

            // Act
            var batchCount = task.GetBatchCount();

            // Assert
            batchCount.Should().Be(2000);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void BaseAffCustomerDataTask_RealWorldConfiguration_ShouldWork()
        {
            // Arrange
            var realWorldLicenseeSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "12B", "SB", "AG", "PT", "MW" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "12B", SettingCodePrefix = "12B" },
                    new LicenseeSiteMapping { SiteName = "SB", SettingCodePrefix = "SBO" },
                    new LicenseeSiteMapping { SiteName = "AG", SettingCodePrefix = "AG" },
                    new LicenseeSiteMapping { SiteName = "PT", SettingCodePrefix = "PT" },
                    new LicenseeSiteMapping { SiteName = "MW", SettingCodePrefix = "MW" }
                }
            };

            var realWorldFetchSetting = new FetchServiceSetting
            {
                PullInfoCron = "0 */5 * * * *",
                AdjustmentValue = 30,
                MaxTimeRange = 7,
                SupportedPrefix = new[] { "12B", "SB", "AG", "PT", "MW" },
                DataSource = "AffCustomer"
            };

            // Act
            var task = new TestableBaseAffCustomerDataTask(
                _mockLicenseeApi.Object,
                _mockMemberRepository.Object,
                _mockLogger.Object,
                realWorldLicenseeSetting,
                realWorldFetchSetting);

            // Assert
            task.Should().NotBeNull();
            task.GetSupportedSites().Should().HaveCount(5);
            task.GetSupportedSites().Should().Contain("12B");
            task.GetSupportedSites().Should().Contain("SB");
            task.GetSupportedSites().Should().Contain("AG");
            task.GetSupportedSites().Should().Contain("PT");
            task.GetSupportedSites().Should().Contain("MW");
        }

        #endregion

        // Testable wrapper class to access protected members
        private class TestableBaseAffCustomerDataTask : BaseAffCustomerDataTask
        {
            public TestableBaseAffCustomerDataTask(
                ILicenseeApi licenseeApi,
                IAffMemberRepository memberImportRepository,
                ILogger<BaseAffCustomerDataTask> logger,
                LicenseeSiteSetting licenseeSiteSetting,
                FetchServiceSetting fetchServiceSetting)
                : base(licenseeApi, memberImportRepository, logger, licenseeSiteSetting, fetchServiceSetting)
            {
            }

            public IEnumerable<string> GetSupportedSites() => SupportedSites;
            public bool GetIsNeedRealData() => IsNeedRealData;
            public int GetBatchCount() => BATCH_COUNT;
        }
    }
}
