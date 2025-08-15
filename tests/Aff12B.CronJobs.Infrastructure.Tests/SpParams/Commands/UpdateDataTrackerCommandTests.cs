using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams.Commands
{
    /// <summary>
    /// Comprehensive unit tests for UpdateDataTrackerCommand class.
    /// Business-focused scenarios for data tracker update commands.
    /// Covers command validation, property handling, and status update scenarios.
    /// </summary>
    public class UpdateDataTrackerCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithPrefixProvider()
        {
            // Arrange & Act
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldAcceptVariousPrefixProviders()
        {
            // Arrange - Different business prefixes
            var prefixes = new[] { "12B", "CASINO_A", "SPORTS_B", "LIVE_C", "VIP_SITE" };

            // Act & Assert - All should be created successfully
            foreach (var prefix in prefixes)
            {
                var command = new UpdateDataTrackerCommand(prefix);
                command.Should().NotBeNull();
            }
        }

        [Fact]
        public void PushTrackerId_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");
            var expectedTrackerId = 987654321L;

            // Act
            command.PushTrackerId = expectedTrackerId;

            // Assert
            command.PushTrackerId.Should().Be(expectedTrackerId);
        }

        [Fact]
        public void StatusId_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");
            var expectedStatusId = 2; // Processing status

            // Act
            command.StatusId = expectedStatusId;

            // Assert
            command.StatusId.Should().Be(expectedStatusId);
        }

        [Fact]
        public void ErrCode_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");
            var expectedErrCode = 404;

            // Act
            command.ErrCode = expectedErrCode;

            // Assert
            command.ErrCode.Should().Be(expectedErrCode);
        }

        [Fact]
        public void ErrMsg_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");
            var expectedErrMsg = "Update failed: Record not found";

            // Act
            command.ErrMsg = expectedErrMsg;

            // Assert
            command.ErrMsg.Should().Be(expectedErrMsg);
        }

        [Fact]
        public void PushDataType_ShouldReturnDefault()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");

            // Act
            var pushDataType = command.PushDataType;

            // Assert
            pushDataType.Should().Be(default(int));
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");

            // Act - Multiple calls
            var result1 = command.IsValid();
            var result2 = command.IsValid();
            var result3 = command.IsValid();

            // Assert - Should always return true
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldImplementRequiredInterfaces()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("TEST_PREFIX");

            // Act & Assert - Should implement required interfaces
            command.Should().BeAssignableTo<IdenticalSuffixNonQueryCommand>();
            command.Should().BeAssignableTo<INonQueryCommand>();
            command.Should().BeAssignableTo<IDataTracker>();
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportSuccessfulUpdateScenario()
        {
            // Arrange - Successful update scenario
            var command = new UpdateDataTrackerCommand("12B_CASINO");

            // Act - Set up successful data tracker update
            command.PushTrackerId = 123456789L;
            command.StatusId = 1; // Completed status
            command.ErrCode = 0; // No error
            command.ErrMsg = string.Empty;

            // Assert - Should be ready for successful update
            command.PushTrackerId.Should().Be(123456789L);
            command.StatusId.Should().Be(1);
            command.ErrCode.Should().Be(0);
            command.ErrMsg.Should().BeEmpty();
            command.PushDataType.Should().Be(default(int));
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportErrorUpdateScenario()
        {
            // Arrange - Error update scenario
            var command = new UpdateDataTrackerCommand("SPORTS_API");

            // Act - Set up error status update
            command.PushTrackerId = 987654321L;
            command.StatusId = -1; // Error status
            command.ErrCode = 500; // Server error
            command.ErrMsg = "Database connection timeout during update operation";

            // Assert - Should capture error update details
            command.PushTrackerId.Should().Be(987654321L);
            command.StatusId.Should().Be(-1);
            command.ErrCode.Should().Be(500);
            command.ErrMsg.Should().Contain("timeout");
            command.IsValid().Should().BeTrue(); // Command itself is still valid
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportProcessingStatusScenario()
        {
            // Arrange - Processing status scenario
            var command = new UpdateDataTrackerCommand("LIVE_CASINO");

            // Act - Set up processing status update
            command.PushTrackerId = 555666777L;
            command.StatusId = 2; // Processing status
            command.ErrCode = 0;
            command.ErrMsg = "Data processing in progress - 75% complete";

            // Assert - Should handle processing status
            command.PushTrackerId.Should().Be(555666777L);
            command.StatusId.Should().Be(2);
            command.ErrCode.Should().Be(0);
            command.ErrMsg.Should().Contain("processing in progress");
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldHandleUnicodeContent()
        {
            // Arrange - Unicode content scenario
            var command = new UpdateDataTrackerCommand("ASIA_SITE");

            // Act - Set up with unicode error message
            command.ErrMsg = "更新失败_Update_Failed_アップデート失敗";

            // Assert - Should handle unicode content
            command.ErrMsg.Should().Contain("更新失败");
            command.ErrMsg.Should().Contain("Update_Failed");
            command.ErrMsg.Should().Contain("アップデート失敗");
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportHighVolumeTrackerIds()
        {
            // Arrange - High volume tracker ID scenario
            var command = new UpdateDataTrackerCommand("HIGH_VOLUME_SITE");

            // Act - Set up with very large tracker ID
            command.PushTrackerId = long.MaxValue;
            command.StatusId = 1;
            command.ErrCode = 0;

            // Assert - Should handle large tracker IDs
            command.PushTrackerId.Should().Be(long.MaxValue);
            command.StatusId.Should().Be(1);
            command.ErrCode.Should().Be(0);
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldHandleEdgeCaseValues()
        {
            // Arrange - Edge case values
            var command = new UpdateDataTrackerCommand("EDGE_CASE_TEST");

            // Act - Set edge case values
            command.PushTrackerId = 0L;
            command.StatusId = int.MinValue;
            command.ErrCode = int.MaxValue;
            command.ErrMsg = null!;

            // Assert - Should handle edge cases
            command.PushTrackerId.Should().Be(0L);
            command.StatusId.Should().Be(int.MinValue);
            command.ErrCode.Should().Be(int.MaxValue);
            command.ErrMsg.Should().BeNull();
            command.PushDataType.Should().Be(default(int));
            command.IsValid().Should().BeTrue(); // Should still be valid
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportMultipleStatusTypes()
        {
            // Arrange - Multiple status type scenarios
            var statusTypes = new[]
            {
                (Status: 0, Description: "Pending"),
                (Status: 1, Description: "Completed"),
                (Status: 2, Description: "Processing"),
                (Status: -1, Description: "Failed"),
                (Status: 3, Description: "Cancelled")
            };

            foreach (var statusType in statusTypes)
            {
                // Act
                var command = new UpdateDataTrackerCommand("MULTI_STATUS_SITE");
                command.PushTrackerId = 12345L + statusType.Status;
                command.StatusId = statusType.Status;
                command.ErrMsg = $"Status: {statusType.Description}";

                // Assert
                command.StatusId.Should().Be(statusType.Status);
                command.ErrMsg.Should().Contain(statusType.Description);
                command.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldMaintainStateConsistency()
        {
            // Arrange
            var command = new UpdateDataTrackerCommand("CONSISTENCY_TEST");

            // Act - Set initial state
            command.PushTrackerId = 12345L;
            command.StatusId = 1;
            command.ErrCode = 0;
            command.ErrMsg = "Success";

            var initialTrackerId = command.PushTrackerId;
            var initialStatusId = command.StatusId;
            var initialErrCode = command.ErrCode;
            var initialErrMsg = command.ErrMsg;
            var initialPushDataType = command.PushDataType;

            // Perform multiple operations
            for (int i = 0; i < 5; i++)
            {
                command.IsValid();
            }

            // Assert - State should remain consistent
            command.PushTrackerId.Should().Be(initialTrackerId);
            command.StatusId.Should().Be(initialStatusId);
            command.ErrCode.Should().Be(initialErrCode);
            command.ErrMsg.Should().Be(initialErrMsg);
            command.PushDataType.Should().Be(initialPushDataType);
        }

        [Fact]
        public async Task UpdateDataTrackerCommand_ShouldSupportConcurrentAccess()
        {
            // Arrange - Concurrent access scenario
            var command = new UpdateDataTrackerCommand("CONCURRENT_TEST");
            var tasks = new List<Task<bool>>();

            // Act - Multiple concurrent validation calls
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => command.IsValid()));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All concurrent calls should return true
            results.Should().HaveCount(20);
            results.Should().AllSatisfy(result => result.Should().BeTrue());
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldSupportBatchUpdateScenario()
        {
            // Arrange - Batch update scenario
            var trackerIds = new[] { 1001L, 1002L, 1003L, 1004L, 1005L };
            var commands = new List<UpdateDataTrackerCommand>();

            // Act - Create batch of update commands
            foreach (var trackerId in trackerIds)
            {
                var command = new UpdateDataTrackerCommand("BATCH_SITE");
                command.PushTrackerId = trackerId;
                command.StatusId = 1; // Mark as completed
                command.ErrCode = 0;
                command.ErrMsg = $"Batch update for tracker {trackerId}";
                commands.Add(command);
            }

            // Assert - All commands should be valid and ready for batch update
            commands.Should().HaveCount(5);
            commands.Should().AllSatisfy(cmd =>
            {
                cmd.StatusId.Should().Be(1);
                cmd.ErrCode.Should().Be(0);
                cmd.IsValid().Should().BeTrue();
            });
        }

        [Fact]
        public void UpdateDataTrackerCommand_ShouldDifferentiateFromInsertCommand()
        {
            // Arrange - Compare with insert command behavior
            var updateCommand = new UpdateDataTrackerCommand("COMPARISON_TEST");
            var insertCommand = new InsertDataTrackerCommand("COMPARISON_TEST");

            // Act
            updateCommand.PushTrackerId = 12345L;
            insertCommand.PushTrackerId = 12345L;

            // Assert - Should have different behaviors for PushDataType
            updateCommand.PushDataType.Should().Be(default(int)); // Returns default
            insertCommand.StatusId.Should().NotBe(default(int)); // Has specific status

            // Both should be valid
            updateCommand.IsValid().Should().BeTrue();
            insertCommand.IsValid().Should().BeTrue();
        }
    }
}
