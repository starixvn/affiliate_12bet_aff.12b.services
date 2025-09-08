using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Deposit;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Deposit
{
    public class FetchDepositInfoDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockOptionsSnapshot;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchDepositInfoDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockOptionsSnapshot = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                PullInfoCron = "0 */5 * * * *",
                AdjustmentValue = 30,
                MaxTimeRange = 7,
                SupportedPrefix = new[] { "12B", "SB", "AG" },
                DataSource = "DepositInfo"
            };

            _mockOptionsSnapshot.Setup(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
                .Returns(_fetchServiceSetting);
        }

        private FetchDepositInfoDataTask CreateTask()
        {
            return new FetchDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);
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
        public void Constructor_NullRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            // The base constructor validates null repository
            var act = () => new FetchDepositInfoDataTask(
                null!,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("thirdPartyImportRepository");
        }

        [Fact]
        public void Constructor_NullLicenseeApi_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // BaseDepositInfoDataTask constructor doesn't validate null licenseeApi
            var act = () => new FetchDepositInfoDataTask(
                _mockRepository.Object,
                null!,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullTrackerService_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            // BaseDepositInfoDataTask constructor doesn't validate null trackerService
            var act = () => new FetchDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                null!,
                _mockOptionsSnapshot.Object);

            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_NullOptionsSnapshot_ShouldThrowNullReferenceException()
        {
            // Arrange, Act & Assert
            // The constructor calls Get() on null optionsSnapshot, causing NullReferenceException
            var act = () => new FetchDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                null!);

            act.Should().Throw<NullReferenceException>();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void FetchDepositInfoDataTask_ShouldInheritFromBaseDepositInfoDataTask()
        {
            // Arrange
            var task = CreateTask();

            // Act & Assert
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void FetchDepositInfoDataTask_ShouldUseCorrectServiceConfigName()
        {
            // Arrange & Act
            var task = CreateTask();

            // Assert
            _mockOptionsSnapshot.Verify(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING), Times.Once);
        }

        [Fact]
        public void FetchDepositInfoDataTask_WithDifferentSettings_ShouldWork()
        {
            // Arrange
            var customSetting = new FetchServiceSetting
            {
                PullInfoCron = "0 0 */2 * * *",
                AdjustmentValue = 60,
                MaxTimeRange = 14,
                SupportedPrefix = new[] { "CUSTOM1", "CUSTOM2" },
                DataSource = "CustomDepositInfo"
            };

            _mockOptionsSnapshot.Setup(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
                .Returns(customSetting);

            // Act
            var task = CreateTask();

            // Assert
            task.Should().NotBeNull();
            _mockOptionsSnapshot.Verify(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING), Times.Once);
        }

        [Fact]
        public void FetchDepositInfoDataTask_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var task1 = CreateTask();
            var task2 = CreateTask();

            // Assert
            task1.Should().NotBeNull();
            task2.Should().NotBeNull();
            task1.Should().NotBeSameAs(task2);

            // Each instance should call Get method
            _mockOptionsSnapshot.Verify(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING), Times.Exactly(2));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void FetchDepositInfoDataTask_NullSettingFromOptionsSnapshot_ShouldThrowArgumentNullException()
        {
            // Arrange
            _mockOptionsSnapshot.Setup(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
                .Returns((FetchServiceSetting)null!);

            // Act & Assert
            var act = () => CreateTask();
            // The base constructor validates null settings
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("setting");
        }

        [Fact]
        public void FetchDepositInfoDataTask_EmptySettingsFromOptionsSnapshot_ShouldWork()
        {
            // Arrange
            var emptySetting = new FetchServiceSetting
            {
                PullInfoCron = null,
                AdjustmentValue = 0,
                MaxTimeRange = 0,
                SupportedPrefix = new string[0],
                DataSource = null
            };

            _mockOptionsSnapshot.Setup(x => x.Get(ServiceConfigName.PULL_DEPOSIT_INFO_SETTING))
                .Returns(emptySetting);

            // Act & Assert
            var act = () => CreateTask();
            act.Should().NotThrow();
        }

        #endregion

        #region Constants Tests

        [Fact]
        public void ServiceConfigName_PULL_DEPOSIT_INFO_SETTING_ShouldHaveCorrectValue()
        {
            // Arrange & Act & Assert
            ServiceConfigName.PULL_DEPOSIT_INFO_SETTING.Should().Be("PullDepositInfoSetting");
        }

        #endregion
    }
}
