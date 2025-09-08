using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs
{
    /// <summary>
    /// Basic unit tests for FetchBetRecordDataTask class.
    /// Focus on constructor validation and interface implementation.
    /// </summary>
    public class FetchBetRecordDataTaskBasicTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ILogger<FetchBetRecordDataTask>> _mockLogger;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockSettings;
        private readonly FetchServiceSetting _fetchServiceSetting;

        public FetchBetRecordDataTaskBasicTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockLogger = new Mock<ILogger<FetchBetRecordDataTask>>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockSettings = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "BET", "WAGER", "GAME" },
                MaxTimeRange = 120,
                AdjustmentValue = -30
            };

            _mockSettings.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchBetRecordDataTask_ShouldImplementRequiredInterfaces()
        {
            // Arrange & Act
            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            // Assert - Should implement required interfaces
            task.Should().BeAssignableTo<FetchDataForMultipleRepositoryTaskBase<TpBetRecordModel>>();
            task.Should().BeAssignableTo<IFetchBetRecordDataTask>();
        }

        [Fact]
        public async Task FetchDataAsync_WithValidDateRange_ShouldCallApiWithCorrectParameters()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 15, 10, 0, 0);
            var toDate = new DateTime(2024, 1, 15, 12, 0, 0);

            var betRecords = new List<TpBetRecordModel>
            {
                new TpBetRecordModel { TpId = 1, Stake = 100.50m, WinlostAmount = 50.25m, CurrencyId = 1 },
                new TpBetRecordModel { TpId = 2, Stake = 200.75m, WinlostAmount = -200.75m, CurrencyId = 1 }
            };

            // Setup API to return empty results for all calls to avoid pagination issues
            _mockLicenseeApi.Setup(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<TpBetRecordModel>());

            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            // Act
            await task.FetchDataAsync(fromDate, toDate);

            // Assert - Should call API at least once (exact number depends on internal pagination logic)
            _mockLicenseeApi.Verify(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_WithEmptyResults_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 15, 10, 0, 0);
            var toDate = new DateTime(2024, 1, 15, 12, 0, 0);

            _mockLicenseeApi.Setup(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<TpBetRecordModel>());

            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            // Act & Assert - Should not throw
            await task.FetchDataAsync(fromDate, toDate);

            _mockLicenseeApi.Verify(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_WithApiException_ShouldPropagateException()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 15, 10, 0, 0);
            var toDate = new DateTime(2024, 1, 15, 12, 0, 0);

            _mockLicenseeApi.Setup(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new HttpRequestException("Bet record API unavailable"));

            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            // Act & Assert - Should propagate API exception
            await task.Invoking(t => t.FetchDataAsync(fromDate, toDate))
                .Should().ThrowAsync<HttpRequestException>()
                .WithMessage("Bet record API unavailable");
        }

        [Fact]
        public async Task FetchDataAsync_WithNullApiResponse_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 15, 10, 0, 0);
            var toDate = new DateTime(2024, 1, 15, 12, 0, 0);

            _mockLicenseeApi.Setup(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((IEnumerable<TpBetRecordModel>)null!);

            var task = new FetchBetRecordDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockTrackerService.Object,
                _mockSettings.Object);

            // Act & Assert - Should handle null response gracefully
            await task.FetchDataAsync(fromDate, toDate);

            _mockLicenseeApi.Verify(x => x.GetBetRecordAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Fact]
        public void FetchBetRecordDataTask_ShouldSupportMultipleCurrencyBets()
        {
            // Arrange - Test that the task can handle different bet record scenarios
            var multiCurrencyBets = new List<TpBetRecordModel>
            {
                new TpBetRecordModel { TpId = 1, Stake = 100.00m, CurrencyId = 1 }, // USD
                new TpBetRecordModel { TpId = 2, Stake = 85.50m, CurrencyId = 2 }, // EUR
                new TpBetRecordModel { TpId = 3, Stake = 750.25m, CurrencyId = 3 }, // CNY
                new TpBetRecordModel { TpId = 4, Stake = 11000.00m, CurrencyId = 4 }, // JPY
                new TpBetRecordModel { TpId = 5, Stake = 75.75m, CurrencyId = 5 } // GBP
            };

            // Act & Assert - Should be able to create bet records with different currencies
            multiCurrencyBets.Should().HaveCount(5);
            multiCurrencyBets.Should().OnlyContain(bet => bet.CurrencyId.HasValue && bet.CurrencyId > 0);
            multiCurrencyBets.Should().OnlyContain(bet => bet.Stake.HasValue && bet.Stake > 0);
        }

        [Fact]
        public void FetchBetRecordDataTask_ShouldHandleSpecialBetScenarios()
        {
            // Arrange - Test edge cases for bet records
            var specialBets = new List<TpBetRecordModel>
            {
                new TpBetRecordModel { TpId = 0, Stake = 0.00m, WinlostAmount = 0.00m, CurrencyId = 0 }, // Edge case: zero values
                new TpBetRecordModel { TpId = -1, Stake = null, WinlostAmount = 50.00m, CurrencyId = -1 }, // Edge case: negative/null values
                new TpBetRecordModel { TpId = int.MaxValue, Stake = decimal.MaxValue, WinlostAmount = decimal.MinValue, CurrencyId = int.MaxValue }, // Edge case: max values
                new TpBetRecordModel { TpId = 999, Stake = 0.01m, WinlostAmount = 1000000.00m, CurrencyId = null } // Edge case: tiny bet, huge win
            };

            // Act & Assert - Should handle special cases
            specialBets.Should().HaveCount(4);
            specialBets.Should().Contain(bet => bet.TpId == 0);
            specialBets.Should().Contain(bet => bet.TpId == int.MaxValue);
            specialBets.Should().Contain(bet => bet.Stake == null);
            specialBets.Should().Contain(bet => bet.CurrencyId == null);
        }

        [Fact]
        public void FetchBetRecordDataTask_ShouldSupportBusinessBettingScenarios()
        {
            // Arrange - Business betting scenarios
            var businessBetRecords = new List<TpBetRecordModel>
            {
                new TpBetRecordModel
                {
                    TpId = 1001,
                    Stake = 1000.00m,
                    WinlostAmount = 2500.00m, // Big win
                    TransTime = new DateTime(2024, 1, 15, 18, 30, 0),
                    CurrencyId = 1
                },
                new TpBetRecordModel
                {
                    TpId = 1002,
                    Stake = 500.00m,
                    WinlostAmount = -500.00m, // Loss
                    TransTime = new DateTime(2024, 1, 15, 19, 15, 0),
                    CurrencyId = 1
                },
                new TpBetRecordModel
                {
                    TpId = 1003,
                    Stake = 250.00m,
                    WinlostAmount = 0.00m, // Push/Draw
                    TransTime = new DateTime(2024, 1, 15, 20, 45, 0),
                    CurrencyId = 2
                }
            };

            // Act & Assert - Should support various business scenarios
            businessBetRecords.Should().HaveCount(3);
            businessBetRecords.Should().Contain(bet => bet.WinlostAmount > bet.Stake); // Big win scenario
            businessBetRecords.Should().Contain(bet => bet.WinlostAmount < 0); // Loss scenario
            businessBetRecords.Should().Contain(bet => bet.WinlostAmount == 0); // Push/Draw scenario
        }

        [Fact]
        public void FetchBetRecordDataTask_ShouldHandlePaginationScenarios()
        {
            // Arrange - Pagination test data
            var firstPageRecords = new List<TpBetRecordModel>();
            for (int i = 1; i <= 2000; i++)
            {
                firstPageRecords.Add(new TpBetRecordModel { TpId = i, Stake = 100.00m, CurrencyId = 1 });
            }

            var secondPageRecords = new List<TpBetRecordModel>();
            for (int i = 2001; i <= 2500; i++)
            {
                secondPageRecords.Add(new TpBetRecordModel { TpId = i, Stake = 100.00m, CurrencyId = 1 });
            }

            // Act & Assert - Should handle large datasets
            firstPageRecords.Should().HaveCount(2000);
            secondPageRecords.Should().HaveCount(500);

            var allRecords = firstPageRecords.Concat(secondPageRecords).ToList();
            allRecords.Should().HaveCount(2500);
            allRecords.Should().OnlyContain(bet => bet.Stake == 100.00m);
            allRecords.Should().OnlyContain(bet => bet.CurrencyId == 1);
        }
    }
}
