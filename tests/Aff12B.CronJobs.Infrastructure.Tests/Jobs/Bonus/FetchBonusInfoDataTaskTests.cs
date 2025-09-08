using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Aff12B.CronJobs.Infrastructure.Jobs.Bonus;
using Aff12B.CronJobs.Infrastructure.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Bonus
{
    /// <summary>
    /// Unit tests for FetchBonusInfoDataTask class.
    /// Tests bonus information data fetching functionality.
    /// </summary>
    public class FetchBonusInfoDataTaskTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchBonusInfoDataTask _task;

        public FetchBonusInfoDataTaskTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                DataSource = "BonusInfo",
                AdjustmentValue = 10,
                MaxTimeRange = 1440
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.PULL_BONUS_INFO_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new FetchBonusInfoDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchBonusInfoDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchBonusInfoDataTask_ShouldInheritFromCorrectBaseClass()
        {
            // Act & Assert - Should inherit from the correct base class
            _task.Should().BeAssignableTo<FetchDataByBatchForMultiRepoTaskBase<BonusModel, string>>();
        }

        [Fact]
        public void PagingSize_ShouldReturn500()
        {
            // Act - Access the PagingSize property through cached reflection
            var property = ReflectionTestHelper.GetCachedProtectedProperty(typeof(FetchBonusInfoDataTask), "PagingSize");
            var pagingSize = (int)property.GetValue(_task);

            // Assert - Should return 500 as the paging size
            pagingSize.Should().Be(500);
        }

        [Fact]
        public async Task GetDataAsync_ShouldCallLicenseeApiWithCorrectParameters()
        {
            // Arrange
            var trackValue = "123";
            var batchCount = 500;
            var expectedBonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 123, BonusCode = "WELCOME", BonusAmount = 100.00m },
                new BonusModel { BonusId = 124, BonusCode = "RELOAD", BonusAmount = 50.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfo(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedBonusData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<BonusModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert
            _mockLicenseeApi.Verify(x => x.GetBonusInfo(123, 500), Times.Once);
            result.Should().BeEquivalentTo(expectedBonusData);
        }

        [Fact]
        public async Task GetDataAsync_ShouldConvertTrackValueToInteger()
        {
            // Arrange
            var trackValue = "456";
            var batchCount = 500;
            var expectedBonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 456, BonusCode = "VIP", BonusAmount = 200.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfo(456, 500))
                .ReturnsAsync(expectedBonusData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<BonusModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should convert string "456" to integer 456
            _mockLicenseeApi.Verify(x => x.GetBonusInfo(456, 500), Times.Once);
            result.Should().BeEquivalentTo(expectedBonusData);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldReturnMaxBonusIdAsString()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 100, BonusCode = "BONUS1" },
                new BonusModel { BonusId = 250, BonusCode = "BONUS2" },
                new BonusModel { BonusId = 175, BonusCode = "BONUS3" }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchBonusInfoDataTask), "GetInsertTrackingValue");
            var result = (string)method.Invoke(_task, new object[] { bonusData });

            // Assert - Should return the maximum BonusId as string
            result.Should().Be("250");
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldHandleSingleBonus()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 42, BonusCode = "SINGLE" }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchBonusInfoDataTask), "GetInsertTrackingValue");
            var result = (string)method.Invoke(_task, new object[] { bonusData });

            // Assert - Should return the single BonusId as string
            result.Should().Be("42");
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldReturnAdjustedIdFromTracker()
        {
            // Arrange
            var trackerInfo = new TrackerInfo { KeyValue = "100" };
            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<string>(_task, "GetLastTrackingValueAsync");

            // Assert - Should return adjusted value (100 - 10 = 90)
            result.Should().Be("90");
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldHandleEmptyKeyValue()
        {
            // Arrange
            var trackerInfo = new TrackerInfo { KeyValue = "" };
            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<string>(_task, "GetLastTrackingValueAsync");

            // Assert - Should return "0" when KeyValue is empty (0 - 10 = 0, clamped to 0)
            result.Should().Be("0");
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldHandleNullKeyValue()
        {
            // Arrange
            var trackerInfo = new TrackerInfo { KeyValue = null };
            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<string>(_task, "GetLastTrackingValueAsync");

            // Assert - Should return "0" when KeyValue is null (0 - 10 = 0, clamped to 0)
            result.Should().Be("0");
        }

        [Fact]
        public void GetAdjustmentId_ShouldReturnAdjustedValue()
        {
            // Arrange & Act - Use reflection to call the private method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchBonusInfoDataTask), "GetAdjustmentId");

            var result1 = (int)method.Invoke(_task, new object[] { 100 });
            var result2 = (int)method.Invoke(_task, new object[] { 50 });

            // Assert - Should subtract adjustment value (10)
            result1.Should().Be(90); // 100 - 10 = 90
            result2.Should().Be(40); // 50 - 10 = 40
        }

        [Fact]
        public void GetAdjustmentId_ShouldClampToZero()
        {
            // Arrange & Act - Use reflection to call the private method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchBonusInfoDataTask), "GetAdjustmentId");

            var result1 = (int)method.Invoke(_task, new object[] { 5 });
            var result2 = (int)method.Invoke(_task, new object[] { 0 });

            // Assert - Should clamp negative results to 0
            result1.Should().Be(0); // 5 - 10 = -5, clamped to 0
            result2.Should().Be(0); // 0 - 10 = -10, clamped to 0
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleEmptyResults()
        {
            // Arrange
            var trackValue = "999";
            var batchCount = 500;
            var emptyBonusData = new List<BonusModel>();

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfo(999, 500))
                .ReturnsAsync(emptyBonusData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<BonusModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should handle empty results gracefully
            result.Should().BeEmpty();
            _mockLicenseeApi.Verify(x => x.GetBonusInfo(999, 500), Times.Once);
        }

        [Fact]
        public async Task GetDataAsync_ShouldSupportLargeBatchSizes()
        {
            // Arrange
            var trackValue = "1000";
            var batchCount = 2000;
            // Reduced from 2000 to 25 records for performance
            var largeBonusData = Enumerable.Range(1000, 25)
                .Select(i => new BonusModel { BonusId = i, BonusCode = $"BONUS{i}", BonusAmount = i * 10.0m })
                .ToList();

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfo(1000, 2000))
                .ReturnsAsync(largeBonusData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<BonusModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should handle large batch sizes
            result.Should().HaveCount(25);
            result.First().BonusId.Should().Be(1000);
            result.Last().BonusId.Should().Be(1024);
            _mockLicenseeApi.Verify(x => x.GetBonusInfo(1000, 2000), Times.Once);
        }
    }
}
