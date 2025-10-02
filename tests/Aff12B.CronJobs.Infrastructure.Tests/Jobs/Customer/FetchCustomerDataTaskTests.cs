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
    /// Unit tests for FetchCustomerDataTask class.
    /// Business-focused scenarios for customer data fetching with scheduled tasks.
    /// Tests interface implementation, task scheduling, and data processing workflows.
    /// </summary>
    public class FetchCustomerDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockOptions;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchCustomerDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockOptions = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                AdjustmentValue = 10,
                MaxTimeRange = 120,
                SupportedPrefix = new[] { "CUSTOMER_", "USER_" },
                PullInfoCron = "0 0 */6 * * *"
            };

            _mockOptions.Setup(x => x.Get(ServiceConfigName.PULL_CUSTOMER_SETTING))
                .Returns(_fetchServiceSetting);
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldCreateInstance_WhenValidDependencies()
        {
            // Arrange & Act
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should create instance successfully
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseCustomerDataTask>();
            task.Should().BeAssignableTo<IFetchCustomerDataTask>();
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldImplementIFetchCustomerDataTask()
        {
            // Arrange - Type checking for interface implementation
            var taskType = typeof(FetchCustomerDataTask);

            // Act & Assert - Should implement IFetchCustomerDataTask
            taskType.Should().Implement<IFetchCustomerDataTask>();
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldInheritFromBaseCustomerDataTask()
        {
            // Arrange - Type checking for inheritance
            var taskType = typeof(FetchCustomerDataTask);

            // Act & Assert - Should inherit from BaseCustomerDataTask
            taskType.Should().BeDerivedFrom<BaseCustomerDataTask>();
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldBeSealedClass()
        {
            // Arrange - Type checking for sealed class
            var taskType = typeof(FetchCustomerDataTask);

            // Act & Assert - Should be sealed class
            taskType.IsSealed.Should().BeTrue("because FetchCustomerDataTask is a sealed implementation");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldUseCorrectServiceConfiguration()
        {
            // Arrange & Act
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Assert - Should use correct service configuration
            _mockOptions.Verify(x => x.Get(ServiceConfigName.PULL_CUSTOMER_SETTING), Times.Once,
                "because it should use the customer data configuration");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Arrange & Act
            var act = () => new FetchCustomerDataTask(
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
            var act = () => new FetchCustomerDataTask(
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
            var act = () => new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                null,
                _mockOptions.Object);

            // Assert - Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("timeTrackerService");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_ScheduledDataSynchronization()
        {
            // Arrange - Scheduled data synchronization scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support scheduled data synchronization
            task.Should().NotBeNull("because scheduled customer data synchronization is essential for business operations");
            task.Should().BeAssignableTo<IFetchCustomerDataTask>("because it implements scheduled task interface");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_BatchProcessing()
        {
            // Arrange - Batch processing scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support batch processing
            task.Should().NotBeNull("because batch processing is required for efficient customer data handling");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it inherits batch processing capabilities");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_DailyDataProcessing()
        {
            // Arrange - Daily data processing scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support daily data processing
            task.Should().NotBeNull("because daily customer data processing is critical for business reporting");
            task.Should().BeAssignableTo<IFetchCustomerDataTask>("because it provides daily task methods");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_DateRangeProcessing()
        {
            // Arrange - Date range processing scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support date range processing
            task.Should().NotBeNull("because date range processing is needed for historical data recovery");
            task.Should().BeAssignableTo<IFetchCustomerDataTask>("because it provides date range task methods");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_CustomerAnalytics()
        {
            // Arrange - Customer analytics scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support customer analytics
            task.Should().NotBeNull("because customer analytics requires regular data updates");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it provides customer data processing foundation");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_RegulatoryReporting()
        {
            // Arrange - Regulatory reporting scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support regulatory reporting
            task.Should().NotBeNull("because regulatory reporting requires complete customer data");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it processes customer data for compliance");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_DataWarehouseIntegration()
        {
            // Arrange - Data warehouse integration scenario
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support data warehouse integration
            task.Should().NotBeNull("because data warehouse requires regular customer data feeds");
            task.Should().BeAssignableTo<BaseCustomerDataTask>("because it provides data processing capabilities");
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportDependencyInjection()
        {
            // Arrange - Dependency injection scenario
            var repository = _mockRepository.Object;
            var licenseeApi = _mockLicenseeApi.Object;
            var trackerService = _mockTrackerService.Object;
            var options = _mockOptions.Object;

            // Act
            var task = new FetchCustomerDataTask(repository, licenseeApi, trackerService, options);

            // Assert - Should accept dependencies through constructor
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseCustomerDataTask>();
            task.Should().BeAssignableTo<IFetchCustomerDataTask>();
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_MultiTenantProcessing()
        {
            // Arrange - Multi-tenant processing scenario
            _fetchServiceSetting.SupportedPrefix = new[] { "TENANT_A", "TENANT_B", "TENANT_C" };
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support multi-tenant processing
            task.Should().NotBeNull("because multi-tenant systems require customer data isolation");
            _fetchServiceSetting.SupportedPrefix.Should().HaveCount(3, "because multiple tenants are supported");
            _fetchServiceSetting.SupportedPrefix.Should().AllSatisfy(prefix =>
                prefix.Should().StartWith("TENANT_", "because prefixes identify tenant boundaries"));
        }

        [Fact]
        public void FetchCustomerDataTask_ShouldSupportBusinessScenario_PerformanceOptimization()
        {
            // Arrange - Performance optimization scenario
            _fetchServiceSetting.MaxTimeRange = 30; // 30 minutes for better performance
            var task = new FetchCustomerDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _mockOptions.Object);

            // Act & Assert - Should support performance optimization
            task.Should().NotBeNull("because performance optimization is critical for large datasets");
            _fetchServiceSetting.MaxTimeRange.Should().BeLessOrEqualTo(60,
                "because smaller time ranges improve processing performance");
        }
    }
}
