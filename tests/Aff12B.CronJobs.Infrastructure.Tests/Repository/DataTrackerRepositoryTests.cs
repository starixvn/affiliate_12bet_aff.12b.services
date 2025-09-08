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
    public class DataTrackerRepositoryTests
    {
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly DataTrackerRepository _repository;

        public DataTrackerRepositoryTests()
        {
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _repository = new DataTrackerRepository(_mockDynamicRepository.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidRepository_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new DataTrackerRepository(_mockDynamicRepository.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new DataTrackerRepository(null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("repo");
        }

        #endregion

        #region FetchDataTracker Tests

        [Fact]
        public async Task FetchDataTracker_ValidCriteria_ShouldCallRepositoryFetchAsync()
        {
            // Arrange
            var criteria = new DataTrackerCriteria("TEST");
            var expectedResult = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 1, StatusId = 1, PrefixSiteName = "Site1", PushDataType = 1, PushDataInfo = "Active" },
                new DataTrackerItem { PushTrackerId = 2, StatusId = 0, PrefixSiteName = "Site2", PushDataType = 2, PushDataInfo = "Inactive" }
            };

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.FetchDataTracker(criteria);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task FetchDataTracker_ShouldCastCriteriaCorrectly()
        {
            // Arrange
            var criteria = new DataTrackerCriteria("TEST");
            var expectedResult = new List<DataTrackerItem>();

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _repository.FetchDataTracker(criteria);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<DataTrackerItem>(
                    It.Is<ICriteria>(c => c == (ICriteria)criteria),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task FetchDataTracker_EmptyResult_ShouldReturnEmptyEnumerable()
        {
            // Arrange
            var criteria = new DataTrackerCriteria("TEST");
            var expectedResult = new List<DataTrackerItem>();

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _repository.FetchDataTracker(criteria);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region UpdateDataTrackerAsync Tests

        [Fact]
        public async Task UpdateDataTrackerAsync_ValidCommand_ShouldCallRepositoryExecuteAsync()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.UpdateDataTrackerAsync(command);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateDataTrackerAsync_ShouldCastCommandCorrectly()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.UpdateDataTrackerAsync(command);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<UpdateDataTrackerCommand>(c => c == (UpdateDataTrackerCommand)command),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateDataTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST");
            var expectedException = new InvalidOperationException("Database error");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.UpdateDataTrackerAsync(command);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        #endregion

        #region InsertDataTrackerAsync Tests

        [Fact]
        public async Task InsertDataTrackerAsync_ValidCommand_ShouldCallRepositoryExecuteAsync()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.InsertDataTrackerAsync(command);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task InsertDataTrackerAsync_ShouldCastCommandCorrectly()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            await _repository.InsertDataTrackerAsync(command);

            // Assert
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(
                    It.Is<InsertDataTrackerCommand>(c => c == (InsertDataTrackerCommand)command),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task InsertDataTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST");
            var expectedException = new InvalidOperationException("Database connection failed");

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _repository.InsertDataTrackerAsync(command);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database connection failed");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Repository_AllOperations_ShouldWorkSequentially()
        {
            // Arrange
            var criteria = new DataTrackerCriteria("TEST");
            var updateCommand = new UpdateDataTrackerCommand("TEST");
            var insertCommand = new InsertDataTrackerCommand("TEST");
            var expectedFetchResult = new List<DataTrackerItem>
            {
                new DataTrackerItem { PushTrackerId = 1, StatusId = 1, PrefixSiteName = "Site1", PushDataType = 1, PushDataInfo = "Active" }
            };

            _mockDynamicRepository
                .Setup(x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFetchResult);

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            _mockDynamicRepository
                .Setup(x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var fetchResult = await _repository.FetchDataTracker(criteria);
            await _repository.UpdateDataTrackerAsync(updateCommand);
            await _repository.InsertDataTrackerAsync(insertCommand);

            // Assert
            fetchResult.Should().BeEquivalentTo(expectedFetchResult);
            _mockDynamicRepository.Verify(
                x => x.FetchAsync<DataTrackerItem>(It.IsAny<ICriteria>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<UpdateDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDynamicRepository.Verify(
                x => x.ExecuteAsync(It.IsAny<InsertDataTrackerCommand>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion
    }
}
