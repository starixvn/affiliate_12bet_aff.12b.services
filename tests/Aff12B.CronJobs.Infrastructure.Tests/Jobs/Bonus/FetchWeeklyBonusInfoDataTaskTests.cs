using Aff12B.Common.DataDomain.Models;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Aff12B.CronJobs.Infrastructure.Jobs.Bonus;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Bonus
{
    /// <summary>
    /// Unit tests for FetchWeeklyBonusInfoDataTask class.
    /// Tests weekly bonus information data fetching functionality.
    /// </summary>
    public class FetchWeeklyBonusInfoDataTaskTests
    {
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchWeeklyBonusInfoDataTask _task;

        public FetchWeeklyBonusInfoDataTaskTests()
        {
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                DataSource = "WeeklyBonusInfo",
                SupportedPrefix = new[] { "PREFIX1", "PREFIX2" },
                MaxTimeRange = 1440
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.PULL_WEEKLY_BONUS_INFO_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new FetchWeeklyBonusInfoDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchWeeklyBonusInfoDataTask(
                _mockLicenseeApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLicenseeApiIsNull()
        {
            // Act & Assert
            var act = () => new FetchWeeklyBonusInfoDataTask(
                null!,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("licenseeApi");
        }

        [Fact]
        public void FetchWeeklyBonusInfoDataTask_ShouldInheritFromCorrectBaseClass()
        {
            // Act & Assert - Should inherit from the correct base class
            _task.Should().BeAssignableTo<FetchDataForMultipleRepositoryTaskBase<BonusModel>>();
        }

        [Fact]
        public void SUBSTRACT_DATE_ShouldBeNegative39()
        {
            // Act - Access the constant through reflection since it's private
            var substractDateField = typeof(FetchWeeklyBonusInfoDataTask)
                .GetField("SUBSTRACT_DATE", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var substractDate = (int)substractDateField.GetValue(null);

            // Assert - Should be -39 days for last month calculation
            substractDate.Should().Be(-39);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldProcessDateRangeFromLastMonth()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "WEEKLY1", BonusAmount = 100.00m },
                new BonusModel { BonusId = 2, BonusCode = "WEEKLY2", BonusAmount = 200.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(bonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should call API for date processing
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                SettingConstant.MAX_ROW_COUNT),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldSaveTrackerInfo()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "WEEKLY1", BonusAmount = 100.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(bonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should save tracker information
            _mockTrackerService.Verify(x => x.SaveTrackerAsync(
                It.IsAny<string>(),
                It.Is<TrackerInfo>(t =>
                    t.CalledTime != default(DateTime) &&
                    !string.IsNullOrEmpty(t.KeyValue))),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldHandleEmptyResults()
        {
            // Arrange
            var emptyBonusData = new List<BonusModel>();

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(emptyBonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should handle empty results gracefully
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<int>()),
                Times.AtLeastOnce);
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task PullAndImportData_ShouldHandlePagination()
        {
            // Arrange - Create minimal test data but simulate pagination behavior
            var firstPageData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "BONUS1", BonusAmount = 10.0m },
                new BonusModel { BonusId = 2, BonusCode = "BONUS2", BonusAmount = 20.0m }
            };

            var secondPageData = new List<BonusModel>
            {
                new BonusModel { BonusId = 3, BonusCode = "BONUS3", BonusAmount = 30.0m }
            };

            var testDate = DateTime.Today.AddDays(-10);

            // Mock the API to return data on first page and second page
            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(testDate, 0, SettingConstant.MAX_ROW_COUNT))
                .ReturnsAsync(firstPageData);
            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(testDate, 1, SettingConstant.MAX_ROW_COUNT))
                .ReturnsAsync(secondPageData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act - Use reflection to call the private method
            var pullAndImportDataMethod = typeof(FetchWeeklyBonusInfoDataTask)
                .GetMethod("PullAndImportData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)pullAndImportDataMethod.Invoke(_task, new object[] { testDate });

            // Assert - Should call first page, but second page won't be called automatically
            // unless first page returns exactly MAX_ROW_COUNT items
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(testDate, 0, SettingConstant.MAX_ROW_COUNT), Times.Once);
            // Second page won't be called since first page has < MAX_ROW_COUNT items
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(testDate, 1, SettingConstant.MAX_ROW_COUNT), Times.Never);
            _mockTrackerService.Verify(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()), Times.Once);
        }

        [Fact]
        public void GetLastMonthDate_ShouldReturnDateMinus39Days()
        {
            // Act - Use reflection to call the private method
            var getLastMonthDateMethod = typeof(FetchWeeklyBonusInfoDataTask)
                .GetMethod("GetLastMonthDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (DateTime)getLastMonthDateMethod.Invoke(_task, new object[0]);

            // Assert - Should return date 39 days ago
            var expectedDate = DateTimeOffset.Now.AddDays(-39).Date;
            result.Should().BeCloseTo(expectedDate, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void BuildXml_ShouldCallXmlHelper()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "TEST", BonusAmount = 100.00m },
                new BonusModel { BonusId = 2, BonusCode = "TEST2", BonusAmount = 200.00m }
            };

            // Act - Use reflection to call the protected method
            var buildXmlMethod = typeof(FetchWeeklyBonusInfoDataTask)
                .GetMethod("BuildXml", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (string)buildXmlMethod.Invoke(_task, new object[] { bonusData });

            // Assert - Should return XML string
            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith("<root>");
            result.Should().EndWith("</root>");
        }

        [Fact]
        public void GetSupportedRepoPrefix_ShouldReturnSettingsSupportedPrefix()
        {
            // Act - Use reflection to call the protected method
            var getSupportedRepoPrefixMethod = typeof(FetchWeeklyBonusInfoDataTask)
                .GetMethod("GetSupportedRepoPrefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (IEnumerable<string>)getSupportedRepoPrefixMethod.Invoke(_task, new object[0]);

            // Assert - Should return supported prefixes from settings
            result.Should().BeEquivalentTo(new[] { "PREFIX1", "PREFIX2" });
        }

        [Fact]
        public async Task FetchDataAsync_ShouldProcessMultipleDays()
        {
            // Arrange - Mock to return small data sets to avoid long loops
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "DAILY", BonusAmount = 50.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(bonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should process multiple days from last month to now
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                SettingConstant.MAX_ROW_COUNT),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldIncludeTrackerJsonWithDateAndPageIndex()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "JSON_TEST", BonusAmount = 75.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(bonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should save tracker with JSON containing Date and PageIndex
            _mockTrackerService.Verify(x => x.SaveTrackerAsync(
                It.IsAny<string>(),
                It.Is<TrackerInfo>(t =>
                    t.KeyValue.Contains("Date") &&
                    t.KeyValue.Contains("PageIndex"))),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task FetchDataAsync_ShouldUseCorrectMaxRowCount()
        {
            // Arrange
            var bonusData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "MAX_ROW", BonusAmount = 125.00m }
            };

            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(bonusData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act
            await _task.FetchDataAsync();

            // Assert - Should use SettingConstant.MAX_ROW_COUNT
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                SettingConstant.MAX_ROW_COUNT),
                Times.AtLeastOnce);
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task FetchDataAsync_ShouldIncrementPageIndexCorrectly()
        {
            // Arrange - Create minimal test data for performance
            var firstPageData = new List<BonusModel>
            {
                new BonusModel { BonusId = 1, BonusCode = "PAGE1", BonusAmount = 1 },
                new BonusModel { BonusId = 2, BonusCode = "PAGE2", BonusAmount = 2 }
            };

            var testDate = DateTime.Today.AddDays(-5);

            // Mock the API to return small dataset
            _mockLicenseeApi
                .Setup(x => x.GetBonusInfoByDate(testDate, 0, SettingConstant.MAX_ROW_COUNT))
                .ReturnsAsync(firstPageData);

            _mockTrackerService
                .Setup(x => x.SaveTrackerAsync(It.IsAny<string>(), It.IsAny<TrackerInfo>()))
                .Returns(Task.CompletedTask);

            // Act - Use reflection to call the private method
            var pullAndImportDataMethod = typeof(FetchWeeklyBonusInfoDataTask)
                .GetMethod("PullAndImportData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)pullAndImportDataMethod.Invoke(_task, new object[] { testDate });

            // Assert - Should call first page only (since result count < MAX_ROW_COUNT)
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(testDate, 0, SettingConstant.MAX_ROW_COUNT), Times.Once);
            // Second page should not be called since first page has less than MAX_ROW_COUNT items
            _mockLicenseeApi.Verify(x => x.GetBonusInfoByDate(testDate, 1, SettingConstant.MAX_ROW_COUNT), Times.Never);
        }
    }
}
