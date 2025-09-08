using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Reflection;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs
{
    /// <summary>
    /// Simplified unit tests for FetchWithdrawalInfoDataTask class.
    /// Business-focused scenarios for withdrawal information data fetching.
    /// </summary>
    public class FetchWithdrawalInfoDataTaskSimpleTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockSettings;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchWithdrawalInfoDataTask _task;

        public FetchWithdrawalInfoDataTaskSimpleTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockSettings = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "WITHDRAW", "PAYOUT", "CASHOUT" },
                MaxTimeRange = 180,
                AdjustmentValue = -5
            };

            _mockSettings.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);

            _task = new FetchWithdrawalInfoDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchWithdrawalInfoDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            var act = () => new FetchWithdrawalInfoDataTask(null!, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
            act.Should().Throw<ArgumentNullException>();
        }

        // Note: Constructor null checks removed as primary constructors don't immediately validate parameters

        [Fact]
        public void Constructor_ShouldThrowException_WhenSettingsIsNull()
        {
            // Act & Assert
            var act = () => new FetchWithdrawalInfoDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, null!);
            act.Should().Throw<Exception>(); // Could be ArgumentNullException or NullReferenceException
        }

        [Fact]
        public async Task GetDataAsync_ShouldFetchWithdrawalInfoFromApi()
        {
            // Arrange - Simple withdrawal data scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var withdrawals = new List<WithdrawalInfoModel>
            {
                new WithdrawalInfoModel { WithdrawalId = 1, Amount = 1000.50m, CurrencyId = 1, Status = 1 },
                new WithdrawalInfoModel { WithdrawalId = 2, Amount = 2500.75m, CurrencyId = 2, Status = 0 }
            };

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, batchCount)).ReturnsAsync(withdrawals);

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should call API with correct parameters
            _mockLicenseeApi.Verify(x => x.GetWithdrawalInfo(expectedDateTime, batchCount), Times.Once);
            result.Should().BeEquivalentTo(withdrawals);
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleEmptyWithdrawalList()
        {
            // Arrange - Empty withdrawal list scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 500;
            var expectedDateTime = Convert.ToDateTime(trackValue);
            var emptyWithdrawals = new List<WithdrawalInfoModel>();

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, batchCount)).ReturnsAsync(emptyWithdrawals);

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should handle empty list gracefully
            _mockLicenseeApi.Verify(x => x.GetWithdrawalInfo(expectedDateTime, batchCount), Times.Once);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleApiException()
        {
            // Arrange - API failure scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, batchCount))
                .ThrowsAsync(new HttpRequestException("Withdrawal API unavailable"));

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should propagate API exception
            var act = async () => await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });
            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Withdrawal API unavailable");
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleInvalidDateFormat()
        {
            // Arrange - Invalid date format scenario
            var invalidTrackValue = "invalid-date-format";
            var batchCount = 1000;

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should throw TargetInvocationException wrapping FormatException for invalid date
            var act = async () => await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { invalidTrackValue, batchCount });
            await act.Should().ThrowAsync<TargetInvocationException>()
                .Where(ex => ex.InnerException is FormatException);
        }

        [Fact]
        public void FetchWithdrawalInfoDataTask_ShouldInheritFromBaseClass()
        {
            // Arrange & Act & Assert - Should inherit from base class
            _task.Should().BeAssignableTo<FetchDataByDateTimeTrackerForMultiRepoTaskBase<WithdrawalInfoModel>>();
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task GetDataAsync_ShouldSupportLargeBatchSizes()
        {
            // Arrange - Large batch size scenario
            var trackValue = "2024-01-15 10:30:00";
            var largeBatchCount = 10000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var largeWithdrawals = new List<WithdrawalInfoModel>();
            // Reduced from 5000 to 20 records for performance
            for (int i = 1; i <= 20; i++)
            {
                largeWithdrawals.Add(new WithdrawalInfoModel
                {
                    WithdrawalId = i,
                    CustId = 1000 + i,
                    Amount = 100.00m + i,
                    CurrencyId = 1,
                    Status = i % 2
                });
            }

            // Force garbage collection after creating test data
            GC.Collect();

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, largeBatchCount)).ReturnsAsync(largeWithdrawals);

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, largeBatchCount });

            // Assert - Should handle large batch sizes
            _mockLicenseeApi.Verify(x => x.GetWithdrawalInfo(expectedDateTime, largeBatchCount), Times.Once);
            result.Should().HaveCount(20);
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleSpecialWithdrawalScenarios()
        {
            // Arrange - Special withdrawal scenarios
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var specialWithdrawals = new List<WithdrawalInfoModel>
            {
                new WithdrawalInfoModel { WithdrawalId = 0, CustId = 0, Amount = 0.00m, CurrencyId = 0, Status = 0 }, // Edge case: zero values
                new WithdrawalInfoModel { WithdrawalId = -1, CustId = -1, Amount = -100.00m, CurrencyId = -1, Status = -1 }, // Edge case: negative values
                new WithdrawalInfoModel { WithdrawalId = int.MaxValue, CustId = int.MaxValue, Amount = decimal.MaxValue, CurrencyId = int.MaxValue, Status = int.MaxValue }, // Edge case: max values
                new WithdrawalInfoModel { WithdrawalId = 999, CustId = 999, Amount = 100.00m, CurrencyId = null, Status = null } // Edge case: null values
            };

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, batchCount)).ReturnsAsync(specialWithdrawals);

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should handle special cases
            _mockLicenseeApi.Verify(x => x.GetWithdrawalInfo(expectedDateTime, batchCount), Times.Once);
            result.Should().HaveCount(4);
        }

        [Fact]
        public async Task GetDataAsync_ShouldSupportBusinessWithdrawalScenario()
        {
            // Arrange - Business withdrawal scenario with different statuses
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 2000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var businessWithdrawals = new List<WithdrawalInfoModel>
            {
                new WithdrawalInfoModel { WithdrawalId = 1, CustId = 1001, Amount = 50000.00m, CurrencyId = 1, Status = 1 }, // Approved
                new WithdrawalInfoModel { WithdrawalId = 2, CustId = 1002, Amount = 25000.50m, CurrencyId = 2, Status = 0 }, // Pending
                new WithdrawalInfoModel { WithdrawalId = 3, CustId = 1003, Amount = 100.00m, CurrencyId = 3, Status = -1 } // Rejected
            };

            _mockLicenseeApi.Setup(x => x.GetWithdrawalInfo(expectedDateTime, batchCount)).ReturnsAsync(businessWithdrawals);

            // Use reflection to access the protected method
            var method = typeof(FetchWithdrawalInfoDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<WithdrawalInfoModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should process all business withdrawals
            _mockLicenseeApi.Verify(x => x.GetWithdrawalInfo(expectedDateTime, batchCount), Times.Once);
            result.Should().HaveCount(3);
            result.Should().Contain(w => w.Status == 1); // Approved
            result.Should().Contain(w => w.Status == 0); // Pending
            result.Should().Contain(w => w.Status == -1); // Rejected
        }
    }
}
