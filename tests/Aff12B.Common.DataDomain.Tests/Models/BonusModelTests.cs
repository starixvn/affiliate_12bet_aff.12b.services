using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for BonusModel class.
    /// Reduced from 330 lines to focus on meaningful business scenarios.
    /// Bonus/promotion model with expiry tracking and status management.
    /// </summary>
    public class BonusModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var bonus = new BonusModel();

            // Assert
            bonus.Should().NotBeNull();
            bonus.BonusId.Should().Be(0);
            bonus.TransId.Should().BeNull();
            bonus.PromotionType.Should().Be(0);
            bonus.BonusCode.Should().BeNull();
            bonus.CustId.Should().Be(0);
            bonus.UserName.Should().BeNull();
            bonus.CurrencyId.Should().Be(0);
            bonus.SiteId.Should().Be(0);
            bonus.LockedAmount.Should().Be(0);
            bonus.BonusAmount.Should().Be(0);
            bonus.ExpiryTime.Should().Be(default);
            bonus.BonusInfo.Should().BeNull();
            bonus.Memo.Should().BeNull();
            bonus.CreateTime.Should().Be(default);
            bonus.Status.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTime = DateTime.UtcNow.AddDays(-1);
            var expiryTime = DateTime.UtcNow.AddDays(30);

            // Act
            var bonus = new BonusModel
            {
                BonusId = 12345,
                TransId = "BONUS_TXN_001",
                PromotionType = 1,
                BonusCode = "WELCOME100",
                CustId = 67890,
                UserName = "lucky_player",
                CurrencyId = 1,
                SiteId = 100,
                LockedAmount = 500.00m,
                BonusAmount = 100.00m,
                ExpiryTime = expiryTime,
                BonusInfo = "Welcome bonus - 100% match up to $100",
                Memo = "First deposit bonus",
                CreateTime = createTime,
                Status = 1
            };

            // Assert
            bonus.BonusId.Should().Be(12345);
            bonus.TransId.Should().Be("BONUS_TXN_001");
            bonus.PromotionType.Should().Be(1);
            bonus.BonusCode.Should().Be("WELCOME100");
            bonus.CustId.Should().Be(67890);
            bonus.UserName.Should().Be("lucky_player");
            bonus.CurrencyId.Should().Be(1);
            bonus.SiteId.Should().Be(100);
            bonus.LockedAmount.Should().Be(500.00m);
            bonus.BonusAmount.Should().Be(100.00m);
            bonus.ExpiryTime.Should().Be(expiryTime);
            bonus.BonusInfo.Should().Be("Welcome bonus - 100% match up to $100");
            bonus.Memo.Should().Be("First deposit bonus");
            bonus.CreateTime.Should().Be(createTime);
            bonus.Status.Should().Be(1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void PromotionType_ShouldSupportVariousTypes(int promotionType)
        {
            // Act
            var bonus = new BonusModel { PromotionType = promotionType };

            // Assert
            bonus.PromotionType.Should().Be(promotionType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Status_ShouldRepresentBonusStates(int status)
        {
            // Act
            var bonus = new BonusModel { Status = status };

            // Assert
            bonus.Status.Should().Be(status);
        }

        [Fact]
        public void BonusModel_ShouldSupportWelcomeBonusScenario()
        {
            // Arrange - Welcome bonus scenario
            var bonus = new BonusModel
            {
                BonusId = 88888,
                TransId = "WELCOME_BONUS_001",
                PromotionType = 1, // Welcome bonus
                BonusCode = "WELCOME100",
                CustId = 12345,
                UserName = "new_player",
                CurrencyId = 1, // USD
                SiteId = 200,
                LockedAmount = 1000.00m, // Player deposited $1000
                BonusAmount = 100.00m, // 10% bonus
                ExpiryTime = DateTime.UtcNow.AddDays(30),
                BonusInfo = "Welcome Bonus: 10% match up to $100 on first deposit",
                Memo = "New player welcome promotion",
                CreateTime = DateTime.UtcNow.AddHours(-1),
                Status = 1 // Active
            };

            // Assert - Welcome bonus validations
            bonus.PromotionType.Should().Be(1); // Welcome bonus
            bonus.BonusCode.Should().StartWith("WELCOME");
            bonus.BonusAmount.Should().BePositive();
            bonus.LockedAmount.Should().BeGreaterThan(bonus.BonusAmount); // Deposit > bonus
            bonus.ExpiryTime.Should().BeAfter(bonus.CreateTime);
            bonus.Status.Should().Be(1); // Active
            bonus.BonusInfo.Should().Contain("Welcome");
            bonus.TransId.Should().Contain("WELCOME");
        }

        [Fact]
        public void BonusModel_ShouldSupportReloadBonusScenario()
        {
            // Arrange - Reload bonus scenario
            var bonus = new BonusModel
            {
                BonusId = 77777,
                TransId = "RELOAD_BONUS_001",
                PromotionType = 2, // Reload bonus
                BonusCode = "RELOAD50",
                CustId = 54321,
                UserName = "returning_player",
                LockedAmount = 500.00m,
                BonusAmount = 50.00m, // 10% reload bonus
                ExpiryTime = DateTime.UtcNow.AddDays(14), // Shorter expiry
                BonusInfo = "Reload Bonus: 10% on deposits over $500",
                Memo = "Weekly reload promotion",
                CreateTime = DateTime.UtcNow.AddMinutes(-30),
                Status = 1
            };

            // Assert - Reload bonus validations
            bonus.PromotionType.Should().Be(2); // Reload bonus
            bonus.BonusCode.Should().StartWith("RELOAD");
            bonus.BonusAmount.Should().BeLessThan(bonus.LockedAmount);
            bonus.ExpiryTime.Should().BeAfter(DateTime.UtcNow.AddDays(7)); // At least a week
            bonus.TransId.Should().Contain("RELOAD");
            bonus.UserName.Should().Contain("returning");
        }

        [Fact]
        public void BonusModel_ShouldSupportFreeSpinsBonusScenario()
        {
            // Arrange - Free spins bonus scenario
            var bonus = new BonusModel
            {
                BonusId = 99999,
                TransId = "FREESPINS_001",
                PromotionType = 3, // Free spins
                BonusCode = "SPIN50",
                CustId = 11111,
                UserName = "slots_lover",
                LockedAmount = 0.00m, // No deposit required
                BonusAmount = 25.00m, // Value of free spins
                ExpiryTime = DateTime.UtcNow.AddDays(7), // Short expiry for spins
                BonusInfo = "50 Free Spins on Starburst - No deposit required",
                Memo = "Weekend free spins promotion",
                CreateTime = DateTime.UtcNow.AddMinutes(-15),
                Status = 1
            };

            // Assert - Free spins validations
            bonus.PromotionType.Should().Be(3); // Free spins
            bonus.LockedAmount.Should().Be(0); // No deposit required
            bonus.BonusAmount.Should().BePositive(); // Has value
            bonus.BonusCode.Should().StartWith("SPIN");
            bonus.BonusInfo.Should().Contain("Free Spins");
            bonus.ExpiryTime.Should().BeBefore(DateTime.UtcNow.AddDays(10)); // Short expiry
        }

        [Fact]
        public void BonusModel_ShouldSupportExpiredBonusScenario()
        {
            // Arrange - Expired bonus scenario
            var bonus = new BonusModel
            {
                BonusId = 66666,
                TransId = "EXPIRED_BONUS_001",
                PromotionType = 1,
                BonusCode = "EXPIRED100",
                CustId = 33333,
                UserName = "late_player",
                LockedAmount = 200.00m,
                BonusAmount = 20.00m,
                ExpiryTime = DateTime.UtcNow.AddDays(-5), // Expired 5 days ago
                BonusInfo = "Expired welcome bonus",
                Memo = "Bonus expired - not claimed in time",
                CreateTime = DateTime.UtcNow.AddDays(-35),
                Status = 3 // Expired
            };

            // Assert - Expired bonus validations
            bonus.Status.Should().Be(3); // Expired
            bonus.ExpiryTime.Should().BeBefore(DateTime.UtcNow); // Past expiry
            bonus.CreateTime.Should().BeBefore(bonus.ExpiryTime); // Created before expiry
            bonus.Memo.Should().Contain("expired");
            bonus.BonusInfo.Should().Contain("Expired");
        }

        [Fact]
        public void BonusModel_ShouldSupportHighRollerBonusScenario()
        {
            // Arrange - High roller VIP bonus
            var bonus = new BonusModel
            {
                BonusId = 555555,
                TransId = "VIP_BONUS_001",
                PromotionType = 5, // VIP bonus
                BonusCode = "VIP1000",
                CustId = 99999,
                UserName = "high_roller_vip",
                CurrencyId = 1,
                SiteId = 500, // VIP site
                LockedAmount = 10000.00m, // High deposit
                BonusAmount = 1000.00m, // Substantial bonus
                ExpiryTime = DateTime.UtcNow.AddDays(90), // Extended expiry for VIP
                BonusInfo = "VIP Exclusive: 10% bonus on deposits over $10,000",
                Memo = "High roller VIP treatment",
                CreateTime = DateTime.UtcNow.AddHours(-2),
                Status = 1
            };

            // Assert - High roller validations
            bonus.PromotionType.Should().Be(5); // VIP bonus
            bonus.LockedAmount.Should().BeGreaterThan(5000); // High deposit
            bonus.BonusAmount.Should().BeGreaterThan(500); // Substantial bonus
            bonus.SiteId.Should().Be(500); // VIP site
            bonus.ExpiryTime.Should().BeAfter(DateTime.UtcNow.AddDays(60)); // Extended expiry
            bonus.UserName.Should().Contain("vip");
            bonus.BonusInfo.Should().Contain("VIP");
        }

        [Fact]
        public void BonusModel_ShouldSupportCancelledBonusScenario()
        {
            // Arrange - Cancelled bonus scenario
            var bonus = new BonusModel
            {
                BonusId = 44444,
                TransId = "CANCELLED_BONUS_001",
                PromotionType = 2,
                BonusCode = "CANCELLED50",
                CustId = 22222,
                UserName = "changed_mind_player",
                LockedAmount = 300.00m,
                BonusAmount = 30.00m,
                ExpiryTime = DateTime.UtcNow.AddDays(20),
                BonusInfo = "Cancelled reload bonus",
                Memo = "Player requested bonus cancellation",
                CreateTime = DateTime.UtcNow.AddDays(-2),
                Status = 4 // Cancelled
            };

            // Assert - Cancelled bonus validations
            bonus.Status.Should().Be(4); // Cancelled
            bonus.Memo.Should().Contain("cancellation");
            bonus.BonusInfo.Should().Contain("Cancelled");
            bonus.ExpiryTime.Should().BeAfter(DateTime.UtcNow); // Still valid time but cancelled
        }

        [Fact]
        public void BonusModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var bonuses = new[]
            {
                new BonusModel { BonusId = 1, BonusAmount = 100.00m, Status = 1, PromotionType = 1 },
                new BonusModel { BonusId = 2, BonusAmount = 50.00m, Status = 1, PromotionType = 2 },
                new BonusModel { BonusId = 3, BonusAmount = 25.00m, Status = 3, PromotionType = 3 }
            };

            // Act & Assert
            bonuses.Should().HaveCount(3);
            bonuses.Where(b => b.Status == 1).Should().HaveCount(2); // Active bonuses
            bonuses.Sum(b => b.BonusAmount).Should().Be(175.00m); // Total bonus value
            bonuses.Where(b => b.PromotionType == 1).Should().HaveCount(1); // Welcome bonuses
            bonuses.Max(b => b.BonusAmount).Should().Be(100.00m); // Highest bonus
        }

        [Fact]
        public void BonusModel_ShouldHandleUnicodeContent()
        {
            // Act
            var bonus = new BonusModel
            {
                BonusCode = "欢迎奖金_WELCOME_ボーナス",
                BonusInfo = "多语言奖金信息_Multilingual_多言語ボーナス",
                Memo = "国际化支持_International_国際化サポート"
            };

            // Assert
            bonus.BonusCode.Should().Contain("欢迎奖金");
            bonus.BonusCode.Should().Contain("WELCOME");
            bonus.BonusCode.Should().Contain("ボーナス");
            bonus.BonusInfo.Should().Contain("Multilingual");
            bonus.Memo.Should().Contain("International");
        }

        [Theory]
        [InlineData(100.00, 1000.00, 10.0)]
        [InlineData(50.00, 500.00, 10.0)]
        [InlineData(25.00, 1000.00, 2.5)]
        [InlineData(0.00, 100.00, 0.0)]
        public void BonusCalculation_ShouldCalculatePercentage(decimal bonusAmount, decimal lockedAmount, double expectedPercentage)
        {
            // Arrange
            var bonus = new BonusModel
            {
                BonusAmount = bonusAmount,
                LockedAmount = lockedAmount
            };

            // Act
            var actualPercentage = lockedAmount > 0 ? (double)(bonusAmount / lockedAmount * 100) : 0;

            // Assert
            actualPercentage.Should().BeApproximately(expectedPercentage, 0.1);
            if (bonusAmount > 0)
            {
                bonus.BonusAmount.Should().BePositive();
            }
        }

        [Fact]
        public void BonusModel_ShouldSupportTimeValidations()
        {
            // Arrange
            var createTime = DateTime.UtcNow.AddDays(-1);
            var expiryTime = DateTime.UtcNow.AddDays(29);

            var bonus = new BonusModel
            {
                CreateTime = createTime,
                ExpiryTime = expiryTime
            };

            // Assert - Time validations
            bonus.ExpiryTime.Should().BeAfter(bonus.CreateTime);
            bonus.ExpiryTime.Should().BeAfter(DateTime.UtcNow);
            (bonus.ExpiryTime - bonus.CreateTime).TotalDays.Should().BeGreaterThan(25);
        }
    }
}