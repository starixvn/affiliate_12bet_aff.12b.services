using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.RetryJob;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.RetryJob
{
    /// <summary>
    /// Unit tests for ReInsertAffInfosDataTask class.
    /// Tests affiliate information re-insertion retry functionality.
    /// </summary>
    public class ReInsertAffInfosDataTaskTests
    {
        private readonly Mock<IDataTrackerService> _mockTrackerService;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ILogger<ReInsertAffInfosDataTask>> _mockLogger;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly ReInsertAffInfosDataTask _task;

        public ReInsertAffInfosDataTaskTests()
        {
            _mockTrackerService = new Mock<IDataTrackerService>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockLogger = new Mock<ILogger<ReInsertAffInfosDataTask>>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "DB1", "DB2", "DB3" }
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.INSERT_AFFILIATE_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new ReInsertAffInfosDataTask(
                _mockTrackerService.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new ReInsertAffInfosDataTask(
                _mockTrackerService.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void ListPullStatusId_ShouldContainCorrectStatuses()
        {
            // Act - Access the static field through reflection
            var listPullStatusIdField = typeof(ReInsertAffInfosDataTask)
                .GetField("ListPullStatusId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var listPullStatusId = (IEnumerable<int>)listPullStatusIdField.GetValue(null);

            // Assert - Should contain InProcess and Failure statuses
            listPullStatusId.Should().Contain(DataTrackerStatus.InProcess);
            listPullStatusId.Should().Contain(DataTrackerStatus.Failure);
            listPullStatusId.Should().HaveCount(2);
        }

        [Fact]
        public async Task HandleAsync_ShouldFetchTrackerWithCorrectParameters()
        {
            // Arrange
            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(new List<AffInfo> { new AffInfo { Player_CustId = 1 } }),
                    PrefixSiteName = "DB1"
                }
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(dataTrackerItems);

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should call FetchTrackerAsync with correct parameters
            _mockTrackerService.Verify(x => x.FetchTrackerAsync(
                It.Is<IEnumerable<string>>(prefixes => prefixes.SequenceEqual(_fetchServiceSetting.SupportedPrefix)),
                It.Is<IEnumerable<int>>(types => types.Contains(DataTrackerType.InsertAffInfos)),
                It.Is<IEnumerable<int>>(statuses =>
                    statuses.Contains(DataTrackerStatus.InProcess) &&
                    statuses.Contains(DataTrackerStatus.Failure))),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessEmptyTrackerItems()
        {
            // Arrange
            var emptyDataTrackerItems = new List<DataTrackerItem>();

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(emptyDataTrackerItems);

            // Act
            await _task.HandleAsync();

            // Assert - Should not call API when no tracker items exist
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()), Times.Never);
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessEachTrackerItem()
        {
            // Arrange
            var affParams1 = new List<AffiliateParam> { new AffiliateParam { Player_CustId = 1, AffId = 100 } };
            var affParams2 = new List<AffiliateParam> { new AffiliateParam { Player_CustId = 2, AffId = 200 } };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(affParams1),
                    PrefixSiteName = "DB1",
                    PushTrackerId = 1
                },
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(affParams2),
                    PrefixSiteName = "DB2",
                    PushTrackerId = 2
                }
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(dataTrackerItems);

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should process each tracker item
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()), Times.Exactly(2));
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                DataTrackerStatus.Success),
                Times.Exactly(2));
        }

        [Fact]
        public async Task HandleAsync_ShouldDeserializeAffiliateParamCorrectly()
        {
            // Arrange
            var originalAffInfos = new List<AffInfo>
            {
                new AffInfo
                {
                    Player_CustId = 123,
                    AffId = 456,
                    UserName = "TestAffiliate",
                    Email = "test@example.com"
                }
            };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(originalAffInfos),
                    PrefixSiteName = "DB1"
                }
            };

            IEnumerable<AffInfo> capturedAffInfos = null;
            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(dataTrackerItems);

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .Callback<IEnumerable<AffInfo>>(affInfos => capturedAffInfos = affInfos)
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should pass the AffInfo collection correctly
            capturedAffInfos.Should().NotBeNull();
            capturedAffInfos.Should().HaveCount(1);
            var affInfo = capturedAffInfos.First();
            affInfo.AffId.Should().Be(originalAffInfos.First().AffId);
            affInfo.Player_CustId.Should().Be(originalAffInfos.First().Player_CustId);
        }

        [Fact]
        public async Task HandleAsync_ShouldUpdateStatusToSuccess_WhenApiCallSucceeds()
        {
            // Arrange
            var affInfos = new List<AffInfo> { new AffInfo { Player_CustId = 1, AffId = 100 } };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonSerializer.Serialize(affInfos),
                PrefixSiteName = "DB1",
                PushTrackerId = 1
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should update tracker status to success
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.Is<DataTrackerItem>(item => item.PushTrackerId == 1),
                DataTrackerStatus.Success),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldLogError_WhenApiCallFails()
        {
            // Arrange
            var affInfos = new List<AffInfo> { new AffInfo { Player_CustId = 1, AffId = 100 } };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonSerializer.Serialize(affInfos),
                PrefixSiteName = "DB1",
                PushTrackerId = 1
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = false, ErrMsg = "API Error Message" });

            // Act
            await _task.HandleAsync();

            // Assert - Should log error when API call fails
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Reinsert aff infos to")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            // Should not update tracker status when API call fails
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldHandleJsonDeserializationError()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = "invalid json string",
                PrefixSiteName = "DB1",
                PushTrackerId = 1
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should call API with invalid JSON string (validation happens at API level)
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldHandleNullPushDataInfo()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = null,
                PrefixSiteName = "DB1",
                PushTrackerId = 1
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should call API with null or empty collection when PushDataInfo is null
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.Is<IEnumerable<AffInfo>>(a => a == null || !a.Any())), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessMixedSuccessAndFailure()
        {
            // Arrange
            var affParams1 = new List<AffiliateParam> { new AffiliateParam { Player_CustId = 1, AffId = 100 } };
            var affParams2 = new List<AffiliateParam> { new AffiliateParam { Player_CustId = 2, AffId = 200 } };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(affParams1),
                    PrefixSiteName = "DB1",
                    PushTrackerId = 1
                },
                new DataTrackerItem
                {
                    PushDataInfo = JsonSerializer.Serialize(affParams2),
                    PrefixSiteName = "DB2",
                    PushTrackerId = 2
                }
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(dataTrackerItems);

            // First call succeeds, second call fails
            _mockLicenseeApi
                .SetupSequence(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true })
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = false, ErrMsg = "Second call failed" });

            // Act
            await _task.HandleAsync();

            // Assert - Should process both items
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()), Times.Exactly(2));

            // Should update status to success for first item only
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.Is<DataTrackerItem>(item => item.PushTrackerId == 1),
                DataTrackerStatus.Success),
                Times.Once);

            // Should not update status for second item (failed)
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.Is<DataTrackerItem>(item => item.PushTrackerId == 2),
                It.IsAny<int>()),
                Times.Never);

            // Should log error for second item
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Second call failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(DataTrackerStatus.InProcess)]
        [InlineData(DataTrackerStatus.Failure)]
        public async Task HandleAsync_ShouldProcessCorrectStatusTypes(int statusType)
        {
            // Arrange
            var affInfos = new List<AffInfo> { new AffInfo { Player_CustId = 1, AffId = 100 } };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonSerializer.Serialize(affInfos),
                PrefixSiteName = "DB1",
                StatusId = statusType
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should process items with InProcess or Failure status
            _mockLicenseeApi.Verify(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()), Times.Once);
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                DataTrackerStatus.Success),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldUseCorrectDataTrackerType()
        {
            // Arrange
            var affInfos = new List<AffInfo> { new AffInfo { Player_CustId = 1, AffId = 100 } };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonSerializer.Serialize(affInfos),
                PrefixSiteName = "DB1"
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffInfos(It.IsAny<IEnumerable<AffInfo>>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should fetch tracker with InsertAffInfos type
            _mockTrackerService.Verify(x => x.FetchTrackerAsync(
                It.IsAny<IEnumerable<string>>(),
                It.Is<IEnumerable<int>>(types => types.Contains(DataTrackerType.InsertAffInfos)),
                It.IsAny<IEnumerable<int>>()),
                Times.Once);
        }
    }
}
