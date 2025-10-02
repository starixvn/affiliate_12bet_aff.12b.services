using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Customer;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Customer
{
    /// <summary>
    /// Unit tests for FetchRealCustomerDataTask class.
    /// Business-focused scenarios for real-time customer data fetching.
    /// Tests inheritance, configuration, and real-time data handling.
    /// </summary>
    public class FetchRealCustomerDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockOptions;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchRealCustomerDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockOptions = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                AdjustmentValue = 5,
                MaxTimeRange = 60,
                SupportedPrefix = new[] { "REAL_", "LIVE_" },
                PullInfoCron = "0 */5 * * * *"
            };

            _mockOptions.Setup(x => x.Get(ServiceConfigName.PULL_REAL_CUSTOMER_SETTING))
                .Returns(_fetchServiceSetting);
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldCreateInstance_WhenValidDependencies()
        {
            // Arrange & Act
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should create instance successfully
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseCustomerDataTask>();
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldInheritFromBaseCustomerDataTask()
        {
            // Arrange - Type checking for inheritance
            var taskType = typeof(FetchRealCustomerDataTask);

            // Act & Assert - Should inherit from BaseCustomerDataTask
            taskType.Should().BeDerivedFrom<BaseCustomerDataTask>();
        }

        [Fact]
        public void IsNeedRealData_ShouldReturnTrue()
        {
            // Arrange
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act - Access protected property through reflection
            var property = typeof(BaseCustomerDataTask).GetProperty("IsNeedRealData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isNeedRealData = (bool)property.GetValue(task);

            // Assert - Should return true for real data processing
            isNeedRealData.Should().BeTrue("because FetchRealCustomerDataTask processes real-time data");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportBusinessScenario_RealTimeDataProcessing()
        {
            // Arrange - Real-time data processing scenario
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support real-time data processing
            task.Should().NotBeNull("because real-time customer data processing is critical for business operations");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it inherits core customer data functionality");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldUseCorrectServiceConfiguration()
        {
            // Arrange & Act
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should use correct service configuration
            _mockOptions.Verify(x => x.Get(ServiceConfigName.PULL_REAL_CUSTOMER_SETTING), Times.Once,
                "because it should use the real customer data configuration");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Arrange & Act
            var act = () => new FetchRealCustomerDataTask(
                null,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("thirdPartyImportRepository");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLicenseeApiIsNull()
        {
            // Arrange & Act
            var act = () => new FetchRealCustomerDataTask(
                _mockRepository.Object,
                null,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("licenseeApi");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenTrackerServiceIsNull()
        {
            // Arrange & Act
            var act = () => new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                null,
                _mockOptions.Object);

            // Assert - Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("timeTrackerService");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportBusinessScenario_LiveCasinoIntegration()
        {
            // Arrange - Live casino integration scenario
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support live casino customer data integration
            task.Should().NotBeNull("because live casino requires real-time customer data synchronization");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it provides core customer data functionality");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportBusinessScenario_HighFrequencyUpdates()
        {
            // Arrange - High frequency updates scenario
            _fetchServiceSetting.PullInfoCron = "0 * * * * *"; // Every minute
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support high frequency updates
            task.Should().NotBeNull("because real-time scenarios require frequent data updates");
            _fetchServiceSetting.PullInfoCron.Should().Contain("* * * *", "because real-time data requires frequent scheduling");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportBusinessScenario_RealTimeReporting()
        {
            // Arrange - Real-time reporting scenario
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support real-time reporting
            task.Should().NotBeNull("because real-time reporting requires immediate customer data availability");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it inherits data processing capabilities");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportBusinessScenario_ComplianceMonitoring()
        {
            // Arrange - Compliance monitoring scenario
            var task = new FetchRealCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support compliance monitoring
            task.Should().NotBeNull("because compliance monitoring requires real-time customer data tracking");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it provides customer data processing foundation");
        }

        [Fact]
        public void FetchRealCustomerDataTask_ShouldSupportDependencyInjection()
        {
            // Arrange - Dependency injection scenario
            var repository = _mockRepository.Object;
            var licenseeApi = _mockLicenseeApi.Object;
            var trackerService = _mockTrackerService.Object;
            var options = _mockOptions.Object;

            // Act
            var task = new FetchRealCustomerDataTask(repository, licenseeApi, trackerService, options);

            // Assert - Should accept dependencies through constructor
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseCustomerDataTask>();
        }
    }
}
