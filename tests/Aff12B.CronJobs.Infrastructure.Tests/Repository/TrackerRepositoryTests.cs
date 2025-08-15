using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Infrastructure.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Repository
{
    public class TrackerRepositoryTests
    {
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly TrackerRepository _repository;

        public TrackerRepositoryTests()
        {
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _repository = new TrackerRepository(_mockDynamicRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new TrackerRepository(_mockDynamicRepository.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TrackerRepository(null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("repo");
        }

        #endregion

        #region GetLastedTrackerAsync Tests

        [Fact]
        public async Task GetLastedTrackerAsync_ValidSource_ShouldReturnTracker()
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");
            var expectedTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = false
            };

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTracker);

            // Act
            var result = await _repository.GetLastedTrackerAsync(source);

            // Assert
            result.Should().BeEquivalentTo(expectedTracker);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetLastedTrackerAsync_ShouldCastSourceCorrectly()
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");
            var expectedTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now.AddDays(-1),
                KeyValue = "AnotherKey",
                IsReset = true
            };

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTracker);

            // Act
            var result = await _repository.GetLastedTrackerAsync(source);

            // Assert
            result.Should().BeEquivalentTo(expectedTracker);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<TrackerInfo>(
                    It.Is<GetLastedImportTimeCriteria>(c => c == (GetLastedImportTimeCriteria)source),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetLastedTrackerAsync_RepositoryReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TrackerInfo)null!);

            // Act
            var result = await _repository.GetLastedTrackerAsync(source);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetLastedTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");
            var expectedException = new InvalidOperationException("Database connection error");

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.GetLastedTrackerAsync(source);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database connection error");
        }

        #endregion

        #region ImportTimeTrackerAsync Tests

        [Fact]
        public async Task ImportTimeTrackerAsync_ValidTracker_ShouldCallRepository()
        {
            // Arrange
            var tracker = new InsertImportTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.ImportTimeTrackerAsync(tracker);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ImportTimeTrackerAsync_ShouldCastTrackerCorrectly()
        {
            // Arrange
            var tracker = new InsertImportTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.ImportTimeTrackerAsync(tracker);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertImportTrackerCommand>(c => c == (InsertImportTrackerCommand)tracker),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ImportTimeTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var tracker = new InsertImportTrackerCommand("TEST");
            var expectedException = new InvalidOperationException("Insert operation failed");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.ImportTimeTrackerAsync(tracker);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Insert operation failed");
        }

        [Fact]
        public async Task ImportTimeTrackerAsync_MultipleCalls_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var tracker1 = new InsertImportTrackerCommand("TEST1");
            var tracker2 = new InsertImportTrackerCommand("TEST2");
            var tracker3 = new InsertImportTrackerCommand("TEST3");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.ImportTimeTrackerAsync(tracker1);
            await _repository.ImportTimeTrackerAsync(tracker2);
            await _repository.ImportTimeTrackerAsync(tracker3);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Repository_GetAndImportSequence_ShouldWorkCorrectly()
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");
            var tracker = new InsertImportTrackerCommand("TEST");
            var expectedTrackerInfo = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = false
            };

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTrackerInfo);

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var retrievedTracker = await _repository.GetLastedTrackerAsync(source);
            await _repository.ImportTimeTrackerAsync(tracker);

            // Assert
            retrievedTracker.Should().BeEquivalentTo(expectedTrackerInfo);
            _mockDynamicRepository.Verify(
                x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertImportTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData("Key1")]
        [InlineData("Key2")]
        [InlineData("Key3")]
        public async Task GetLastedTrackerAsync_VariousKeys_ShouldHandleAllCorrectly(string keyValue)
        {
            // Arrange
            var source = new GetLastedImportTimeCriteria("TEST");
            var expectedTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = keyValue,
                IsReset = false
            };

            _mockDynamicRepository
                .Setup(x => x.GetAsync<TrackerInfo>(It.IsAny<GetLastedImportTimeCriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTracker);

            // Act
            var result = await _repository.GetLastedTrackerAsync(source);

            // Assert
            result.Should().BeEquivalentTo(expectedTracker);
            result.KeyValue.Should().Be(keyValue);
        }

        #endregion
    }
}
