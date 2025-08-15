using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.Constants;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams.Commands
{
    /// <summary>
    /// Comprehensive unit tests for InsertDataTrackerCommand class.
    /// Business-focused scenarios for data tracker insertion commands.
    /// Covers command validation, property handling, and database operation scenarios.
    /// </summary>
    public class InsertDataTrackerCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithPrefixProvider()
        {
            // Arrange & Act
            var command = new InsertDataTrackerCommand("TEST_PREFIX");

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
                var command = new InsertDataTrackerCommand(prefix);
                command.Should().NotBeNull();
            }
        }

        [Fact]
        public void PushDataType_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");
            var expectedDataType = 5;

            // Act
            command.PushDataType = expectedDataType;

            // Assert
            command.PushDataType.Should().Be(expectedDataType);
        }

        [Fact]
        public void PushDataInfo_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");
            var expectedDataInfo = "Customer data batch #12345";

            // Act
            command.PushDataInfo = expectedDataInfo;

            // Assert
            command.PushDataInfo.Should().Be(expectedDataInfo);
        }

        [Fact]
        public void PushTrackerId_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");
            var expectedTrackerId = 987654321L;

            // Act
            command.PushTrackerId = expectedTrackerId;

            // Assert
            command.PushTrackerId.Should().Be(expectedTrackerId);
        }

        [Fact]
        public void ErrCode_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");
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
            var command = new InsertDataTrackerCommand("TEST_PREFIX");
            var expectedErrMsg = "Data validation failed: Invalid customer ID";

            // Act
            command.ErrMsg = expectedErrMsg;

            // Assert
            command.ErrMsg.Should().Be(expectedErrMsg);
        }

        [Fact]
        public void StatusId_ShouldReturnSuccessStatus()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");

            // Act
            var statusId = command.StatusId;

            // Assert
            statusId.Should().Be(DataTrackerStatus.Success);
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");

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
        public void InsertDataTrackerCommand_ShouldImplementRequiredInterfaces()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("TEST_PREFIX");

            // Act & Assert - Should implement required interfaces
            command.Should().BeAssignableTo<IdenticalSuffixNonQueryCommand>();
            command.Should().BeAssignableTo<INonQueryCommand>();
            command.Should().BeAssignableTo<IDataTracker>();
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldSupportBusinessDataTrackingScenario()
        {
            // Arrange - Business data tracking scenario
            var command = new InsertDataTrackerCommand("12B_CASINO");

            // Act - Set up data tracking for customer import
            command.PushDataType = 1; // Customer data type
            command.PushDataInfo = "Imported 2500 customer records from API batch #789";
            command.PushTrackerId = 123456789L;
            command.ErrCode = 0; // Success
            command.ErrMsg = string.Empty;

            // Assert - Should be ready for database insertion
            command.PushDataType.Should().Be(1);
            command.PushDataInfo.Should().Contain("2500 customer records");
            command.PushTrackerId.Should().Be(123456789L);
            command.ErrCode.Should().Be(0);
            command.ErrMsg.Should().BeEmpty();
            command.StatusId.Should().Be(DataTrackerStatus.Success);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldSupportErrorTrackingScenario()
        {
            // Arrange - Error tracking scenario
            var command = new InsertDataTrackerCommand("SPORTS_API");

            // Act - Set up error tracking for failed import
            command.PushDataType = 3; // Bet data type
            command.PushDataInfo = "Failed to import bet records: API timeout";
            command.PushTrackerId = 987654321L;
            command.ErrCode = 500; // Server error
            command.ErrMsg = "API timeout after 30 seconds - retry required";

            // Assert - Should capture error details
            command.PushDataType.Should().Be(3);
            command.PushDataInfo.Should().Contain("Failed to import");
            command.PushTrackerId.Should().Be(987654321L);
            command.ErrCode.Should().Be(500);
            command.ErrMsg.Should().Contain("timeout");
            command.StatusId.Should().Be(DataTrackerStatus.Success); // Command itself is valid
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldHandleUnicodeContent()
        {
            // Arrange - Unicode content scenario
            var command = new InsertDataTrackerCommand("ASIA_SITE");

            // Act - Set up with unicode content
            command.PushDataInfo = "导入客户数据_Customer_Import_顧客データ";
            command.ErrMsg = "错误信息_Error_Message_エラーメッセージ";

            // Assert - Should handle unicode content
            command.PushDataInfo.Should().Contain("导入客户数据");
            command.PushDataInfo.Should().Contain("Customer_Import");
            command.PushDataInfo.Should().Contain("顧客データ");
            command.ErrMsg.Should().Contain("错误信息");
            command.ErrMsg.Should().Contain("エラーメッセージ");
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldSupportHighVolumeDataTracking()
        {
            // Arrange - High volume data tracking
            var command = new InsertDataTrackerCommand("HIGH_VOLUME_SITE");

            // Act - Set up for large data batch
            command.PushDataType = 2; // Transaction data
            command.PushDataInfo = "Processed 1,000,000 transactions in batch operation";
            command.PushTrackerId = long.MaxValue; // Very large tracker ID
            command.ErrCode = 0;

            // Assert - Should handle high volume scenarios
            command.PushDataType.Should().Be(2);
            command.PushDataInfo.Should().Contain("1,000,000 transactions");
            command.PushTrackerId.Should().Be(long.MaxValue);
            command.ErrCode.Should().Be(0);
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldHandleEdgeCaseValues()
        {
            // Arrange - Edge case values
            var command = new InsertDataTrackerCommand("EDGE_CASE_TEST");

            // Act - Set edge case values
            command.PushDataType = int.MinValue;
            command.PushDataInfo = string.Empty;
            command.PushTrackerId = 0L;
            command.ErrCode = int.MaxValue;
            command.ErrMsg = null!;

            // Assert - Should handle edge cases
            command.PushDataType.Should().Be(int.MinValue);
            command.PushDataInfo.Should().BeEmpty();
            command.PushTrackerId.Should().Be(0L);
            command.ErrCode.Should().Be(int.MaxValue);
            command.ErrMsg.Should().BeNull();
            command.IsValid().Should().BeTrue(); // Should still be valid
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldSupportMultipleDataTypes()
        {
            // Arrange - Multiple data type scenarios
            var dataTypes = new[]
            {
                (Type: 1, Description: "Customer Data"),
                (Type: 2, Description: "Transaction Data"),
                (Type: 3, Description: "Bet Data"),
                (Type: 4, Description: "Bonus Data"),
                (Type: 5, Description: "Withdrawal Data")
            };

            foreach (var dataType in dataTypes)
            {
                // Act
                var command = new InsertDataTrackerCommand("MULTI_TYPE_SITE");
                command.PushDataType = dataType.Type;
                command.PushDataInfo = $"Processing {dataType.Description}";

                // Assert
                command.PushDataType.Should().Be(dataType.Type);
                command.PushDataInfo.Should().Contain(dataType.Description);
                command.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void InsertDataTrackerCommand_ShouldMaintainStateConsistency()
        {
            // Arrange
            var command = new InsertDataTrackerCommand("CONSISTENCY_TEST");

            // Act - Set initial state
            command.PushDataType = 1;
            command.PushDataInfo = "Initial data";
            command.PushTrackerId = 12345L;
            command.ErrCode = 0;
            command.ErrMsg = "No error";

            var initialType = command.PushDataType;
            var initialInfo = command.PushDataInfo;
            var initialTrackerId = command.PushTrackerId;
            var initialErrCode = command.ErrCode;
            var initialErrMsg = command.ErrMsg;
            var initialStatusId = command.StatusId;

            // Perform multiple operations
            for (int i = 0; i < 5; i++)
            {
                command.IsValid();
            }

            // Assert - State should remain consistent
            command.PushDataType.Should().Be(initialType);
            command.PushDataInfo.Should().Be(initialInfo);
            command.PushTrackerId.Should().Be(initialTrackerId);
            command.ErrCode.Should().Be(initialErrCode);
            command.ErrMsg.Should().Be(initialErrMsg);
            command.StatusId.Should().Be(initialStatusId);
        }

        [Fact]
        public async Task InsertDataTrackerCommand_ShouldSupportConcurrentAccess()
        {
            // Arrange - Concurrent access scenario
            var command = new InsertDataTrackerCommand("CONCURRENT_TEST");
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
    }
}
