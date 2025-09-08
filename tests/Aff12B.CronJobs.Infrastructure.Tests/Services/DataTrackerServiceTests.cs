using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Services
{
    public class DataTrackerServiceTests
    {
        private readonly Mock<IDataTrackerRepository> _mockRepository;
        private readonly DataTrackerService _service;

        public DataTrackerServiceTests()
        {
            _mockRepository = new Mock<IDataTrackerRepository>();
            _service = new DataTrackerService(_mockRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new DataTrackerService(_mockRepository.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldNotThrowException()
        {
            // Arrange, Act & Assert
            var act = () => new DataTrackerService(null!);
            act.Should().NotThrow();
        }

        #endregion

        #region FetchTrackerAsync Tests

        [Fact]
        public async Task FetchTrackerAsync_MultipleProviders_ShouldReturnCombinedResults()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1", "Provider2" };
            var dataTypes = new[] { 1, 2 };
            var statuses = new[] { 1 };

            var mockData1 = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 1, PushDataType = 1, StatusId = 1, PushDataInfo = "Data1" }
            };
            var mockData2 = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 2, PushDataType = 2, StatusId = 1, PushDataInfo = "Data2" }
            };

            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ReturnsAsync((DataTrackerCriteria criteria) =>
                {
                    if (criteria.PushDataType == 1) return mockData1;
                    if (criteria.PushDataType == 2) return mockData2;
                    return new List<DataTrackerItem>();
                });

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            result.Should().HaveCount(4); // 2 providers × 2 dataTypes × 1 status = 4 calls
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Exactly(4));
        }

        [Fact]
        public async Task FetchTrackerAsync_EmptyProviders_ShouldReturnEmpty()
        {
            // Arrange
            var prefixProviders = new string[0];
            var dataTypes = new[] { 1 };
            var statuses = new[] { 1 };

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Never);
        }

        [Fact]
        public async Task FetchTrackerAsync_EmptyDataTypes_ShouldReturnEmpty()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1" };
            var dataTypes = new int[0];
            var statuses = new[] { 1 };

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Never);
        }

        [Fact]
        public async Task FetchTrackerAsync_EmptyStatuses_ShouldReturnEmpty()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1" };
            var dataTypes = new[] { 1 };
            var statuses = new int[0];

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Never);
        }

        [Fact]
        public async Task FetchTrackerAsync_ShouldCallFetchAndAssignValueForEachCombination()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1", "Provider2" };
            var dataTypes = new[] { 1, 2 };
            var statuses = new[] { 1, 2 };

            var mockData = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 1, PushDataType = 1, StatusId = 1, PushDataInfo = "Test" }
            };

            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ReturnsAsync(mockData);

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            // 2 providers × 2 dataTypes × 2 statuses = 8 calls
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Exactly(8));

            // Verify that PrefixSiteName is assigned correctly
            var resultList = result.ToList();
            resultList.Should().HaveCount(8);
            resultList.Should().OnlyContain(item =>
                item.PrefixSiteName == "Provider1" || item.PrefixSiteName == "Provider2");
        }

        [Theory]
        [InlineData(new[] { "P1" }, new[] { 1 }, new[] { 1 }, 1)]
        [InlineData(new[] { "P1", "P2" }, new[] { 1 }, new[] { 1 }, 2)]
        [InlineData(new[] { "P1" }, new[] { 1, 2 }, new[] { 1 }, 2)]
        [InlineData(new[] { "P1" }, new[] { 1 }, new[] { 1, 2 }, 2)]
        [InlineData(new[] { "P1", "P2" }, new[] { 1, 2 }, new[] { 1, 2 }, 8)]
        public async Task FetchTrackerAsync_VariousParameters_ShouldHandleCorrectly(
            string[] prefixProviders, int[] dataTypes, int[] statuses, int expectedCalls)
        {
            // Arrange
            var mockData = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 1, PushDataType = 1, StatusId = 1, PushDataInfo = "Test" }
            };

            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ReturnsAsync(mockData);

            // Act
            var result = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Exactly(expectedCalls));
            result.Should().HaveCount(expectedCalls);
        }

        [Fact]
        public async Task FetchTrackerAsync_ShouldSetCorrectCriteriaParameters()
        {
            // Arrange
            var prefixProviders = new[] { "TestProvider" };
            var dataTypes = new[] { 5 };
            var statuses = new[] { 3 };

            var mockData = new List<DataTrackerItem>();

            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ReturnsAsync(mockData);

            // Act
            await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);

            // Assert
            _mockRepository.Verify(x => x.FetchDataTracker(
                It.Is<DataTrackerCriteria>(criteria =>
                    criteria.PushDataType == 5 && criteria.StatusId == 3)),
                Times.Once);
        }

        [Fact]
        public async Task FetchTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1" };
            var dataTypes = new[] { 1 };
            var statuses = new[] { 1 };

            var expectedException = new InvalidOperationException("Repository error");
            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Repository error");
        }

        #endregion

        #region UpdateTrackerStatusAsync Tests

        [Fact]
        public async Task UpdateTrackerStatusAsync_ValidItem_ShouldUpdateStatus()
        {
            // Arrange
            var item = new DataTrackerItem
            {
                PushTrackerId = 123,
                PrefixSiteName = "TestSite",
                StatusId = 1,
                PushDataType = 2,
                PushDataInfo = "TestData"
            };
            var newStatus = 2;

            _mockRepository
                .Setup(x => x.UpdateDataTrackerAsync(It.IsAny<UpdateDataTrackerCommand>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateTrackerStatusAsync(item, newStatus);

            // Assert
            _mockRepository.Verify(x => x.UpdateDataTrackerAsync(
                It.Is<UpdateDataTrackerCommand>(cmd =>
                    cmd.StatusId == newStatus &&
                    cmd.PushTrackerId == item.PushTrackerId)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTrackerStatusAsync_ShouldPassCorrectParameters()
        {
            // Arrange
            var item = new DataTrackerItem
            {
                PushTrackerId = 456,
                PrefixSiteName = "AnotherSite",
                StatusId = 0,
                PushDataType = 3,
                PushDataInfo = "AnotherData"
            };
            var newStatus = 5;

            _mockRepository
                .Setup(x => x.UpdateDataTrackerAsync(It.IsAny<UpdateDataTrackerCommand>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateTrackerStatusAsync(item, newStatus);

            // Assert
            _mockRepository.Verify(x => x.UpdateDataTrackerAsync(
                It.Is<UpdateDataTrackerCommand>(cmd =>
                    cmd.StatusId == newStatus && cmd.PushTrackerId == item.PushTrackerId)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTrackerStatusAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var item = new DataTrackerItem
            {
                PushTrackerId = 789,
                PrefixSiteName = "ErrorSite",
                StatusId = 1,
                PushDataType = 1,
                PushDataInfo = "ErrorData"
            };
            var newStatus = 3;

            var expectedException = new InvalidOperationException("Update failed");
            _mockRepository
                .Setup(x => x.UpdateDataTrackerAsync(It.IsAny<UpdateDataTrackerCommand>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateTrackerStatusAsync(item, newStatus);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Update failed");
        }

        #endregion

        #region InsertDataTrackerAsync Tests

        [Fact]
        public async Task InsertDataTrackerAsync_ValidCommand_ShouldInsertTracker()
        {
            // Arrange
            var mockCommand = new Mock<IDataTracker>();

            _mockRepository
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.InsertDataTrackerAsync(mockCommand.Object);

            // Assert
            _mockRepository.Verify(x => x.InsertDataTrackerAsync(mockCommand.Object), Times.Once);
        }

        [Fact]
        public async Task InsertDataTrackerAsync_ShouldPassCommandDirectly()
        {
            // Arrange
            var mockCommand = new Mock<IDataTracker>();
            IDataTracker capturedCommand = null;

            _mockRepository
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()))
                .Returns(Task.CompletedTask)
                .Callback<IDataTracker>(cmd => capturedCommand = cmd);

            // Act
            await _service.InsertDataTrackerAsync(mockCommand.Object);

            // Assert
            capturedCommand.Should().BeSameAs(mockCommand.Object);
        }

        [Fact]
        public async Task InsertDataTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var mockCommand = new Mock<IDataTracker>();
            var expectedException = new InvalidOperationException("Insert failed");

            _mockRepository
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.InsertDataTrackerAsync(mockCommand.Object);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Insert failed");
        }

        [Fact]
        public async Task InsertDataTrackerAsync_NullCommand_ShouldPassToRepository()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.InsertDataTrackerAsync(null!);

            // Assert
            _mockRepository.Verify(x => x.InsertDataTrackerAsync(null!), Times.Once);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Service_CompleteWorkflow_ShouldExecuteAllOperations()
        {
            // Arrange
            var prefixProviders = new[] { "Provider1" };
            var dataTypes = new[] { 1 };
            var statuses = new[] { 1 };

            var fetchedData = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 100, PushDataType = 1, StatusId = 1, PushDataInfo = "Test" }
            };

            var mockCommand = new Mock<IDataTracker>();

            _mockRepository
                .Setup(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()))
                .ReturnsAsync(fetchedData);

            _mockRepository
                .Setup(x => x.UpdateDataTrackerAsync(It.IsAny<UpdateDataTrackerCommand>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()))
                .Returns(Task.CompletedTask);

            // Act
            var fetchResult = await _service.FetchTrackerAsync(prefixProviders, dataTypes, statuses);
            var firstItem = fetchResult.First();
            await _service.UpdateTrackerStatusAsync(firstItem, 2);
            await _service.InsertDataTrackerAsync(mockCommand.Object);

            // Assert
            fetchResult.Should().HaveCount(1);
            firstItem.PrefixSiteName.Should().Be("Provider1");

            _mockRepository.Verify(x => x.FetchDataTracker(It.IsAny<DataTrackerCriteria>()), Times.Once);
            _mockRepository.Verify(x => x.UpdateDataTrackerAsync(It.IsAny<UpdateDataTrackerCommand>()), Times.Once);
            _mockRepository.Verify(x => x.InsertDataTrackerAsync(It.IsAny<IDataTracker>()), Times.Once);
        }

        #endregion
    }
}
