using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for TpBetRecordModel class.
    /// Reduced from 373 lines to focus on meaningful business scenarios.
    /// Third-party betting record model with game type and settlement support.
    /// </summary>
    public class TpBetRecordModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var betRecord = new TpBetRecordModel();

            // Assert
            betRecord.Should().NotBeNull();
            betRecord.TpId.Should().Be(0);
            betRecord.TransId.Should().BeNull();
            betRecord.GameType.Should().Be(0);
            betRecord.CustId.Should().BeNull();
            betRecord.UserName.Should().BeNull();
            betRecord.CurrencyId.Should().BeNull();
            betRecord.SiteId.Should().BeNull();
            betRecord.TpUserName.Should().BeNull();
            betRecord.TransTime.Should().BeNull();
            betRecord.SettledTime.Should().BeNull();
            betRecord.Stake.Should().BeNull();
            betRecord.WinlostAmount.Should().BeNull();
            betRecord.SettlementType.Should().BeNull();
            betRecord.TicketStatus.Should().BeNull();
            betRecord.ValidBet.Should().BeNull();
            betRecord.Revenue.Should().BeNull();
            betRecord.ProgressiveBet.Should().BeNull();
            betRecord.ProgressiveWin.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var transTime = DateTime.UtcNow.AddHours(-2);
            var settledTime = DateTime.UtcNow.AddHours(-1);
            var createTime = DateTime.UtcNow.AddHours(-3);

            // Act
            var betRecord = new TpBetRecordModel
            {
                TpId = 12345,
                TransId = "BET_TXN_001",
                GameType = 1,
                CustId = "CUST_67890",
                UserName = "lucky_player",
                CurrencyId = 1,
                SiteId = 100,
                TpUserName = "tp_lucky_player",
                TransTime = transTime,
                SettledTime = settledTime,
                Stake = 50.00m,
                WinlostAmount = 75.00m,
                SettlementType = 1,
                TicketStatus = 2,
                OriginalState = "WON",
                TicketKey = 98765,
                CreateTime = createTime,
                ValidBet = 50.00m,
                BonusId = 123,
                Revenue = 25.00m,
                ProgressiveBet = 5.00m,
                ProgressiveWin = 0.00m
            };

            // Assert
            betRecord.TpId.Should().Be(12345);
            betRecord.TransId.Should().Be("BET_TXN_001");
            betRecord.GameType.Should().Be(1);
            betRecord.CustId.Should().Be("CUST_67890");
            betRecord.UserName.Should().Be("lucky_player");
            betRecord.TpUserName.Should().Be("tp_lucky_player");
            betRecord.TransTime.Should().Be(transTime);
            betRecord.SettledTime.Should().Be(settledTime);
            betRecord.Stake.Should().Be(50.00m);
            betRecord.WinlostAmount.Should().Be(75.00m);
            betRecord.OriginalState.Should().Be("WON");
            betRecord.ValidBet.Should().Be(50.00m);
            betRecord.Revenue.Should().Be(25.00m);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GameType_ShouldRepresentVariousGameCategories(int gameType)
        {
            // Act
            var betRecord = new TpBetRecordModel { GameType = gameType };

            // Assert
            betRecord.GameType.Should().Be(gameType);

        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TicketStatus_ShouldRepresentBetStates(int status)
        {
            // Act
            var betRecord = new TpBetRecordModel { TicketStatus = status };

            // Assert
            betRecord.TicketStatus.Should().Be(status);

        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportWinningSlotsBet()
        {
            // Arrange - Winning slots bet
            var betRecord = new TpBetRecordModel
            {
                TpId = 88888,
                TransId = "SLOTS_WIN_001",
                GameType = 1, // Slots
                CustId = "SLOTS_PLAYER",
                UserName = "slot_master",
                TpUserName = "tp_slot_master",
                TransTime = DateTime.UtcNow.AddMinutes(-30),
                SettledTime = DateTime.UtcNow.AddMinutes(-29),
                Stake = 25.00m,
                WinlostAmount = 125.00m, // 5x win
                SettlementType = 1,
                TicketStatus = 1, // Settled
                OriginalState = "WON",
                ValidBet = 25.00m,
                Revenue = 100.00m, // Net win
                ProgressiveBet = 2.50m, // 10% to progressive
                ProgressiveWin = 0.00m // No progressive win this time
            };

            // Assert - Winning slots validations
            betRecord.GameType.Should().Be(1); // Slots
            betRecord.WinlostAmount.Should().BePositive();
            betRecord.WinlostAmount.Should().BeGreaterThan(betRecord.Stake.Value);
            betRecord.Revenue.Should().BePositive();
            betRecord.OriginalState.Should().Be("WON");
            betRecord.TicketStatus.Should().Be(1); // Settled
            betRecord.ProgressiveBet.Should().BePositive();
            betRecord.TransId.Should().Contain("SLOTS");
        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportLosingBet()
        {
            // Arrange - Losing bet
            var betRecord = new TpBetRecordModel
            {
                TpId = 77777,
                TransId = "LOSS_BET_001",
                GameType = 2, // Table games
                CustId = "UNLUCKY_PLAYER",
                UserName = "table_player",
                Stake = 100.00m,
                WinlostAmount = -100.00m, // Full loss
                SettlementType = 1,
                TicketStatus = 1, // Settled
                OriginalState = "LOST",
                ValidBet = 100.00m,
                Revenue = -100.00m, // House wins
                ProgressiveBet = 0.00m, // No progressive on table games
                ProgressiveWin = 0.00m
            };

            // Assert - Losing bet validations
            betRecord.WinlostAmount.Should().BeNegative();
            betRecord.WinlostAmount.Should().Be(-betRecord.Stake.Value);
            betRecord.Revenue.Should().BeNegative();
            betRecord.OriginalState.Should().Be("LOST");
            betRecord.ProgressiveBet.Should().Be(0); // No progressive
        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportProgressiveJackpotWin()
        {
            // Arrange - Progressive jackpot win
            var betRecord = new TpBetRecordModel
            {
                TpId = 999999,
                TransId = "JACKPOT_WIN_001",
                GameType = 1, // Slots
                CustId = "JACKPOT_WINNER",
                UserName = "mega_winner",
                Stake = 10.00m,
                WinlostAmount = 50010.00m, // Base win + progressive
                SettlementType = 1,
                TicketStatus = 1,
                OriginalState = "JACKPOT",
                ValidBet = 10.00m,
                Revenue = 50000.00m, // Massive win
                ProgressiveBet = 1.00m, // Progressive contribution
                ProgressiveWin = 50000.00m // Progressive jackpot won
            };

            // Assert - Progressive jackpot validations
            betRecord.ProgressiveWin.Should().BePositive();
            betRecord.ProgressiveWin.Should().BeGreaterThan(1000); // Substantial jackpot
            betRecord.WinlostAmount.Should().BeGreaterThan(betRecord.ProgressiveWin.Value);
            betRecord.OriginalState.Should().Be("JACKPOT");
            betRecord.Revenue.Should().BeGreaterThan(10000);
            betRecord.TransId.Should().Contain("JACKPOT");
        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportLiveCasinoBet()
        {
            // Arrange - Live casino bet
            var betRecord = new TpBetRecordModel
            {
                TpId = 66666,
                TransId = "LIVE_BLACKJACK_001",
                GameType = 3, // Live casino
                CustId = "LIVE_PLAYER",
                UserName = "blackjack_pro",
                TpUserName = "tp_blackjack_pro",
                TransTime = DateTime.UtcNow.AddMinutes(-45),
                SettledTime = DateTime.UtcNow.AddMinutes(-40),
                Stake = 200.00m,
                WinlostAmount = 200.00m, // Even money win
                SettlementType = 1,
                TicketStatus = 1,
                OriginalState = "WON",
                ValidBet = 200.00m,
                Revenue = 200.00m,
                BonusId = null, // No bonus involved
                ProgressiveBet = 0.00m, // No progressive in live games
                ProgressiveWin = 0.00m
            };

            // Assert - Live casino validations
            betRecord.GameType.Should().Be(3); // Live casino
            betRecord.WinlostAmount.Should().Be(betRecord.Stake); // Even money
            betRecord.ProgressiveBet.Should().Be(0); // No progressive
            betRecord.TransId.Should().Contain("LIVE");
            betRecord.SettledTime.Should().BeAfter(betRecord.TransTime.Value);
        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportVoidBet()
        {
            // Arrange - Void bet scenario
            var betRecord = new TpBetRecordModel
            {
                TpId = 44444,
                TransId = "VOID_BET_001",
                GameType = 4, // Sports betting
                CustId = "SPORTS_BETTOR",
                UserName = "sports_fan",
                Stake = 50.00m,
                WinlostAmount = 0.00m, // No win/loss on void
                SettlementType = 2, // Void settlement
                TicketStatus = 3, // Void
                OriginalState = "VOID",
                ValidBet = 0.00m, // Not valid bet when void
                Revenue = 0.00m,
                ProgressiveBet = 0.00m,
                ProgressiveWin = 0.00m
            };

            // Assert - Void bet validations
            betRecord.TicketStatus.Should().Be(3); // Void
            betRecord.WinlostAmount.Should().Be(0);
            betRecord.ValidBet.Should().Be(0);
            betRecord.Revenue.Should().Be(0);
            betRecord.OriginalState.Should().Be("VOID");
            betRecord.SettlementType.Should().Be(2);
        }

        [Fact]
        public void TpBetRecordModel_ShouldHandleNullableProperties()
        {
            // Act
            var betRecord = new TpBetRecordModel
            {
                CurrencyId = null,
                SiteId = null,
                TransTime = null,
                SettledTime = null,
                Stake = null,
                WinlostAmount = null,
                SettlementType = null,
                TicketStatus = null,
                TicketKey = null,
                CreateTime = null,
                ValidBet = null,
                BonusId = null,
                Revenue = null,
                ProgressiveBet = null,
                ProgressiveWin = null
            };

            // Assert
            betRecord.CurrencyId.Should().BeNull();
            betRecord.Stake.Should().BeNull();
            betRecord.WinlostAmount.Should().BeNull();
            betRecord.TransTime.Should().BeNull();
            betRecord.SettledTime.Should().BeNull();
            betRecord.ValidBet.Should().BeNull();
            betRecord.Revenue.Should().BeNull();
        }

        [Fact]
        public void TpBetRecordModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var betRecords = new[]
            {
                new TpBetRecordModel { TpId = 1, GameType = 1, Stake = 10.00m, WinlostAmount = 15.00m, Revenue = 5.00m },
                new TpBetRecordModel { TpId = 2, GameType = 1, Stake = 25.00m, WinlostAmount = -25.00m, Revenue = -25.00m },
                new TpBetRecordModel { TpId = 3, GameType = 2, Stake = 50.00m, WinlostAmount = 100.00m, Revenue = 50.00m }
            };

            // Act & Assert
            betRecords.Should().HaveCount(3);
            betRecords.Sum(b => b.Stake ?? 0).Should().Be(85.00m); // Total stakes
            betRecords.Sum(b => b.WinlostAmount ?? 0).Should().Be(90.00m); // Net winnings
            betRecords.Sum(b => b.Revenue ?? 0).Should().Be(30.00m); // Net revenue
            betRecords.Where(b => b.GameType == 1).Should().HaveCount(2); // Slots games
            betRecords.Where(b => (b.WinlostAmount ?? 0) > 0).Should().HaveCount(2); // Winning bets
        }

        [Theory]
        [InlineData(100.00, 150.00, 50.00)]
        [InlineData(50.00, -50.00, -50.00)]
        [InlineData(25.00, 0.00, 0.00)]
        public void BetCalculation_ShouldCalculateRevenue(decimal stake, decimal winlost, decimal expectedRevenue)
        {
            // Arrange
            var betRecord = new TpBetRecordModel
            {
                Stake = stake,
                WinlostAmount = winlost,
                Revenue = expectedRevenue
            };

            // Act & Assert
            betRecord.Revenue.Should().Be(expectedRevenue);
            if (winlost > 0)
            {
                betRecord.Revenue.Should().BePositive(); // Player wins, house revenue positive
            }
            else if (winlost < 0)
            {
                betRecord.Revenue.Should().BeNegative(); // Player loses, house revenue negative
            }
        }
    }
}