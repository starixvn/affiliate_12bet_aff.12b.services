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
using Newtonsoft.Json;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.RetryJob
{
    /// <summary>
    /// Unit tests for ReInsertAffiliateInfoDataTask class.
    /// Tests affiliate information re-insertion retry functionality.
    /// </summary>
    public class ReInsertAffiliateInfoDataTaskTests
    {
        private readonly Mock<IDataTrackerService> _mockTrackerService;
        private readonly Mock<ILicenseeApi> _mockLicenseeApi;
        private readonly Mock<ILogger<ReInsertAffiliateInfoDataTask>> _mockLogger;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly ReInsertAffiliateInfoDataTask _task;

        public ReInsertAffiliateInfoDataTaskTests()
        {
            _mockTrackerService = new Mock<IDataTrackerService>();
            _mockLicenseeApi = new Mock<ILicenseeApi>();
            _mockLogger = new Mock<ILogger<ReInsertAffiliateInfoDataTask>>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                SupportedPrefix = new[] { "DB1", "DB2", "DB3" }
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.INSERT_AFFILIATE_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new ReInsertAffiliateInfoDataTask(
                _mockTrackerService.Object,
                _mockLicenseeApi.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new ReInsertAffiliateInfoDataTask(
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
            var listPullStatusIdField = typeof(ReInsertAffiliateInfoDataTask)
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
                    PushDataInfo = JsonConvert.SerializeObject(new AffiliateParam { CustId = 1 }),
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
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should call FetchTrackerAsync with correct parameters
            _mockTrackerService.Verify(x => x.FetchTrackerAsync(
                It.Is<IEnumerable<string>>(prefixes => prefixes.SequenceEqual(_fetchServiceSetting.SupportedPrefix)),
                It.Is<IEnumerable<int>>(types => types.Contains(DataTrackerType.InsertAffiliate)),
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
            _mockLicenseeApi.Verify(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()), Times.Never);
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessEachTrackerItem()
        {
            // Arrange
            var affiliateParam1 = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var affiliateParam2 = new AffiliateParam { CustId = 2, AffiliateId = 200 };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonConvert.SerializeObject(affiliateParam1),
                    PrefixSiteName = "DB1",
                    PushTrackerId = 1
                },
                new DataTrackerItem
                {
                    PushDataInfo = JsonConvert.SerializeObject(affiliateParam2),
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
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should process each tracker item
            _mockLicenseeApi.Verify(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()), Times.Exactly(2));
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                DataTrackerStatus.Success),
                Times.Exactly(2));
        }

        [Fact]
        public async Task HandleAsync_ShouldDeserializeAffiliateParamCorrectly()
        {
            // Arrange
            var originalAffiliateParam = new AffiliateParam
            {
                CustId = 123,
                AffiliateId = 456,
                AffiliateName = "TestAffiliate",
                Email = "test@example.com"
            };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonConvert.SerializeObject(originalAffiliateParam),
                    PrefixSiteName = "DB1"
                }
            };

            var capturedAffiliateParam = new AffiliateParam();
            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(dataTrackerItems);

            _mockLicenseeApi
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .Callback<AffiliateParam>(param => capturedAffiliateParam = param)
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should deserialize AffiliateParam correctly
            capturedAffiliateParam.CustId.Should().Be(originalAffiliateParam.CustId);
            capturedAffiliateParam.AffiliateId.Should().Be(originalAffiliateParam.AffiliateId);
            capturedAffiliateParam.AffiliateName.Should().Be(originalAffiliateParam.AffiliateName);
            capturedAffiliateParam.Email.Should().Be(originalAffiliateParam.Email);
        }

        [Fact]
        public async Task HandleAsync_ShouldUpdateStatusToSuccess_WhenApiCallSucceeds()
        {
            // Arrange
            var affiliateParam = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonConvert.SerializeObject(affiliateParam),
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
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
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
            var affiliateParam = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonConvert.SerializeObject(affiliateParam),
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
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = false, ErrMsg = "API Error Message" });

            // Act
            await _task.HandleAsync();

            // Assert - Should log error when API call fails
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Insert affiliate to 12B DB fail")),
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

            // Act & Assert - Should throw JsonException for invalid JSON
            await Assert.ThrowsAsync<JsonReaderException>(() => _task.HandleAsync());

            // Should not call API or update tracker status when deserialization fails
            _mockLicenseeApi.Verify(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()), Times.Never);
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                It.IsAny<int>()),
                Times.Never);
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

            // Act & Assert - Should handle null PushDataInfo gracefully
            await Assert.ThrowsAsync<ArgumentNullException>(() => _task.HandleAsync());
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessMixedSuccessAndFailure()
        {
            // Arrange
            var affiliateParam1 = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var affiliateParam2 = new AffiliateParam { CustId = 2, AffiliateId = 200 };

            var dataTrackerItems = new List<DataTrackerItem>
            {
                new DataTrackerItem
                {
                    PushDataInfo = JsonConvert.SerializeObject(affiliateParam1),
                    PrefixSiteName = "DB1",
                    PushTrackerId = 1
                },
                new DataTrackerItem
                {
                    PushDataInfo = JsonConvert.SerializeObject(affiliateParam2),
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
                .SetupSequence(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true })
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = false, ErrMsg = "Second call failed" });

            // Act
            await _task.HandleAsync();

            // Assert - Should process both items
            _mockLicenseeApi.Verify(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()), Times.Exactly(2));

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
            var affiliateParam = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonConvert.SerializeObject(affiliateParam),
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
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should process items with InProcess or Failure status
            _mockLicenseeApi.Verify(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()), Times.Once);
            _mockTrackerService.Verify(x => x.UpdateTrackerStatusAsync(
                It.IsAny<DataTrackerItem>(),
                DataTrackerStatus.Success),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldUseCorrectDataTrackerType()
        {
            // Arrange
            var affiliateParam = new AffiliateParam { CustId = 1, AffiliateId = 100 };
            var dataTrackerItem = new DataTrackerItem
            {
                PushDataInfo = JsonConvert.SerializeObject(affiliateParam),
                PrefixSiteName = "DB1"
            };

            _mockTrackerService
                .Setup(x => x.FetchTrackerAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new[] { dataTrackerItem });

            _mockLicenseeApi
                .Setup(x => x.InsertAffiliateInfo(It.IsAny<AffiliateParam>()))
                .ReturnsAsync(new Common.DataDomain.Api.ResultInfo { IsSuccess = true });

            // Act
            await _task.HandleAsync();

            // Assert - Should fetch tracker with InsertAffiliate type
            _mockTrackerService.Verify(x => x.FetchTrackerAsync(
                It.IsAny<IEnumerable<string>>(),
                It.Is<IEnumerable<int>>(types => types.Contains(DataTrackerType.InsertAffiliate)),
                It.IsAny<IEnumerable<int>>()),
                Times.Once);
        }
    }
}
