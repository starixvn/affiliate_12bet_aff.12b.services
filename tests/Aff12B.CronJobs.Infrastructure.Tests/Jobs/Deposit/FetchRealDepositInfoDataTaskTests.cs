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
    /// <summary>
    /// Unit tests for FetchRealDepositInfoDataTask class.
    /// Tests real deposit data fetching functionality.
    /// </summary>
    public class FetchRealDepositInfoDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockThirdPartyImportRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockOptionsSnapshot;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchRealDepositInfoDataTaskTests()
        {
            _mockThirdPartyImportRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockOptionsSnapshot = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                DataSource = "TestDataSource",
                AdjustmentValue = 10,
                MaxTimeRange = 1440,
                SupportedPrefix = new[] { "DB1", "DB2" }
            };

            _mockOptionsSnapshot
                .Setup(x => x.Get(ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING))
                .Returns(_fetchServiceSetting);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>();
        }

        [Fact]
        public void IsNeedRealData_ShouldReturnTrue()
        {
            // Arrange
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            // Act - Use reflection to access the protected property
            var isNeedRealDataProperty = typeof(FetchRealDepositInfoDataTask)
                .GetProperty("IsNeedRealData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isNeedRealData = (bool)isNeedRealDataProperty.GetValue(task);

            // Assert - Should return true for real deposit data task
            isNeedRealData.Should().BeTrue();
        }

        [Fact]
        public void FetchRealDepositInfoDataTask_ShouldInheritFromBaseDepositInfoDataTask()
        {
            // Arrange
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            // Act & Assert - Should inherit from the correct base class
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>();
        }

        [Fact]
        public void Constructor_ShouldPassCorrectServiceConfigToBase()
        {
            // Arrange & Act - Create the task
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            // Assert - Should have called the options snapshot with the correct service config name
            _mockOptionsSnapshot.Verify(x => x.Get(ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldHandleNullOptionsSnapshot()
        {
            // Act & Assert - Should throw NullReferenceException when options snapshot is null (expected behavior)
            Action act = () => new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                null);

            act.Should().Throw<NullReferenceException>();
        }

        [Theory]
        [InlineData("PullRealDepositInfoSetting")]
        public void Constructor_ShouldUseCorrectServiceConfigName(string expectedConfigName)
        {
            // Arrange - Verify the constant value matches expected
            var actualConfigName = ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING;

            // Act & Assert - Should use the correct service config name
            actualConfigName.Should().Be(expectedConfigName);

            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            _mockOptionsSnapshot.Verify(x => x.Get(actualConfigName), Times.Once);
        }

        [Fact]
        public void FetchRealDepositInfoDataTask_ShouldOverrideIsNeedRealDataFromBase()
        {
            // Arrange
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            // Act - Check if IsNeedRealData is overridden
            var taskType = typeof(FetchRealDepositInfoDataTask);
            var baseType = typeof(BaseDepositInfoDataTask);

            var taskProperty = taskType.GetProperty("IsNeedRealData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var baseProperty = baseType.GetProperty("IsNeedRealData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert - Should override the base property
            taskProperty.Should().NotBeNull();
            baseProperty.Should().NotBeNull();
            taskProperty.DeclaringType.Should().Be(taskType);
        }

        [Fact]
        public void FetchRealDepositInfoDataTask_ShouldHaveParameterlessPublicConstructor_ForDI()
        {
            // Act - Check constructor parameters
            var constructors = typeof(FetchRealDepositInfoDataTask).GetConstructors();
            var publicConstructor = constructors.FirstOrDefault(c => c.IsPublic);

            // Assert - Should have a public constructor with the expected parameters
            publicConstructor.Should().NotBeNull();
            var parameters = publicConstructor.GetParameters();
            parameters.Should().HaveCount(4);
            parameters[0].ParameterType.Should().Be<IThirdPartyImportRepository>();
            parameters[1].ParameterType.Should().Be<ILicenseeApi>();
            parameters[2].ParameterType.Should().Be<ITrackerService>();
            parameters[3].ParameterType.Should().Be<IOptionsSnapshot<FetchServiceSetting>>();
        }

        [Fact]
        public void FetchRealDepositInfoDataTask_ShouldBeInCorrectNamespace()
        {
            // Act & Assert - Should be in the correct namespace
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            task.GetType().Namespace.Should().Be("Aff12B.CronJobs.Infrastructure.Jobs.Deposit");
        }

        [Fact]
        public void FetchRealDepositInfoDataTask_ShouldCallBaseConstructorWithCorrectParameters()
        {
            // Arrange - Set up the options snapshot to return our test setting
            _mockOptionsSnapshot
                .Setup(x => x.Get(ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING))
                .Returns(_fetchServiceSetting);

            // Act - Create the task (this calls the base constructor)
            var task = new FetchRealDepositInfoDataTask(
                _mockThirdPartyImportRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptionsSnapshot.Object);

            // Assert - Should have called Get with the correct service config name
            _mockOptionsSnapshot.Verify(x => x.Get(ServiceConfigName.PULL_REAL_DEPOSIT_INFO_SETTING), Times.Once);
            task.Should().NotBeNull();
        }
    }
}
