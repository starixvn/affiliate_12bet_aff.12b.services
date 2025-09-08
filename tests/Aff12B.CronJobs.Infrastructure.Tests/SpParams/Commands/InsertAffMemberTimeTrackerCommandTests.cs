using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams.Commands
{
    /// <summary>
    /// Comprehensive unit tests for InsertAffMemberTimeTrackerCommand class.
    /// Business-focused scenarios for affiliate member time tracking commands.
    /// Covers command validation, time tracking, and affiliate member import scenarios.
    /// </summary>
    public class InsertAffMemberTimeTrackerCommandTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithPrefixProviderAndSpName()
        {
            // Arrange & Act
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestStoredProc");

            // Assert
            command.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ShouldAcceptVariousPrefixProvidersAndSpNames()
        {
            // Arrange - Different business scenarios
            var scenarios = new[]
            {
                (Prefix: "12B", SpName: "sp_ImportAffMembers"),
                (Prefix: "CASINO_A", SpName: "sp_ProcessAffiliates"),
                (Prefix: "SPORTS_B", SpName: "sp_UpdateMemberTime"),
                (Prefix: "VIP_SITE", SpName: "sp_TrackVipMembers")
            };

            // Act & Assert - All should be created successfully
            foreach (var scenario in scenarios)
            {
                var command = new InsertAffMemberTimeTrackerCommand(scenario.Prefix, scenario.SpName);
                command.Should().NotBeNull();
                command.SName.Should().Be(scenario.SpName);
            }
        }

        [Fact]
        public void LastImportedTime_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");
            var expectedTime = new DateTime(2024, 1, 15, 10, 30, 45);

            // Act
            command.LastImportedTime = expectedTime;

            // Assert
            command.LastImportedTime.Should().Be(expectedTime);
        }

        [Fact]
        public void SName_ShouldReturnStoredProcedureName()
        {
            // Arrange
            var expectedSpName = "sp_ImportAffiliateMembers";
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", expectedSpName);

            // Act
            var spName = command.SName;

            // Assert
            spName.Should().Be(expectedSpName);
        }

        [Fact]
        public void ErrCode_ShouldGetAndSetCorrectly()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");
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
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");
            var expectedErrMsg = "Time tracker insertion failed";

            // Act
            command.ErrMsg = expectedErrMsg;

            // Assert
            command.ErrMsg.Should().Be(expectedErrMsg);
        }

        [Fact]
        public void ErrMsg_ShouldDefaultToEmptyString()
        {
            // Arrange & Act
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");

            // Assert
            command.ErrMsg.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");

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
        public void InsertAffMemberTimeTrackerCommand_ShouldImplementRequiredInterfaces()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("TEST_PREFIX", "TestSP");

            // Act & Assert - Should implement required interfaces
            command.Should().BeAssignableTo<IdenticalSuffixNonQueryCommand>();
            command.Should().BeAssignableTo<IInsertAffMemberTimeTracker>();
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldSupportAffiliateImportScenario()
        {
            // Arrange - Affiliate member import scenario
            var command = new InsertAffMemberTimeTrackerCommand("12B_AFF", "sp_ImportAffiliateMembers");
            var importTime = new DateTime(2024, 1, 15, 14, 30, 0);

            // Act - Set up successful affiliate import tracking
            command.LastImportedTime = importTime;
            command.ErrCode = 0; // Success
            command.ErrMsg = string.Empty;

            // Assert - Should be ready for affiliate time tracking
            command.LastImportedTime.Should().Be(importTime);
            command.SName.Should().Be("sp_ImportAffiliateMembers");
            command.ErrCode.Should().Be(0);
            command.ErrMsg.Should().BeEmpty();
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldSupportErrorTrackingScenario()
        {
            // Arrange - Error tracking scenario
            var command = new InsertAffMemberTimeTrackerCommand("CASINO_AFF", "sp_ProcessAffMembers");
            var failedImportTime = new DateTime(2024, 1, 15, 16, 45, 30);

            // Act - Set up error tracking for failed import
            command.LastImportedTime = failedImportTime;
            command.ErrCode = 500; // Server error
            command.ErrMsg = "Database timeout during affiliate member import";

            // Assert - Should capture error details
            command.LastImportedTime.Should().Be(failedImportTime);
            command.SName.Should().Be("sp_ProcessAffMembers");
            command.ErrCode.Should().Be(500);
            command.ErrMsg.Should().Contain("timeout");
            command.IsValid().Should().BeTrue(); // Command itself is valid
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldHandleRecentImportTime()
        {
            // Arrange - Recent import time scenario
            var command = new InsertAffMemberTimeTrackerCommand("LIVE_AFF", "sp_TrackLiveAffMembers");
            var recentTime = DateTime.Now.AddMinutes(-5); // 5 minutes ago

            // Act
            command.LastImportedTime = recentTime;

            // Assert - Should handle recent timestamps
            command.LastImportedTime.Should().BeCloseTo(recentTime, TimeSpan.FromSeconds(1));
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldHandleHistoricalImportTime()
        {
            // Arrange - Historical import time scenario
            var command = new InsertAffMemberTimeTrackerCommand("SPORTS_AFF", "sp_ImportSportsAffMembers");
            var historicalTime = new DateTime(2020, 1, 1, 0, 0, 0); // Historical date

            // Act
            command.LastImportedTime = historicalTime;

            // Assert - Should handle historical timestamps
            command.LastImportedTime.Should().Be(historicalTime);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldHandleFutureImportTime()
        {
            // Arrange - Future import time scenario (edge case)
            var command = new InsertAffMemberTimeTrackerCommand("VIP_AFF", "sp_TrackVipAffMembers");
            var futureTime = DateTime.Now.AddDays(1); // Tomorrow

            // Act
            command.LastImportedTime = futureTime;

            // Assert - Should handle future timestamps (even if unusual)
            command.LastImportedTime.Should().Be(futureTime);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldHandleUnicodeContent()
        {
            // Arrange - Unicode content scenario
            var command = new InsertAffMemberTimeTrackerCommand("ASIA_AFF", "sp_导入会员_ImportMembers");

            // Act - Set up with unicode error message
            command.ErrMsg = "导入失败_Import_Failed_インポート失敗";

            // Assert - Should handle unicode content
            command.SName.Should().Contain("导入会员");
            command.ErrMsg.Should().Contain("导入失败");
            command.ErrMsg.Should().Contain("インポート失敗");
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldSupportMultipleAffiliatePrograms()
        {
            // Arrange - Multiple affiliate program scenarios
            var programs = new[]
            {
                (Prefix: "CASINO_AFF", SpName: "sp_ImportCasinoAffMembers"),
                (Prefix: "SPORTS_AFF", SpName: "sp_ImportSportsAffMembers"),
                (Prefix: "POKER_AFF", SpName: "sp_ImportPokerAffMembers"),
                (Prefix: "LIVE_AFF", SpName: "sp_ImportLiveAffMembers"),
                (Prefix: "VIP_AFF", SpName: "sp_ImportVipAffMembers")
            };

            var importTime = new DateTime(2024, 1, 15, 12, 0, 0);

            foreach (var program in programs)
            {
                // Act
                var command = new InsertAffMemberTimeTrackerCommand(program.Prefix, program.SpName);
                command.LastImportedTime = importTime;
                command.ErrCode = 0;

                // Assert
                command.SName.Should().Be(program.SpName);
                command.LastImportedTime.Should().Be(importTime);
                command.ErrCode.Should().Be(0);
                command.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldHandleEdgeCaseValues()
        {
            // Arrange - Edge case values
            var command = new InsertAffMemberTimeTrackerCommand("EDGE_CASE", "");

            // Act - Set edge case values
            command.LastImportedTime = DateTime.MinValue;
            command.ErrCode = int.MaxValue;
            command.ErrMsg = null!;

            // Assert - Should handle edge cases
            command.LastImportedTime.Should().Be(DateTime.MinValue);
            command.SName.Should().BeEmpty();
            command.ErrCode.Should().Be(int.MaxValue);
            command.ErrMsg.Should().BeNull();
            command.IsValid().Should().BeTrue(); // Should still be valid
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldMaintainStateConsistency()
        {
            // Arrange
            var command = new InsertAffMemberTimeTrackerCommand("CONSISTENCY_TEST", "sp_TestConsistency");
            var testTime = new DateTime(2024, 1, 15, 10, 30, 0);

            // Act - Set initial state
            command.LastImportedTime = testTime;
            command.ErrCode = 0;
            command.ErrMsg = "Success";

            var initialTime = command.LastImportedTime;
            var initialSpName = command.SName;
            var initialErrCode = command.ErrCode;
            var initialErrMsg = command.ErrMsg;

            // Perform multiple operations
            for (int i = 0; i < 5; i++)
            {
                command.IsValid();
            }

            // Assert - State should remain consistent
            command.LastImportedTime.Should().Be(initialTime);
            command.SName.Should().Be(initialSpName);
            command.ErrCode.Should().Be(initialErrCode);
            command.ErrMsg.Should().Be(initialErrMsg);
        }

        [Fact]
        public async Task InsertAffMemberTimeTrackerCommand_ShouldSupportConcurrentAccess()
        {
            // Arrange - Concurrent access scenario
            var command = new InsertAffMemberTimeTrackerCommand("CONCURRENT_TEST", "sp_ConcurrentTest");
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
        public void InsertAffMemberTimeTrackerCommand_ShouldSupportBatchTimeTrackingScenario()
        {
            // Arrange - Batch time tracking scenario
            var batchTimes = new[]
            {
                new DateTime(2024, 1, 15, 10, 0, 0),
                new DateTime(2024, 1, 15, 11, 0, 0),
                new DateTime(2024, 1, 15, 12, 0, 0),
                new DateTime(2024, 1, 15, 13, 0, 0),
                new DateTime(2024, 1, 15, 14, 0, 0)
            };

            var commands = new List<InsertAffMemberTimeTrackerCommand>();

            // Act - Create batch of time tracking commands
            foreach (var batchTime in batchTimes)
            {
                var command = new InsertAffMemberTimeTrackerCommand("BATCH_AFF", "sp_BatchImport");
                command.LastImportedTime = batchTime;
                command.ErrCode = 0;
                command.ErrMsg = $"Batch import at {batchTime:HH:mm}";
                commands.Add(command);
            }

            // Assert - All commands should be valid and chronologically ordered
            commands.Should().HaveCount(5);
            commands.Should().AllSatisfy(cmd =>
            {
                cmd.ErrCode.Should().Be(0);
                cmd.IsValid().Should().BeTrue();
            });

            // Verify chronological order
            for (int i = 1; i < commands.Count; i++)
            {
                commands[i].LastImportedTime.Should().BeAfter(commands[i - 1].LastImportedTime);
            }
        }

        [Fact]
        public void InsertAffMemberTimeTrackerCommand_ShouldSupportHighFrequencyImportScenario()
        {
            // Arrange - High frequency import scenario (every minute)
            var command = new InsertAffMemberTimeTrackerCommand("HIGH_FREQ_AFF", "sp_HighFreqImport");
            var baseTime = new DateTime(2024, 1, 15, 10, 0, 0);

            // Act - Simulate high frequency imports
            for (int minute = 0; minute < 60; minute++)
            {
                command.LastImportedTime = baseTime.AddMinutes(minute);
                command.ErrCode = 0;
                command.ErrMsg = $"Import #{minute + 1}";

                // Assert - Each import should be valid
                command.IsValid().Should().BeTrue();
                command.LastImportedTime.Should().Be(baseTime.AddMinutes(minute));
            }
        }
    }
}
