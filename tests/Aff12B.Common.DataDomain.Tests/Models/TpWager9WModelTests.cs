using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for TpWager9WModel class.
    /// Reduced from 476 lines to focus on meaningful business scenarios.
    /// Third-party wager model for 9W betting platform.
    /// </summary>
    public class TpWager9WModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var wager = new TpWager9WModel();

            // Assert
            wager.Should().NotBeNull();
            wager.BetId.Should().BeNull();
            wager.UserId.Should().BeNull();
            wager.AgentId.Should().BeNull();
            wager.EventId.Should().BeNull();
            wager.MarketId.Should().BeNull();
            wager.MatchSettledDate.Should().BeNull();
            wager.BetStatus.Should().BeNull();
            wager.MatchStake.Should().Be(0);
            wager.ProfitLoss.Should().Be(0);
            wager.CreateTime.Should().BeNull();
            wager.UpdateTime.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTime = DateTime.UtcNow.AddHours(-2);
            var updateTime = DateTime.UtcNow;

            // Act
            var wager = new TpWager9WModel
            {
                BetId = "BET_12345",
                UserId = "USER_001",
                AgentId = "AGENT_001",
                EventId = "SOCCER_MATCH_001",
                MarketId = "1X2_FULLTIME",
                MatchSettledDate = "2024-12-16 15:30:00",
                BetStatus = "WON",
                MatchStake = 100.00m,
                ProfitLoss = 85.00m,
                CreateTime = createTime,
                UpdateTime = updateTime
            };

            // Assert
            wager.BetId.Should().Be("BET_12345");
            wager.UserId.Should().Be("USER_001");
            wager.AgentId.Should().Be("AGENT_001");
            wager.EventId.Should().Be("SOCCER_MATCH_001");
            wager.MarketId.Should().Be("1X2_FULLTIME");
            wager.MatchSettledDate.Should().Be("2024-12-16 15:30:00");
            wager.BetStatus.Should().Be("WON");
            wager.MatchStake.Should().Be(100.00m);
            wager.ProfitLoss.Should().Be(85.00m);
            wager.CreateTime.Should().Be(createTime);
            wager.UpdateTime.Should().Be(updateTime);
        }

        [Theory]
        [InlineData("WON", 100.00, 85.00)]
        [InlineData("LOST", 50.00, -50.00)]
        [InlineData("VOID", 25.00, 0.00)]
        [InlineData("PENDING", 75.00, 0.00)]
        public void BetStatus_ShouldRepresentVariousBetStates(string status, decimal stake, decimal expectedProfitLoss)
        {
            // Act
            var wager = new TpWager9WModel
            {
                BetStatus = status,
                MatchStake = stake,
                ProfitLoss = expectedProfitLoss
            };

            // Assert
            wager.BetStatus.Should().Be(status);
            wager.MatchStake.Should().Be(stake);
            wager.ProfitLoss.Should().Be(expectedProfitLoss);
        }

        [Fact]
        public void TpWager9WModel_ShouldSupportWinningBetScenario()
        {
            // Arrange - Winning soccer bet scenario
            var wager = new TpWager9WModel
            {
                BetId = "WIN_BET_001",
                UserId = "LUCKY_USER",
                AgentId = "SPORTS_AGENT",
                EventId = "PREMIER_LEAGUE_MATCH",
                MarketId = "HOME_WIN",
                MatchSettledDate = "2024-12-16 17:45:00",
                BetStatus = "WON",
                MatchStake = 200.00m,
                ProfitLoss = 180.00m, // Won 1.9x
                CreateTime = DateTime.UtcNow.AddHours(-3),
                UpdateTime = DateTime.UtcNow.AddHours(-1)
            };

            // Assert - Winning bet validations
            wager.BetId.Should().StartWith("WIN_");
            wager.BetStatus.Should().Be("WON");
            wager.ProfitLoss.Should().BePositive();
            wager.MatchStake.Should().BePositive();
            wager.EventId.Should().Contain("MATCH");
            wager.MarketId.Should().Contain("WIN");
            wager.MatchSettledDate.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TpWager9WModel_ShouldSupportLosingBetScenario()
        {
            // Arrange - Losing bet scenario
            var wager = new TpWager9WModel
            {
                BetId = "LOSS_BET_002",
                UserId = "UNLUCKY_USER",
                AgentId = "AGENT_002",
                EventId = "BASKETBALL_GAME",
                MarketId = "OVER_TOTAL_POINTS",
                MatchSettledDate = "2024-12-16 20:15:00",
                BetStatus = "LOST",
                MatchStake = 150.00m,
                ProfitLoss = -150.00m, // Lost full stake
                CreateTime = DateTime.UtcNow.AddHours(-4),
                UpdateTime = DateTime.UtcNow.AddHours(-2)
            };

            // Assert - Losing bet validations
            wager.BetStatus.Should().Be("LOST");
            wager.ProfitLoss.Should().BeNegative();
            wager.ProfitLoss.Should().Be(-wager.MatchStake); // Full loss
            wager.EventId.Should().Contain("BASKETBALL");
        }

        [Fact]
        public void TpWager9WModel_ShouldSupportVoidBetScenario()
        {
            // Arrange - Void bet scenario (match cancelled)
            var wager = new TpWager9WModel
            {
                BetId = "VOID_BET_003",
                UserId = "USER_003",
                AgentId = "AGENT_003",
                EventId = "TENNIS_CANCELLED",
                MarketId = "SET_WINNER",
                MatchSettledDate = "2024-12-16 CANCELLED",
                BetStatus = "VOID",
                MatchStake = 75.00m,
                ProfitLoss = 0.00m, // No profit/loss on void
                CreateTime = DateTime.UtcNow.AddHours(-5),
                UpdateTime = DateTime.UtcNow.AddHours(-1)
            };

            // Assert - Void bet validations
            wager.BetStatus.Should().Be("VOID");
            wager.ProfitLoss.Should().Be(0);
            wager.MatchStake.Should().BePositive(); // Stake was placed
            wager.MatchSettledDate.Should().Contain("CANCELLED");
            wager.EventId.Should().Contain("CANCELLED");
        }

        [Fact]
        public void TpWager9WModel_ShouldSupportPendingBetScenario()
        {
            // Arrange - Pending bet scenario (match not finished)
            var wager = new TpWager9WModel
            {
                BetId = "PENDING_BET_004",
                UserId = "PATIENT_USER",
                AgentId = "LIVE_AGENT",
                EventId = "LIVE_SOCCER_MATCH",
                MarketId = "NEXT_GOAL_SCORER",
                MatchSettledDate = null, // Not settled yet
                BetStatus = "PENDING",
                MatchStake = 50.00m,
                ProfitLoss = 0.00m, // Not determined yet
                CreateTime = DateTime.UtcNow.AddMinutes(-30),
                UpdateTime = DateTime.UtcNow.AddMinutes(-5)
            };

            // Assert - Pending bet validations
            wager.BetStatus.Should().Be("PENDING");
            wager.ProfitLoss.Should().Be(0); // Not determined
            wager.MatchSettledDate.Should().BeNullOrEmpty(); // Not settled
            wager.EventId.Should().Contain("LIVE");
            wager.MarketId.Should().Contain("GOAL");
        }

        [Fact]
        public void TpWager9WModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var wagers = new[]
            {
                new TpWager9WModel { BetId = "BET_1", BetStatus = "WON", MatchStake = 100.00m, ProfitLoss = 90.00m },
                new TpWager9WModel { BetId = "BET_2", BetStatus = "LOST", MatchStake = 50.00m, ProfitLoss = -50.00m },
                new TpWager9WModel { BetId = "BET_3", BetStatus = "VOID", MatchStake = 25.00m, ProfitLoss = 0.00m },
                new TpWager9WModel { BetId = "BET_4", BetStatus = "WON", MatchStake = 75.00m, ProfitLoss = 60.00m }
            };

            // Act & Assert
            wagers.Should().HaveCount(4);
            wagers.Sum(w => w.MatchStake).Should().Be(250.00m); // Total stakes
            wagers.Sum(w => w.ProfitLoss).Should().Be(100.00m); // Net profit
            wagers.Where(w => w.BetStatus == "WON").Should().HaveCount(2); // Winning bets
            wagers.Where(w => w.ProfitLoss > 0).Should().HaveCount(2); // Profitable bets
            wagers.Max(w => w.ProfitLoss).Should().Be(90.00m); // Best win
        }

        [Fact]
        public void TpWager9WModel_ShouldHandleNullableProperties()
        {
            // Act
            var wager = new TpWager9WModel
            {
                CreateTime = null,
                UpdateTime = null
            };

            // Assert
            wager.CreateTime.Should().BeNull();
            wager.UpdateTime.Should().BeNull();
        }

        [Fact]
        public void TpWager9WModel_ShouldHandleStringDateFormat()
        {
            // Arrange - Testing string date handling
            var wager = new TpWager9WModel
            {
                MatchSettledDate = "2024-12-16T15:30:00.000Z"
            };

            // Act & Assert
            wager.MatchSettledDate.Should().Be("2024-12-16T15:30:00.000Z");
            wager.MatchSettledDate.Should().Contain("2024-12-16");
            wager.MatchSettledDate.Should().Contain("T15:30:00");
        }
    }
}