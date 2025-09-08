using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.DataDomain.Params;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Apis;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.Jobs.Base;
using Aff12B.CronJobs.Infrastructure.Jobs.Product9W;
using Aff12B.CronJobs.Infrastructure.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs.Product9W
{
    /// <summary>
    /// Unit tests for FetchTpReport9WDataTask class.
    /// Tests Product9W report data fetching functionality.
    /// </summary>
    public class FetchTpReport9WDataTaskTests
    {
        private readonly Mock<IProduct9WApi> _mockProduct9WApi;
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchTpReport9WDataTask _task;

        public FetchTpReport9WDataTaskTests()
        {
            _mockProduct9WApi = new Mock<IProduct9WApi>();
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                DataSource = "TpReport9W",
                AdjustmentValue = 5,
                SupportedPrefix = new[] { "9W1", "9W2" }
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.PULL_TPREPORT_9W_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new FetchTpReport9WDataTask(
                _mockProduct9WApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchTpReport9WDataTask(
                _mockProduct9WApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchTpReport9WDataTask_ShouldInheritFromCorrectBaseClass()
        {
            // Act & Assert - Should inherit from the correct base class
            _task.Should().BeAssignableTo<FetchDataByBatchForMultiRepoTaskBase<TpReport9WModel, SQTrackerInfo>>();
        }

        [Fact]
        public async Task GetDataAsync_ShouldCallProduct9WApiWithCorrectParameters()
        {
            // Arrange
            var trackValue = new SQTrackerInfo
            {
                SQ = 12345,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1000;
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 12345, UpdateTime = DateTime.Now },
                new TpReport9WModel { SQ = 12346, UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpReportAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(reportData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpReport9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should call API with correct parameters
            _mockProduct9WApi.Verify(x => x.GetTpReportAsync(
                It.Is<BatchCountParam>(p =>
                    p.SQ == trackValue.SQ &&
                    p.FromDate == trackValue.UpdateTime &&
                    p.BatchCount == batchCount &&
                    p.ToDate <= DateTime.Now)),
                Times.Once);

            result.Should().BeEquivalentTo(reportData);
        }

        [Fact]
        public async Task GetDataAsync_ShouldUseCurrentDateTimeAsToDate()
        {
            // Arrange
            var trackValue = new SQTrackerInfo
            {
                SQ = 12345,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1000;
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 12345, UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpReportAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(reportData);

            // Act - Use reflection to call the protected method
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpReport9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should use current datetime as ToDate
            _mockProduct9WApi.Verify(x => x.GetTpReportAsync(
                It.Is<BatchCountParam>(p =>
                    p.ToDate <= DateTime.Now &&
                    p.ToDate >= DateTime.Now.AddMinutes(-1))), // Allow for small time difference
                Times.Once);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldReturnCorrectSQTrackerInfo()
        {
            // Arrange
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 12345, UpdateTime = DateTime.Now.AddHours(-2) },
                new TpReport9WModel { SQ = 12346, UpdateTime = DateTime.Now.AddHours(-1) },
                new TpReport9WModel { SQ = 12347, UpdateTime = DateTime.Now }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpReport9WDataTask), "GetInsertTrackingValue");
            var result = (SQTrackerInfo)method.Invoke(_task, new object[] { reportData });

            // Assert - Should return tracking info from the last item
            result.Should().NotBeNull();
            result.SQ.Should().Be(12347);
            result.UpdateTime.Should().Be(reportData.Last().UpdateTime.Value);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldThrowException_WhenDataIsEmpty()
        {
            // Arrange
            var emptyData = new List<TpReport9WModel>();

            // Act & Assert - Should throw InvalidOperationException for empty data
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpReport9WDataTask), "GetInsertTrackingValue");

            var act = () => method.Invoke(_task, new object[] { emptyData });

            act.Should().Throw<System.Reflection.TargetInvocationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage("TpReport9W cannot determine tracking value: data collection is empty.");
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldThrowException_WhenLastItemUpdateTimeIsNull()
        {
            // Arrange
            var reportDataWithNullUpdateTime = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 12345, UpdateTime = DateTime.Now },
                new TpReport9WModel { SQ = 12346, UpdateTime = null } // Last item has null UpdateTime
            };

            // Act & Assert - Should throw InvalidOperationException for null UpdateTime
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpReport9WDataTask), "GetInsertTrackingValue");

            var act = () => method.Invoke(_task, new object[] { reportDataWithNullUpdateTime });

            act.Should().Throw<System.Reflection.TargetInvocationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage("TpReport9W cannot determine tracking value: last item's UpdateTime is null.");
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldReturnAdjustedSQTrackerInfo()
        {
            // Arrange
            var originalTrackerInfo = new SQTrackerInfo
            {
                SQ = 12345,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var trackerInfo = new TrackerInfo
            {
                KeyValue = JsonConvert.SerializeObject(originalTrackerInfo),
                CalledTime = DateTime.Now.AddHours(-1)
            };

            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<SQTrackerInfo>(_task, "GetLastTrackingValueAsync");

            // Assert - Should return adjusted SQTrackerInfo
            result.Should().NotBeNull();
            result.SQ.Should().Be(originalTrackerInfo.SQ);
            result.UpdateTime.Should().Be(originalTrackerInfo.UpdateTime.AddMinutes(_fetchServiceSetting.AdjustmentValue));
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldGetTrackerFromService()
        {
            // Arrange
            var originalTrackerInfo = new SQTrackerInfo
            {
                SQ = 12345,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var trackerInfo = new TrackerInfo
            {
                KeyValue = JsonConvert.SerializeObject(originalTrackerInfo),
                CalledTime = DateTime.Now.AddHours(-1)
            };

            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<SQTrackerInfo>(_task, "GetLastTrackingValueAsync");

            // Assert - Should call tracker service
            _mockTrackerService.Verify(x => x.GetTrackerAsync(
                It.IsAny<string>(),
                It.IsAny<Func<TrackerInfo, DateTime>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldDeserializeTrackerKeyValue()
        {
            // Arrange
            var originalTrackerInfo = new SQTrackerInfo
            {
                SQ = 98765,
                UpdateTime = DateTime.Now.AddDays(-2)
            };
            var trackerInfo = new TrackerInfo
            {
                KeyValue = JsonConvert.SerializeObject(originalTrackerInfo),
                CalledTime = DateTime.Now.AddHours(-2)
            };

            _mockTrackerService
                .Setup(x => x.GetTrackerAsync(It.IsAny<string>(), It.IsAny<Func<TrackerInfo, DateTime>>()))
                .ReturnsAsync(trackerInfo);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<SQTrackerInfo>(_task, "GetLastTrackingValueAsync");

            // Assert - Should correctly deserialize and adjust the tracking info
            result.Should().NotBeNull();
            result.SQ.Should().Be(originalTrackerInfo.SQ);
            result.UpdateTime.Should().Be(originalTrackerInfo.UpdateTime.AddMinutes(_fetchServiceSetting.AdjustmentValue));
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleEmptyResults()
        {
            // Arrange
            var trackValue = new SQTrackerInfo
            {
                SQ = 12345,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1000;
            var emptyReportData = new List<TpReport9WModel>();

            _mockProduct9WApi
                .Setup(x => x.GetTpReportAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(emptyReportData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpReport9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should handle empty results gracefully
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDataAsync_ShouldCreateBatchCountParamWithAllRequiredProperties()
        {
            // Arrange
            var trackValue = new SQTrackerInfo
            {
                SQ = 54321,
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 2000;
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 54321, UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpReportAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(reportData);

            // Act - Use reflection to call the protected method
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpReport9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should create BatchCountParam with all required properties
            _mockProduct9WApi.Verify(x => x.GetTpReportAsync(
                It.Is<BatchCountParam>(p =>
                    p.SQ == trackValue.SQ &&
                    p.FromDate == trackValue.UpdateTime &&
                    p.BatchCount == batchCount &&
                    p.ToDate > trackValue.UpdateTime)), // ToDate should be later than FromDate
                Times.Once);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldUseSQFromLastItem()
        {
            // Arrange
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 11111, UpdateTime = DateTime.Now.AddHours(-3) },
                new TpReport9WModel { SQ = 22222, UpdateTime = DateTime.Now.AddHours(-2) },
                new TpReport9WModel { SQ = 33333, UpdateTime = DateTime.Now.AddHours(-1) }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpReport9WDataTask), "GetInsertTrackingValue");
            var result = (SQTrackerInfo)method.Invoke(_task, new object[] { reportData });

            // Assert - Should use SQ from the last item in the collection
            result.SQ.Should().Be(33333);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldUseUpdateTimeFromLastItem()
        {
            // Arrange
            var lastUpdateTime = DateTime.Now;
            var reportData = new List<TpReport9WModel>
            {
                new TpReport9WModel { SQ = 11111, UpdateTime = DateTime.Now.AddHours(-3) },
                new TpReport9WModel { SQ = 22222, UpdateTime = DateTime.Now.AddHours(-2) },
                new TpReport9WModel { SQ = 33333, UpdateTime = lastUpdateTime }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpReport9WDataTask), "GetInsertTrackingValue");
            var result = (SQTrackerInfo)method.Invoke(_task, new object[] { reportData });

            // Assert - Should use UpdateTime from the last item in the collection
            result.UpdateTime.Should().Be(lastUpdateTime);
        }
    }
}
