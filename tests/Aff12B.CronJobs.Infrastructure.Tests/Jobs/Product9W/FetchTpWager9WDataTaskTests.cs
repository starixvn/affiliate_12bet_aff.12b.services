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
    /// Unit tests for FetchTpWager9WDataTask class.
    /// Tests Product9W wager data fetching functionality.
    /// </summary>
    public class FetchTpWager9WDataTaskTests
    {
        private readonly Mock<IProduct9WApi> _mockProduct9WApi;
        private readonly Mock<IThirdPartyImportRepository> _mockRepository;
        private readonly Mock<ITrackerService> _mockTrackerService;
        private readonly Mock<IOptionsSnapshot<FetchServiceSetting>> _mockConfiguration;
        private readonly FetchServiceSetting _fetchServiceSetting;
        private readonly FetchTpWager9WDataTask _task;

        public FetchTpWager9WDataTaskTests()
        {
            _mockProduct9WApi = new Mock<IProduct9WApi>();
            _mockRepository = new Mock<IThirdPartyImportRepository>();
            _mockTrackerService = new Mock<ITrackerService>();
            _mockConfiguration = new Mock<IOptionsSnapshot<FetchServiceSetting>>();

            _fetchServiceSetting = new FetchServiceSetting
            {
                DataSource = "TpWager9W",
                AdjustmentValue = 10,
                SupportedPrefix = new[] { "WAG1", "WAG2" }
            };

            _mockConfiguration
                .Setup(x => x.Get(ServiceConfigName.PULL_TPWAGER_9W_SETTING))
                .Returns(_fetchServiceSetting);

            _task = new FetchTpWager9WDataTask(
                _mockProduct9WApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new FetchTpWager9WDataTask(
                _mockProduct9WApi.Object,
                _mockRepository.Object,
                _mockTrackerService.Object,
                _mockConfiguration.Object);

            task.Should().NotBeNull();
        }

        [Fact]
        public void FetchTpWager9WDataTask_ShouldInheritFromCorrectBaseClass()
        {
            // Act & Assert - Should inherit from the correct base class
            _task.Should().BeAssignableTo<FetchDataByBatchForMultiRepoTaskBase<TpWager9WModel, CreateTimeTrackerInfo>>();
        }

        [Fact]
        public async Task GetDataAsync_ShouldCallProduct9WApiWithCorrectParameters()
        {
            // Arrange
            var trackValue = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-2),
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1500;
            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-2), UpdateTime = DateTime.Now.AddHours(-1) },
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-1), UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpWagerAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(wagerData);

            // Act - Use cached reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpWager9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should call API with correct parameters
            _mockProduct9WApi.Verify(x => x.GetTpWagerAsync(
                It.Is<BatchCountParam>(p =>
                    p.CreateTime == trackValue.CreateTime &&
                    p.FromDate == trackValue.UpdateTime &&
                    p.BatchCount == batchCount &&
                    p.ToDate <= DateTime.Now)),
                Times.Once);

            result.Should().BeEquivalentTo(wagerData);
        }

        [Fact]
        public async Task GetDataAsync_ShouldUseCurrentDateTimeAsToDate()
        {
            // Arrange
            var trackValue = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-2),
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1000;
            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now, UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpWagerAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(wagerData);

            // Act - Use cached reflection to call the protected method
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpWager9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should use current datetime as ToDate
            _mockProduct9WApi.Verify(x => x.GetTpWagerAsync(
                It.Is<BatchCountParam>(p =>
                    p.ToDate <= DateTime.Now &&
                    p.ToDate >= DateTime.Now.AddMinutes(-1))), // Allow for small time difference
                Times.Once);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldReturnCorrectCreateTimeTrackerInfo()
        {
            // Arrange
            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-3), UpdateTime = DateTime.Now.AddHours(-2) },
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-2), UpdateTime = DateTime.Now.AddHours(-1) },
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-1), UpdateTime = DateTime.Now }
            };

            // Act - Use cached reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");
            var result = (CreateTimeTrackerInfo)method.Invoke(_task, new object[] { wagerData });

            // Assert - Should return tracking info from the last item
            result.Should().NotBeNull();
            result.CreateTime.Should().Be(wagerData.Last().CreateTime.Value);
            result.UpdateTime.Should().Be(wagerData.Last().UpdateTime.Value);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldThrowException_WhenDataIsEmpty()
        {
            // Arrange
            var emptyData = new List<TpWager9WModel>();

            // Act & Assert - Should throw InvalidOperationException for empty data
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");

            var act = () => method.Invoke(_task, new object[] { emptyData });

            act.Should().Throw<System.Reflection.TargetInvocationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage("TpWager9W cannot determine tracking value: data collection is empty.");
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldThrowException_WhenLastItemCreateTimeIsNull()
        {
            // Arrange
            var wagerDataWithNullCreateTime = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now, UpdateTime = DateTime.Now },
                new TpWager9WModel { CreateTime = null, UpdateTime = DateTime.Now } // Last item has null CreateTime
            };

            // Act & Assert - Should throw InvalidOperationException for null CreateTime
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");

            var act = () => method.Invoke(_task, new object[] { wagerDataWithNullCreateTime });

            act.Should().Throw<System.Reflection.TargetInvocationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage("TpWager9W cannot determine tracking value: last item's CreateTime or UpdateTime is null.");
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldThrowException_WhenLastItemUpdateTimeIsNull()
        {
            // Arrange
            var wagerDataWithNullUpdateTime = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now, UpdateTime = DateTime.Now },
                new TpWager9WModel { CreateTime = DateTime.Now, UpdateTime = null } // Last item has null UpdateTime
            };

            // Act & Assert - Should throw InvalidOperationException for null UpdateTime
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");

            var act = () => method.Invoke(_task, new object[] { wagerDataWithNullUpdateTime });

            act.Should().Throw<System.Reflection.TargetInvocationException>()
                .WithInnerException<InvalidOperationException>()
                .WithMessage("TpWager9W cannot determine tracking value: last item's CreateTime or UpdateTime is null.");
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldReturnAdjustedCreateTimeTrackerInfo()
        {
            // Arrange
            var originalTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-2),
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
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<CreateTimeTrackerInfo>(_task, "GetLastTrackingValueAsync");

            // Assert - Should return adjusted CreateTimeTrackerInfo
            result.Should().NotBeNull();
            result.CreateTime.Should().Be(originalTrackerInfo.CreateTime);
            result.UpdateTime.Should().Be(originalTrackerInfo.UpdateTime.AddMinutes(_fetchServiceSetting.AdjustmentValue));
        }

        [Fact]
        public async Task GetLastTrackingValueAsync_ShouldGetTrackerFromService()
        {
            // Arrange
            var originalTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-2),
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
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<CreateTimeTrackerInfo>(_task, "GetLastTrackingValueAsync");

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
            var originalTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-3),
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
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<CreateTimeTrackerInfo>(_task, "GetLastTrackingValueAsync");

            // Assert - Should correctly deserialize and adjust the tracking info
            result.Should().NotBeNull();
            result.CreateTime.Should().Be(originalTrackerInfo.CreateTime);
            result.UpdateTime.Should().Be(originalTrackerInfo.UpdateTime.AddMinutes(_fetchServiceSetting.AdjustmentValue));
        }

        [Fact]
        public async Task GetDataAsync_ShouldHandleEmptyResults()
        {
            // Arrange
            var trackValue = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-2),
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 1000;
            var emptyWagerData = new List<TpWager9WModel>();

            _mockProduct9WApi
                .Setup(x => x.GetTpWagerAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(emptyWagerData);

            // Act - Use reflection to call the protected method
            var result = await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpWager9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should handle empty results gracefully
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetDataAsync_ShouldCreateBatchCountParamWithAllRequiredProperties()
        {
            // Arrange
            var trackValue = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.Now.AddDays(-3),
                UpdateTime = DateTime.Now.AddDays(-1)
            };
            var batchCount = 2500;
            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now, UpdateTime = DateTime.Now }
            };

            _mockProduct9WApi
                .Setup(x => x.GetTpWagerAsync(It.IsAny<BatchCountParam>()))
                .ReturnsAsync(wagerData);

            // Act - Use reflection to call the protected method
            await ReflectionTestHelper.InvokeCachedProtectedMethodAsync<IEnumerable<TpWager9WModel>>(_task, "GetDataAsync", trackValue, batchCount);

            // Assert - Should create BatchCountParam with all required properties
            _mockProduct9WApi.Verify(x => x.GetTpWagerAsync(
                It.Is<BatchCountParam>(p =>
                    p.CreateTime == trackValue.CreateTime &&
                    p.FromDate == trackValue.UpdateTime &&
                    p.BatchCount == batchCount &&
                    p.ToDate > trackValue.UpdateTime)), // ToDate should be later than FromDate
                Times.Once);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldUseCreateTimeFromLastItem()
        {
            // Arrange
            var createTime1 = DateTime.Now.AddHours(-3);
            var createTime2 = DateTime.Now.AddHours(-2);
            var createTime3 = DateTime.Now.AddHours(-1);

            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = createTime1, UpdateTime = DateTime.Now.AddHours(-3) },
                new TpWager9WModel { CreateTime = createTime2, UpdateTime = DateTime.Now.AddHours(-2) },
                new TpWager9WModel { CreateTime = createTime3, UpdateTime = DateTime.Now.AddHours(-1) }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");
            var result = (CreateTimeTrackerInfo)method.Invoke(_task, new object[] { wagerData });

            // Assert - Should use CreateTime from the last item in the collection
            result.CreateTime.Should().Be(createTime3);
        }

        [Fact]
        public void GetInsertTrackingValue_ShouldUseUpdateTimeFromLastItem()
        {
            // Arrange
            var updateTime1 = DateTime.Now.AddHours(-3);
            var updateTime2 = DateTime.Now.AddHours(-2);
            var updateTime3 = DateTime.Now.AddHours(-1);

            var wagerData = new List<TpWager9WModel>
            {
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-3), UpdateTime = updateTime1 },
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-2), UpdateTime = updateTime2 },
                new TpWager9WModel { CreateTime = DateTime.Now.AddHours(-1), UpdateTime = updateTime3 }
            };

            // Act - Use reflection to call the protected method
            var method = ReflectionTestHelper.GetCachedProtectedMethod(typeof(FetchTpWager9WDataTask), "GetInsertTrackingValue");
            var result = (CreateTimeTrackerInfo)method.Invoke(_task, new object[] { wagerData });

            // Assert - Should use UpdateTime from the last item in the collection
            result.UpdateTime.Should().Be(updateTime3);
        }
    }
}
