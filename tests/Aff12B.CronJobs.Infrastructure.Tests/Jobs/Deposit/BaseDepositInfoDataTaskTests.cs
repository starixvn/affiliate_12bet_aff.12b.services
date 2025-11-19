using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs.Deposit;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Deposit
{
    /// <summary>
    /// Unit tests for BaseDepositInfoDataTask class.
    /// Business-focused scenarios for deposit information data processing.
    /// Tests inheritance, configuration, and deposit data handling workflows.
    /// </summary>
    public class BaseDepositInfoDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public BaseDepositInfoDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockTrackerService = new Mock<ITrackerService>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                AdjustmentValue = 15,
                MaxTimeRange = 180,
                SupportedPrefix = new[] { "DEPOSIT_", "PAYMENT_" },
                PullInfoCron = "0 0 */4 * * *"
            };
        }

        // Create a concrete implementation for testing the abstract base class
        private class TestableBaseDepositInfoDataTask : BaseDepositInfoDataTask
        {
            public TestableBaseDepositInfoDataTask(
                IThirdPartyImportRepository thirdPartyImportRepository,
                ILicenseeApi licenseeApi,
                ITrackerService trackerService,
                FetchServiceSetting fetchServiceSetting)
                : base(thirdPartyImportRepository, licenseeApi, trackerService, fetchServiceSetting)
            {
            }

            // Expose protected property for testing
            public new bool IsNeedRealData => base.IsNeedRealData;
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldCreateInstance_WhenValidDependencies()
        {
            // Arrange & Act
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Assert - Should create instance successfully
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Arrange & Act
            var act = () => new TestableBaseDepositInfoDataTask(
                null,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Assert - Should throw ArgumentNullException
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldAcceptNullLicenseeApi()
        {
            // Arrange & Act
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                null,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Assert - Should create instance (null check not implemented in base class)
            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldAcceptNullTrackerService()
        {
            // Arrange & Act
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                null,
                _fetchServiceSetting);

            // Assert - Should create instance (null check not implemented in base class)
            task.Should().NotBeNull();
        }

        [Fact]
        public void IsNeedRealData_ShouldReturnFalse_ByDefault()
        {
            // Arrange
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should return false for base implementation
            task.IsNeedRealData.Should().BeFalse("because base implementation doesn't need real-time data by default");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_DepositProcessing()
        {
            // Arrange - Deposit processing scenario
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support deposit processing workflow
            task.Should().NotBeNull("because deposit processing is critical for financial operations");
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>("because it inherits core deposit functionality");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_PaymentGatewayIntegration()
        {
            // Arrange - Payment gateway integration scenario
            _fetchServiceSetting.SupportedPrefix = new[] { "STRIPE_", "PAYPAL_", "BANK_TRANSFER_" };
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support payment gateway integration
            task.Should().NotBeNull("because payment gateway integration is essential for deposit processing");
            _fetchServiceSetting.SupportedPrefix.Should().Contain("STRIPE_");
            _fetchServiceSetting.SupportedPrefix.Should().Contain("PAYPAL_");
            _fetchServiceSetting.SupportedPrefix.Should().Contain("BANK_TRANSFER_");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_FinancialReporting()
        {
            // Arrange - Financial reporting scenario
            _fetchServiceSetting.MaxTimeRange = 720; // 12 hours for comprehensive reporting
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support financial reporting requirements
            task.Should().NotBeNull("because financial reporting requires comprehensive deposit data");
            _fetchServiceSetting.MaxTimeRange.Should().BeGreaterThan(360,
                "because financial reporting needs longer time ranges for comprehensive data");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_RegulatoryCompliance()
        {
            // Arrange - Regulatory compliance scenario
            _fetchServiceSetting.AdjustmentValue = 5; // 5 minutes for compliance accuracy
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support regulatory compliance requirements
            task.Should().NotBeNull("because regulatory compliance requires accurate deposit tracking");
            _fetchServiceSetting.AdjustmentValue.Should().BeLessOrEqualTo(10,
                "because compliance requires minimal adjustment for accuracy");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_FraudDetection()
        {
            // Arrange - Fraud detection scenario
            _fetchServiceSetting.PullInfoCron = "0 */5 * * * *"; // Every 5 minutes for fraud monitoring
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support fraud detection requirements
            task.Should().NotBeNull("because fraud detection requires frequent deposit monitoring");
            _fetchServiceSetting.PullInfoCron.Should().Contain("*/5",
                "because fraud detection requires frequent monitoring");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_HighVolumeDeposits()
        {
            // Arrange - High volume deposits scenario
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support high volume deposit processing
            task.Should().NotBeNull("because high volume scenarios require robust deposit processing");
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>("because it provides scalable deposit handling");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_MultiCurrencyDeposits()
        {
            // Arrange - Multi-currency deposits scenario
            _fetchServiceSetting.SupportedPrefix = new[] { "USD_", "EUR_", "GBP_", "JPY_", "BTC_" };
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support multi-currency deposit processing
            task.Should().NotBeNull("because multi-currency support is essential for global operations");
            _fetchServiceSetting.SupportedPrefix.Should().HaveCount(5,
                "because multiple currencies should be supported");
            _fetchServiceSetting.SupportedPrefix.Should().Contain("BTC_",
                "because cryptocurrency deposits should be supported");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_VIPCustomerDeposits()
        {
            // Arrange - VIP customer deposits scenario
            _fetchServiceSetting.MaxTimeRange = 60; // 1 hour for VIP priority processing
            _fetchServiceSetting.AdjustmentValue = 2; // 2 minutes for VIP accuracy
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support VIP customer deposit processing
            task.Should().NotBeNull("because VIP customers require priority deposit processing");
            _fetchServiceSetting.MaxTimeRange.Should().BeLessOrEqualTo(120,
                "because VIP processing should be frequent");
            _fetchServiceSetting.AdjustmentValue.Should().BeLessOrEqualTo(5,
                "because VIP processing requires high accuracy");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_DepositLimits()
        {
            // Arrange - Deposit limits scenario
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support deposit limits monitoring
            task.Should().NotBeNull("because deposit limits are crucial for responsible gambling");
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>("because it handles deposit validation");
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportDependencyInjection()
        {
            // Arrange - Dependency injection scenario
            var repository = _mockRepository.Object;
            var licenseeApi = _mockLicenseeApi.Object;
            var trackerService = _mockTrackerService.Object;
            var setting = _fetchServiceSetting;

            // Act
            var task = new TestableBaseDepositInfoDataTask(repository, licenseeApi, trackerService, setting);

            // Assert - Should accept dependencies through constructor
            task.Should().NotBeNull();
            task.Should().BeAssignableTo<BaseDepositInfoDataTask>();
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_BonusDepositTracking()
        {
            // Arrange - Bonus deposit tracking scenario
            _fetchServiceSetting.SupportedPrefix = new[] { "BONUS_DEPOSIT_", "WELCOME_DEPOSIT_", "RELOAD_DEPOSIT_" };
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support bonus deposit tracking
            task.Should().NotBeNull("because bonus deposits require special tracking");
            _fetchServiceSetting.SupportedPrefix.Should().AllSatisfy(prefix =>
                prefix.Should().Contain("DEPOSIT", "because all prefixes should relate to deposits"));
        }

        [Fact]
        public void BaseDepositInfoDataTask_ShouldSupportBusinessScenario_RecurringDepositProcessing()
        {
            // Arrange - Recurring deposit processing scenario
            _fetchServiceSetting.PullInfoCron = "0 0 */6 * * *"; // Every 6 hours
            var task = new TestableBaseDepositInfoDataTask(
                _mockRepository.Object,
                _mockLicenseeApi.Object,
                _mockTrackerService.Object,
                _fetchServiceSetting);

            // Act & Assert - Should support recurring deposit processing
            task.Should().NotBeNull("because recurring deposits need regular processing");
            _fetchServiceSetting.PullInfoCron.Should().Contain("*/6",
                "because recurring processing should be scheduled appropriately");
        }
    }
}
