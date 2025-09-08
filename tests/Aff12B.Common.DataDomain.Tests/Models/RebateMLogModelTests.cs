using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for RebateMLogModel class.
    /// Reduced from 456 lines to focus on meaningful business scenarios.
    /// </summary>
    public class RebateMLogModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var rebateLog = new RebateMLogModel();

            // Assert
            rebateLog.Id.Should().Be(0);
            rebateLog.CustId.Should().Be(0);
            rebateLog.CurrencyId.Should().Be(0);
            rebateLog.RebateType.Should().Be(0);
            rebateLog.UserName.Should().BeNull();
            rebateLog.SiteId.Should().Be(0);
            rebateLog.RedeemAmount.Should().Be(0);
            rebateLog.BonusCode.Should().BeNull();
            rebateLog.Memo.Should().BeNull();
            rebateLog.RelId.Should().Be(0);
            rebateLog.CreateTime.Should().Be(default(DateTime));
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTime = new DateTime(2024, 12, 16, 14, 30, 0);

            // Act
            var rebateLog = new RebateMLogModel
            {
                Id = 12345,
                CustId = 67890,
                CurrencyId = 1,
                RebateType = 2,
                UserName = "testuser",
                SiteId = 100,
                RedeemAmount = 150.75m,
                BonusCode = "BONUS2024",
                Memo = "Monthly rebate bonus",
                RelId = 999888777,
                CreateTime = createTime
            };

            // Assert
            rebateLog.Id.Should().Be(12345);
            rebateLog.CustId.Should().Be(67890);
            rebateLog.CurrencyId.Should().Be(1);
            rebateLog.RebateType.Should().Be(2);
            rebateLog.UserName.Should().Be("testuser");
            rebateLog.SiteId.Should().Be(100);
            rebateLog.RedeemAmount.Should().Be(150.75m);
            rebateLog.BonusCode.Should().Be("BONUS2024");
            rebateLog.Memo.Should().Be("Monthly rebate bonus");
            rebateLog.RelId.Should().Be(999888777);
            rebateLog.CreateTime.Should().Be(createTime);
        }

        [Theory]
        [InlineData(0, 0.00)]
        [InlineData(100.50, 100.50)]
        [InlineData(-50.25, -50.25)] // Negative for reversals
        [InlineData(999999.99, 999999.99)]
        public void RedeemAmount_ShouldHandleVariousDecimalValues(decimal amount, decimal expected)
        {
            // Act
            var rebateLog = new RebateMLogModel { RedeemAmount = amount };

            // Assert
            rebateLog.RedeemAmount.Should().Be(expected);
        }

        [Fact]
        public void RebateMLogModel_ShouldSupportPositiveRebateScenario()
        {
            // Arrange & Act - Positive rebate
            var rebateLog = new RebateMLogModel
            {
                Id = 12345,
                CustId = 67890,
                CurrencyId = 1,
                RebateType = 1, // Weekly rebate
                UserName = "vip_customer@casino.com",
                SiteId = 100,
                RedeemAmount = 500.75m,
                BonusCode = "VIP_WEEKLY_2024",
                Memo = "Weekly VIP rebate bonus for high-value customer",
                RelId = 999888777,
                CreateTime = DateTime.UtcNow
            };

            // Assert - Business validations
            rebateLog.Id.Should().BePositive();
            rebateLog.CustId.Should().BePositive();
            rebateLog.RedeemAmount.Should().BePositive();
            rebateLog.UserName.Should().Contain("@");
            rebateLog.BonusCode.Should().Contain("VIP");
            rebateLog.Memo.Should().Contain("rebate");
            rebateLog.CreateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void RebateMLogModel_ShouldSupportNegativeReversalScenario()
        {
            // Arrange & Act - Negative amounts for reversals
            var rebateLog = new RebateMLogModel
            {
                Id = 2,
                CustId = 456,
                RedeemAmount = -100.50m,
                BonusCode = "REVERSAL",
                Memo = "Rebate reversal due to policy violation"
            };

            // Assert
            rebateLog.RedeemAmount.Should().BeNegative();
            rebateLog.BonusCode.Should().Be("REVERSAL");
            rebateLog.Memo.Should().Contain("reversal");
        }

        [Fact]
        public void RebateMLogModel_ShouldHandleUnicodeContent()
        {
            // Act
            var rebateLog = new RebateMLogModel
            {
                UserName = "用户_José_García",
                BonusCode = "奖金_CÓDIGO_2024",
                Memo = "备注信息_Información_adicional_🎰💰"
            };

            // Assert
            rebateLog.UserName.Should().Contain("José");
            rebateLog.BonusCode.Should().Contain("CÓDIGO");
            rebateLog.Memo.Should().Contain("🎰");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void RebateType_ShouldRepresentDifferentRebateTypes(byte rebateType)
        {
            // Act
            var rebateLog = new RebateMLogModel { RebateType = rebateType };

            // Assert
            rebateLog.RebateType.Should().Be(rebateType);
        }

        [Fact]
        public void RebateMLogModel_ShouldHandleMultipleCurrencyScenario()
        {
            // Arrange
            var rebateLogs = new[]
            {
                new RebateMLogModel { CurrencyId = 1, RedeemAmount = 100.00m }, // USD
                new RebateMLogModel { CurrencyId = 2, RedeemAmount = 85.50m },  // EUR
                new RebateMLogModel { CurrencyId = 3, RedeemAmount = 110.25m }  // GBP
            };

            // Assert
            rebateLogs[0].CurrencyId.Should().Be(1);
            rebateLogs[1].CurrencyId.Should().Be(2);
            rebateLogs[2].CurrencyId.Should().Be(3);
            rebateLogs.All(r => r.RedeemAmount > 0).Should().BeTrue();
        }
    }
}