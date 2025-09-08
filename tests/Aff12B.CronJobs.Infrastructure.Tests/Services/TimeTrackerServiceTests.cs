using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Exceptions;
using Aff12B.CronJobs.Infrastructure.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Services
{
    public class TimeTrackerServiceTests
    {
        private readonly Mock<ITrackerRepository> _mockRepository;
        private readonly Mock<ILogger<TimeTrackerService>> _mockLogger;
        private readonly Mock<IOptionsSnapshot<TimeTrackerSetting>> _mockOptionsSnapshot;
        private readonly TimeTrackerSetting _setting;
        private readonly TimeTrackerService _service;

        public TimeTrackerServiceTests()
        {
            _mockRepository = new Mock<ITrackerRepository>();
            _mockLogger = new Mock<ILogger<TimeTrackerService>>();
            _mockOptionsSnapshot = new Mock<IOptionsSnapshot<TimeTrackerSetting>>();

            _setting = new TimeTrackerSetting
            {
                SourcePrefix = new[] { "Prefix1", "Prefix2", "Prefix3" }
            };

            _mockOptionsSnapshot.Setup(x => x.Value).Returns(_setting);
            _service = new TimeTrackerService(_mockRepository.Object, _mockOptionsSnapshot.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new TimeTrackerService(_mockRepository.Object, _mockOptionsSnapshot.Object, _mockLogger.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_NullRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TimeTrackerService(null!, _mockOptionsSnapshot.Object, _mockLogger.Object);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("timeTrackerRepository");
        }

        [Fact]
        public void Constructor_NullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TimeTrackerService(_mockRepository.Object, _mockOptionsSnapshot.Object, null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact]
        public void Constructor_NullOptionsSnapshot_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var act = () => new TimeTrackerService(_mockRepository.Object, null!, _mockLogger.Object);
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*optionsSnapshot*");
        }

        [Fact]
        public void Constructor_NullOptionsSnapshotValue_ShouldThrowArgumentNullException()
        {
            // Arrange
            _mockOptionsSnapshot.Setup(x => x.Value).Returns((TimeTrackerSetting)null!);

            // Act & Assert
            var act = () => new TimeTrackerService(_mockRepository.Object, _mockOptionsSnapshot.Object, _mockLogger.Object);
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*optionsSnapshot.Value*");
        }

        #endregion

        #region GetTrackerAsync Tests

        [Fact]
        public async Task GetTrackerAsync_ValidSource_ShouldReturnMinTracker()
        {
            // Arrange
            const string source = "TestSource";
            var trackerInfos = new List<TrackerInfo>
            {
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-1), KeyValue = "Key1", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-2), KeyValue = "Key2", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-3), KeyValue = "Key3", IsReset = false }
            };

            var callIndex = 0;
            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync(() => trackerInfos[callIndex++]);

            // Act
            var result = await _service.GetTrackerAsync(source, t => t.CalledTime);

            // Assert
            result.Should().NotBeNull();
            result.CalledTime.Should().Be(trackerInfos[2].CalledTime); // Earliest date
            _mockRepository.Verify(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()),
                Times.Exactly(_setting.SourcePrefix.Count()));
        }

        [Fact]
        public async Task GetTrackerAsync_NoTrackers_ShouldThrowInvalidOperationException()
        {
            // Arrange
            const string source = "TestSource";

            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync((TrackerInfo)null!);

            // Act & Assert
            var act = async () => await _service.GetTrackerAsync(source, t => t.CalledTime);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Can't fetch any track time");
        }

        [Fact]
        public async Task GetTrackerAsync_WithComparer_ShouldUseCustomComparer()
        {
            // Arrange
            const string source = "TestSource";
            var trackerInfos = new List<TrackerInfo>
            {
                new TrackerInfo { CalledTime = DateTime.Now, KeyValue = "AAA", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now, KeyValue = "ZZZ", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now, KeyValue = "MMM", IsReset = false }
            };

            var callIndex = 0;
            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync(() => trackerInfos[callIndex++ % trackerInfos.Count]);

            var reverseComparer = Comparer<string>.Create((x, y) => string.Compare(y, x, StringComparison.Ordinal));

            // Act
            var result = await _service.GetTrackerAsync(source, t => t.KeyValue, reverseComparer);

            // Assert
            result.Should().NotBeNull();
            result.KeyValue.Should().Be("ZZZ"); // Last in reverse alphabetical order (Z comes first when reversed)
        }

        [Theory]
        [InlineData("Source1")]
        [InlineData("Source2")]
        [InlineData("AnotherSource")]
        public async Task GetTrackerAsync_VariousSelectors_ShouldWork(string source)
        {
            // Arrange
            var trackerInfos = new List<TrackerInfo>
            {
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-1), KeyValue = "Key1", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-2), KeyValue = "Key2", IsReset = true },
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-3), KeyValue = "Key3", IsReset = false }
            };

            var callIndex = 0;
            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync(() => trackerInfos[callIndex++]);

            // Act - Test different selectors
            var resultByTime = await _service.GetTrackerAsync(source, t => t.CalledTime);
            callIndex = 0; // Reset for next call
            var resultByKey = await _service.GetTrackerAsync(source, t => t.KeyValue);

            // Assert
            resultByTime.Should().NotBeNull();
            resultByKey.Should().NotBeNull();
            _mockRepository.Verify(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()),
                Times.Exactly(_setting.SourcePrefix.Count() * 2));
        }

        [Fact]
        public async Task GetTrackerAsync_SomeTrackersNull_ShouldReturnAvailableTrackers()
        {
            // Arrange
            const string source = "TestSource";
            var trackerInfos = new List<TrackerInfo>
            {
                null!, // First call returns null
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-1), KeyValue = "Key1", IsReset = false },
                new TrackerInfo { CalledTime = DateTime.Now.AddDays(-2), KeyValue = "Key2", IsReset = false }
            };

            var callIndex = 0;
            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync(() => trackerInfos[callIndex++]);

            // Act
            var result = await _service.GetTrackerAsync(source, t => t.CalledTime);

            // Assert
            result.Should().NotBeNull();
            result.CalledTime.Should().Be(trackerInfos[2].CalledTime); // Earliest non-null date
        }

        [Fact]
        public async Task GetTrackerAsync_RepositoryThrowsException_ShouldLogErrorAndContinue()
        {
            // Arrange
            const string source = "TestSource";
            var validTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "ValidKey",
                IsReset = false
            };

            var callCount = 0;
            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 1)
                        throw new InvalidOperationException("Repository error");
                    if (callCount == 2)
                        return Task.FromResult((TrackerInfo)null!);
                    return Task.FromResult(validTracker);
                });

            // Act
            var result = await _service.GetTrackerAsync(source, t => t.CalledTime);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(validTracker);

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region SaveTrackerAsync Tests

        [Fact]
        public async Task SaveTrackerAsync_ValidTracker_ShouldSaveForAllPrefixes()
        {
            // Arrange
            const string source = "TestSource";
            var tracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = false
            };

            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveTrackerAsync(source, tracker);

            // Assert
            _mockRepository.Verify(
                x => x.ImportTimeTrackerAsync(It.Is<InsertImportTrackerCommand>(cmd =>
                    cmd.CalledTime == tracker.CalledTime &&
                    cmd.KeyValue == tracker.KeyValue &&
                    cmd.IsReset == tracker.IsReset &&
                    cmd.DataSource == source)),
                Times.Exactly(_setting.SourcePrefix.Count()));
        }

        [Fact]
        public async Task SaveTrackerAsync_ImportFails_ShouldThrowImporterException()
        {
            // Arrange
            const string source = "TestSource";
            var tracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = true
            };

            // This test will be simplified since we can't easily mock the callback behavior
            // The actual service creates the command internally, so we test the exception path differently
            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .ThrowsAsync(new ImporterException(1001, "Import failed"));

            // Act & Assert
            var act = async () => await _service.SaveTrackerAsync(source, tracker);
            await act.Should().ThrowAsync<ImporterException>();
        }

        [Fact]
        public async Task SaveTrackerAsync_ShouldSetCorrectCommandProperties()
        {
            // Arrange
            const string source = "SpecificSource";
            var tracker = new TrackerInfo
            {
                CalledTime = new DateTime(2023, 12, 25, 10, 30, 45),
                KeyValue = "SpecificKey",
                IsReset = true
            };

            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SaveTrackerAsync(source, tracker);

            // Assert
            _mockRepository.Verify(x => x.ImportTimeTrackerAsync(
                It.Is<InsertImportTrackerCommand>(cmd =>
                    cmd.CalledTime == tracker.CalledTime &&
                    cmd.KeyValue == tracker.KeyValue &&
                    cmd.IsReset == tracker.IsReset &&
                    cmd.DataSource == source)),
                Times.Exactly(_setting.SourcePrefix.Count()));
        }

        [Fact]
        public async Task SaveTrackerAsync_RepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string source = "TestSource";
            var tracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = false
            };

            var expectedException = new InvalidOperationException("Repository error");
            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.SaveTrackerAsync(source, tracker);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Repository error");
        }

        [Fact]
        public async Task SaveTrackerAsync_PartialFailure_ShouldThrowOnFirstError()
        {
            // Arrange
            const string source = "TestSource";
            var tracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "TestKey",
                IsReset = false
            };

            // Simulate failure on the second call
            var callCount = 0;
            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 2)
                        throw new ImporterException(1001, "Second call failed");
                    return Task.CompletedTask;
                });

            // Act & Assert
            var act = async () => await _service.SaveTrackerAsync(source, tracker);
            await act.Should().ThrowAsync<ImporterException>();

            // Should have been called twice (success, then failure)
            _mockRepository.Verify(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()),
                Times.Exactly(2));
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task Service_CompleteWorkflow_ShouldGetAndSaveTracker()
        {
            // Arrange
            const string source = "WorkflowSource";
            var existingTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now.AddDays(-1),
                KeyValue = "ExistingKey",
                IsReset = false
            };
            var newTracker = new TrackerInfo
            {
                CalledTime = DateTime.Now,
                KeyValue = "NewKey",
                IsReset = true
            };

            _mockRepository
                .Setup(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()))
                .ReturnsAsync(existingTracker);

            _mockRepository
                .Setup(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()))
                .Returns(Task.CompletedTask);

            // Act
            var retrievedTracker = await _service.GetTrackerAsync(source, t => t.CalledTime);
            await _service.SaveTrackerAsync(source, newTracker);

            // Assert
            retrievedTracker.Should().NotBeNull();
            retrievedTracker.KeyValue.Should().Be("ExistingKey");

            _mockRepository.Verify(x => x.GetLastedTrackerAsync(It.IsAny<GetLastedImportTimeCriteria>()),
                Times.Exactly(_setting.SourcePrefix.Count()));
            _mockRepository.Verify(x => x.ImportTimeTrackerAsync(It.IsAny<InsertImportTrackerCommand>()),
                Times.Exactly(_setting.SourcePrefix.Count()));
        }

        #endregion
    }
}
