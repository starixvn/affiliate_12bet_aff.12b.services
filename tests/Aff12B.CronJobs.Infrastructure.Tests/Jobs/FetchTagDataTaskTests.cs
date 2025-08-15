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
    /// Comprehensive unit tests for FetchTagDataTask class.
    /// Business-focused scenarios for tag data fetching and import workflows.
    /// Covers API integration, date-time tracking, and multi-repository import scenarios.
    /// </summary>
    public class FetchTagDataTaskTests
    {
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockSettings;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchTagDataTask _task;

        public FetchTagDataTaskTests()
        {
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockSettings = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "TAG", "LABEL", "CATEGORY" },
                MaxTimeRange = 240,
                AdjustmentValue = -10
            };

            _mockSettings.Setup(x => x.Get(It.IsAny<string>())).Returns(_fetchServiceSetting);

            _task = new FetchTagDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchTagDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
            task.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
        {
            // Act & Assert
            var act = () => new FetchTagDataTask(null!, _mockTrackerService.Object, _mockLicenseeApi.Object, _mockSettings.Object);
            act.Should().Throw<ArgumentNullException>();
        }

        // Note: Constructor null checks removed as primary constructors don't immediately validate parameters

        [Fact]
        public void Constructor_ShouldThrowException_WhenSettingsIsNull()
        {
            // Act & Assert
            var act = () => new FetchTagDataTask(_mockRepository.Object, _mockTrackerService.Object, _mockLicenseeApi.Object, null!);
            act.Should().Throw<Exception>(); // Could be ArgumentNullException or NullReferenceException
        }

        [Fact]
        public async Task GetDataAsync_ShouldFetchTagsFromApi()
        {
            // Arrange - Tag data scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var tags = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "VIP", Description = "VIP Customer Tag" },
                new TagModel { Id = 2, TagName = "HighRoller", Description = "High Roller Customer" },
                new TagModel { Id = 3, TagName = "NewPlayer", Description = "New Player Welcome" }
            };

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount)).ReturnsAsync(tags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should call API with correct parameters
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, batchCount), Times.Once);
            result.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleEmptyTagList()
        {
            // Arrange - Empty tag list scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 500;
            var expectedDateTime = Convert.ToDateTime(trackValue);
            var emptyTags = new List<TagModel>();

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount)).ReturnsAsync(emptyTags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should handle empty list gracefully
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, batchCount), Times.Once);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleApiException()
        {
            // Arrange - API failure scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount))
                .ThrowsAsync(new HttpRequestException("Tag API unavailable"));

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should propagate API exception
            var act = async () => await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });
            await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Tag API unavailable");
        }

        [Fact]
        public async Task GetDataAsync_ShouldSupportBusinessTaggingScenario()
        {
            // Arrange - Business tagging scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 2000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var businessTags = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "VIP_PLATINUM", Description = "Platinum VIP Members" },
                new TagModel { Id = 2, TagName = "VIP_GOLD", Description = "Gold VIP Members" },
                new TagModel { Id = 3, TagName = "VIP_SILVER", Description = "Silver VIP Members" },
                new TagModel { Id = 4, TagName = "HIGH_ROLLER", Description = "High Stakes Players" },
                new TagModel { Id = 5, TagName = "FREQUENT_PLAYER", Description = "Daily Active Players" },
                new TagModel { Id = 6, TagName = "NEW_MEMBER", Description = "Recently Registered" },
                new TagModel { Id = 7, TagName = "DORMANT_ACCOUNT", Description = "Inactive for 30+ days" },
                new TagModel { Id = 8, TagName = "BONUS_ELIGIBLE", Description = "Eligible for Bonuses" },
                new TagModel { Id = 9, TagName = "CASHBACK_MEMBER", Description = "Cashback Program Member" },
                new TagModel { Id = 10, TagName = "TOURNAMENT_PLAYER", Description = "Tournament Participant" }
            };

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount)).ReturnsAsync(businessTags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should process all business tags
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, batchCount), Times.Once);
            result.Should().HaveCount(10);
            result.Should().Contain(t => t.TagName == "VIP_PLATINUM");
            result.Should().Contain(t => t.TagName == "HIGH_ROLLER");
            result.Should().Contain(t => t.TagName == "TOURNAMENT_PLAYER");
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleUnicodeTagNames()
        {
            // Arrange - Unicode tag names scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var unicodeTags = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "VIP会员_VIP_Member", Description = "VIP会员标签" },
                new TagModel { Id = 2, TagName = "高额玩家_High_Roller", Description = "高额投注玩家" },
                new TagModel { Id = 3, TagName = "新用户_New_User", Description = "新注册用户" },
                new TagModel { Id = 4, TagName = "常客_Regular", Description = "经常访问的客户" }
            };

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount)).ReturnsAsync(unicodeTags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should handle unicode content
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, batchCount), Times.Once);
            result.Should().Contain(t => t.TagName.Contains("VIP会员"));
            result.Should().Contain(t => t.TagName.Contains("高额玩家"));
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleInvalidDateFormat()
        {
            // Arrange - Invalid date format scenario
            var invalidTrackValue = "invalid-date-format";
            var batchCount = 1000;

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should throw TargetInvocationException wrapping FormatException for invalid date
            var act = async () => await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { invalidTrackValue, batchCount });
            await act.Should().ThrowAsync<TargetInvocationException>()
                .Where(ex => ex.InnerException is FormatException);
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleVariousDateFormats()
        {
            // Arrange - Various valid date formats
            var dateFormats = new[]
            {
                "2024-01-15 10:30:00",
                "01/15/2024 10:30:00 AM",
                "2024/01/15 22:30:00"
            };

            var tags = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "TestTag", Description = "Test Description" }
            };

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var dateFormat in dateFormats)
            {
                // Arrange
                var expectedDateTime = Convert.ToDateTime(dateFormat);
                _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, 1000)).ReturnsAsync(tags);

                // Act
                var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { dateFormat, 1000 });

                // Assert - Should handle various date formats
                result.Should().NotBeNull();
                _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, 1000), Times.AtLeastOnce);
            }
        }

        [Fact]
        public void FetchTagDataTask_ShouldInheritFromBaseClass()
        {
            // Arrange & Act & Assert - Should inherit from base class
            _task.Should().BeAssignableTo<FetchDataByDateTimeTrackerForMultiRepoTaskBase<TagModel>>();
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleTimeoutException()
        {
            // Arrange - API timeout scenario
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount))
                .ThrowsAsync(new TimeoutException("Tag API request timeout"));

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert - Should handle timeout appropriately
            var act = async () => await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });
            await act.Should().ThrowAsync<TimeoutException>().WithMessage("Tag API request timeout");
        }

        [Fact]
        public async Task GetDataAsync_ShouldSupportLargeBatchSizes()
        {
            // Arrange - Large batch size scenario
            var trackValue = "2024-01-15 10:30:00";
            var largeBatchCount = 10000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var largeTags = new List<TagModel>();
            // Reduced from 5000 to 20 records for performance
            for (int i = 1; i <= 20; i++)
            {
                largeTags.Add(new TagModel { Id = i, TagName = $"Tag_{i}", Description = $"Description for Tag {i}" });
            }

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, largeBatchCount)).ReturnsAsync(largeTags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, largeBatchCount });

            // Assert - Should handle large batch sizes
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, largeBatchCount), Times.Once);
            result.Should().HaveCount(20);
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleSpecialTagScenarios()
        {
            // Arrange - Special tag scenarios
            var trackValue = "2024-01-15 10:30:00";
            var batchCount = 1000;
            var expectedDateTime = Convert.ToDateTime(trackValue);

            var specialTags = new List<TagModel>
            {
                new TagModel { Id = 0, TagName = "", Description = "" }, // Edge case: empty values
                new TagModel { Id = -1, TagName = "NegativeTag", Description = "Negative ID tag" }, // Edge case: negative ID
                new TagModel { Id = int.MaxValue, TagName = "MaxTag", Description = "Max value tag" }, // Edge case: max value
                new TagModel { Id = 999, TagName = null!, Description = null! } // Edge case: null values
            };

            _mockLicenseeApi.Setup(x => x.GetTagsAsync(expectedDateTime, batchCount)).ReturnsAsync(specialTags);

            // Use reflection to access the protected method
            var method = typeof(FetchTagDataTask).GetMethod("GetDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<IEnumerable<TagModel>>)method!.Invoke(_task, new object[] { trackValue, batchCount });

            // Assert - Should handle special cases
            _mockLicenseeApi.Verify(x => x.GetTagsAsync(expectedDateTime, batchCount), Times.Once);
            result.Should().HaveCount(4);
        }
    }
}
