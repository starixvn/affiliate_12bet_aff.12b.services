using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for TpReport9WModel class.
    /// Reduced from 489 lines to focus on meaningful business scenarios.
    /// Third-party report model for 9W betting platform.
    /// </summary>
    public class TpReport9WModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var report = new TpReport9WModel();

            // Assert
            report.Should().NotBeNull();
            report.SQ.Should().Be(0);
            report.UserId.Should().BeNull();
            report.AgentId.Should().BeNull();
            report.EventId.Should().BeNull();
            report.MarketId.Should().BeNull();
            report.SettledDate.Should().Be(default(DateTime));
            report.ResettledDate.Should().BeNull();
            report.ProfitLoss.Should().Be(0);
            report.MarketTotal.Should().Be(0);
            report.Commission.Should().Be(0);
            report.AgentCommission.Should().Be(0);
            report.StakeTotal.Should().Be(0);
            report.CreateTime.Should().BeNull();
            report.UpdateTime.Should().BeNull();
            report.CustId.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var settledDate = DateTime.UtcNow.AddHours(-2);
            var resettledDate = DateTime.UtcNow.AddHours(-1);
            var createTime = DateTime.UtcNow.AddHours(-3);
            var updateTime = DateTime.UtcNow;

            // Act
            var report = new TpReport9WModel
            {
                SQ = 123456789L,
                UserId = "USER001",
                AgentId = "AGENT001",
                EventId = "EVENT_SOCCER_001",
                MarketId = "MARKET_1X2_001",
                SettledDate = settledDate,
                ResettledDate = resettledDate,
                ProfitLoss = 150.75m,
                MarketTotal = 1000.00m,
                Commission = 25.50m,
                AgentCommission = 12.75m,
                StakeTotal = 500.00m,
                CreateTime = createTime,
                UpdateTime = updateTime,
                CustId = 98765
            };

            // Assert
            report.SQ.Should().Be(123456789L);
            report.UserId.Should().Be("USER001");
            report.AgentId.Should().Be("AGENT001");
            report.EventId.Should().Be("EVENT_SOCCER_001");
            report.MarketId.Should().Be("MARKET_1X2_001");
            report.SettledDate.Should().Be(settledDate);
            report.ResettledDate.Should().Be(resettledDate);
            report.ProfitLoss.Should().Be(150.75m);
            report.MarketTotal.Should().Be(1000.00m);
            report.Commission.Should().Be(25.50m);
            report.AgentCommission.Should().Be(12.75m);
            report.StakeTotal.Should().Be(500.00m);
            report.CreateTime.Should().Be(createTime);
            report.UpdateTime.Should().Be(updateTime);
            report.CustId.Should().Be(98765);
        }

        [Theory]
        [InlineData(100.00, 25.00, 12.50, 500.00)]
        [InlineData(-50.00, 10.00, 5.00, 200.00)]
        [InlineData(0.00, 0.00, 0.00, 0.00)]
        public void FinancialProperties_ShouldHandleVariousAmounts(decimal profitLoss, decimal commission, decimal agentCommission, decimal stakeTotal)
        {
            // Act
            var report = new TpReport9WModel
            {
                ProfitLoss = profitLoss,
                Commission = commission,
                AgentCommission = agentCommission,
                StakeTotal = stakeTotal
            };

            // Assert
            report.ProfitLoss.Should().Be(profitLoss);
            report.Commission.Should().Be(commission);
            report.AgentCommission.Should().Be(agentCommission);
            report.StakeTotal.Should().Be(stakeTotal);
        }

        [Fact]
        public void TpReport9WModel_ShouldSupportSoccerBettingScenario()
        {
            // Arrange - Soccer match betting scenario
            var report = new TpReport9WModel
            {
                SQ = 987654321L,
                UserId = "SOCCER_FAN_001",
                AgentId = "SPORTS_AGENT_001",
                EventId = "SOCCER_PREMIER_LEAGUE_001",
                MarketId = "1X2_FULLTIME",
                SettledDate = DateTime.UtcNow.AddHours(-1),
                ResettledDate = null, // No resettlement
                ProfitLoss = 75.00m, // Won the bet
                MarketTotal = 2500.00m,
                Commission = 7.50m, // 10% commission
                AgentCommission = 3.75m, // 5% agent commission
                StakeTotal = 100.00m,
                CreateTime = DateTime.UtcNow.AddHours(-3),
                UpdateTime = DateTime.UtcNow.AddHours(-1),
                CustId = 12345
            };

            // Assert - Business validations
            report.SQ.Should().BePositive();
            report.CustId.Should().BePositive();
            report.ProfitLoss.Should().BePositive(); // Winning bet
            report.Commission.Should().BePositive();
            report.AgentCommission.Should().BePositive();
            report.StakeTotal.Should().BePositive();
            report.EventId.Should().Contain("SOCCER");
            report.MarketId.Should().Contain("1X2");
            report.ResettledDate.Should().BeNull(); // No resettlement needed
        }

        [Fact]
        public void TpReport9WModel_ShouldSupportLosingBetScenario()
        {
            // Arrange - Losing bet scenario
            var report = new TpReport9WModel
            {
                SQ = 555666777L,
                UserId = "UNLUCKY_USER",
                AgentId = "AGENT_002",
                EventId = "BASKETBALL_NBA_001",
                MarketId = "OVER_UNDER_200",
                SettledDate = DateTime.UtcNow.AddMinutes(-30),
                ResettledDate = null,
                ProfitLoss = -100.00m, // Lost the bet
                MarketTotal = 1500.00m,
                Commission = 0.00m, // No commission on losses
                AgentCommission = 0.00m,
                StakeTotal = 100.00m,
                CreateTime = DateTime.UtcNow.AddHours(-2),
                UpdateTime = DateTime.UtcNow.AddMinutes(-30),
                CustId = 54321
            };

            // Assert - Losing bet validations
            report.ProfitLoss.Should().BeNegative(); // Lost money
            report.Commission.Should().Be(0); // No commission on losses
            report.AgentCommission.Should().Be(0);
            report.StakeTotal.Should().BePositive(); // Still had stake
            report.EventId.Should().Contain("BASKETBALL");
        }

        [Fact]
        public void TpReport9WModel_ShouldSupportResettlementScenario()
        {
            // Arrange - Resettlement scenario (bet result changed)
            var originalSettled = DateTime.UtcNow.AddHours(-4);
            var resettled = DateTime.UtcNow.AddHours(-1);

            var report = new TpReport9WModel
            {
                SQ = 111222333L,
                UserId = "DISPUTED_USER",
                AgentId = "AGENT_003",
                EventId = "TENNIS_WIMBLEDON_001",
                MarketId = "SET_WINNER_1",
                SettledDate = originalSettled,
                ResettledDate = resettled, // Result was changed
                ProfitLoss = -50.00m, // Changed from win to loss
                MarketTotal = 800.00m,
                Commission = 0.00m, // Adjusted after resettlement
                AgentCommission = 0.00m,
                StakeTotal = 50.00m,
                CreateTime = DateTime.UtcNow.AddHours(-5),
                UpdateTime = resettled,
                CustId = 13579
            };

            // Assert - Resettlement validations
            report.ResettledDate.Should().NotBeNull();
            report.ResettledDate.Should().BeAfter(report.SettledDate);
            report.UpdateTime.Should().Be(report.ResettledDate);
            report.EventId.Should().Contain("TENNIS");
        }

        [Fact]
        public void TpReport9WModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var reports = new[]
            {
                new TpReport9WModel { SQ = 1, ProfitLoss = 100.00m, Commission = 10.00m },
                new TpReport9WModel { SQ = 2, ProfitLoss = -50.00m, Commission = 0.00m },
                new TpReport9WModel { SQ = 3, ProfitLoss = 25.00m, Commission = 2.50m }
            };

            // Act & Assert
            reports.Should().HaveCount(3);
            reports.Sum(r => r.ProfitLoss).Should().Be(75.00m); // Net profit
            reports.Sum(r => r.Commission).Should().Be(12.50m); // Total commission
            reports.Where(r => r.ProfitLoss > 0).Should().HaveCount(2); // Winning bets
            reports.Max(r => r.SQ).Should().Be(3);
        }

        [Fact]
        public void TpReport9WModel_ShouldHandleNullableProperties()
        {
            // Act
            var report = new TpReport9WModel
            {
                ResettledDate = null,
                CreateTime = null,
                UpdateTime = null
            };

            // Assert
            report.ResettledDate.Should().BeNull();
            report.CreateTime.Should().BeNull();
            report.UpdateTime.Should().BeNull();
        }
    }
}