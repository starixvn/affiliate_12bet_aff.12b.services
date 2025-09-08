using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for WithdrawalInfoModel class.
    /// Reduced from 492 lines to focus on meaningful business scenarios.
    /// Implements IHasUpdateTime interface.
    /// </summary>
    public class WithdrawalInfoModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var withdrawal = new WithdrawalInfoModel();

            // Assert
            withdrawal.Should().NotBeNull();
            withdrawal.WithdrawalId.Should().Be(0);
            withdrawal.TransId.Should().BeNull();
            withdrawal.WithdrawalType.Should().BeNull();
            withdrawal.CustId.Should().BeNull();
            withdrawal.UserName.Should().BeNull();
            withdrawal.Amount.Should().BeNull();
            withdrawal.CreateTime.Should().BeNull();
            withdrawal.UpdateTime.Should().BeNull();
            withdrawal.Status.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTime = DateTime.UtcNow.AddHours(-2);
            var updateTime = DateTime.UtcNow.AddHours(-1);

            // Act
            var withdrawal = new WithdrawalInfoModel
            {
                WithdrawalId = 12345,
                TransId = "TXN_12345",
                WithdrawalType = 1,
                CustId = 67890,
                UserName = "testuser",
                CurrencyId = 1,
                SiteID = 100,
                WithdrawalInfo = "Bank Transfer",
                Amount = 500.00m,
                RefId = "REF_98765",
                ChangeFee = 5.00m,
                CreateTime = createTime,
                UpdateTime = updateTime,
                Memo = "Withdrawal request",
                Comment = "Processed successfully",
                Status = 2
            };

            // Assert
            withdrawal.WithdrawalId.Should().Be(12345);
            withdrawal.TransId.Should().Be("TXN_12345");
            withdrawal.WithdrawalType.Should().Be(1);
            withdrawal.CustId.Should().Be(67890);
            withdrawal.UserName.Should().Be("testuser");
            withdrawal.CurrencyId.Should().Be(1);
            withdrawal.SiteID.Should().Be(100);
            withdrawal.WithdrawalInfo.Should().Be("Bank Transfer");
            withdrawal.Amount.Should().Be(500.00m);
            withdrawal.RefId.Should().Be("REF_98765");
            withdrawal.ChangeFee.Should().Be(5.00m);
            withdrawal.CreateTime.Should().Be(createTime);
            withdrawal.UpdateTime.Should().Be(updateTime);
            withdrawal.Memo.Should().Be("Withdrawal request");
            withdrawal.Comment.Should().Be("Processed successfully");
            withdrawal.Status.Should().Be(2);
        }

        [Fact]
        public void IHasUpdateTime_ShouldImplementInterfaceCorrectly()
        {
            // Arrange
            var updateTime = DateTime.UtcNow;
            var withdrawal = new WithdrawalInfoModel { UpdateTime = updateTime };

            // Act - Cast to interface
            IHasUpdateTime hasUpdateTime = withdrawal;

            // Assert
            hasUpdateTime.UpdateTime.Should().Be(updateTime);
            withdrawal.Should().BeAssignableTo<IHasUpdateTime>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void WithdrawalType_ShouldRepresentDifferentWithdrawalMethods(int type)
        {
            // Act
            var withdrawal = new WithdrawalInfoModel { WithdrawalType = type };

            // Assert
            withdrawal.WithdrawalType.Should().Be(type);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Status_ShouldRepresentDifferentWithdrawalStates(int status)
        {
            // Act
            var withdrawal = new WithdrawalInfoModel { Status = status };

            // Assert
            withdrawal.Status.Should().Be(status);
        }

        [Fact]
        public void WithdrawalInfoModel_ShouldSupportBusinessScenario()
        {
            // Arrange - Complete withdrawal scenario
            var withdrawal = new WithdrawalInfoModel
            {
                WithdrawalId = 98765,
                TransId = "TXN_BANK_001",
                WithdrawalType = 1, // Bank transfer
                CustId = 12345,
                UserName = "premium_user",
                CurrencyId = 1, // USD
                SiteID = 200,
                WithdrawalInfo = "Chase Bank - Account ending 1234",
                Amount = 1000.00m,
                RefId = "REF_CHASE_001",
                ChangeFee = 15.00m,
                CreateTime = DateTime.UtcNow.AddHours(-3),
                UpdateTime = DateTime.UtcNow.AddHours(-1),
                Memo = "Large withdrawal request",
                Comment = "Verified and processed",
                Status = 2 // Completed
            };

            // Assert - Business validations
            withdrawal.WithdrawalId.Should().BePositive();
            withdrawal.CustId.Should().BePositive();
            withdrawal.Amount.Should().BePositive();
            withdrawal.ChangeFee.Should().BePositive();
            withdrawal.CreateTime.Should().BeBefore(withdrawal.UpdateTime.Value);
            withdrawal.TransId.Should().StartWith("TXN_");
            withdrawal.RefId.Should().StartWith("REF_");
            withdrawal.WithdrawalInfo.Should().Contain("Account");
            withdrawal.Status.Should().Be(2); // Completed
        }

        [Fact]
        public void WithdrawalInfoModel_ShouldHandleNullableProperties()
        {
            // Act
            var withdrawal = new WithdrawalInfoModel
            {
                WithdrawalType = null,
                CustId = null,
                CurrencyId = null,
                SiteID = null,
                Amount = null,
                ChangeFee = null,
                CreateTime = null,
                UpdateTime = null,
                Status = null
            };

            // Assert
            withdrawal.WithdrawalType.Should().BeNull();
            withdrawal.CustId.Should().BeNull();
            withdrawal.CurrencyId.Should().BeNull();
            withdrawal.SiteID.Should().BeNull();
            withdrawal.Amount.Should().BeNull();
            withdrawal.ChangeFee.Should().BeNull();
            withdrawal.CreateTime.Should().BeNull();
            withdrawal.UpdateTime.Should().BeNull();
            withdrawal.Status.Should().BeNull();
        }

        [Theory]
        [InlineData(100.00, 5.00, 95.00)]
        [InlineData(1000.00, 25.00, 975.00)]
        [InlineData(50.00, 2.50, 47.50)]
        public void WithdrawalCalculation_ShouldCalculateNetAmount(decimal amount, decimal fee, decimal expectedNet)
        {
            // Arrange
            var withdrawal = new WithdrawalInfoModel
            {
                Amount = amount,
                ChangeFee = fee
            };

            // Act
            var netAmount = withdrawal.Amount - withdrawal.ChangeFee;

            // Assert
            netAmount.Should().Be(expectedNet);
        }

        [Fact]
        public void WithdrawalInfoModel_ShouldHandleUnicodeContent()
        {
            // Act
            var withdrawal = new WithdrawalInfoModel
            {
                UserName = "用户_José_García",
                WithdrawalInfo = "銀行转账_Transferencia_Bancária",
                Memo = "提款备注_Nota_de_retiro_💰",
                Comment = "处理完成_Procesado_exitosamente_✅"
            };

            // Assert
            withdrawal.UserName.Should().Contain("José");
            withdrawal.WithdrawalInfo.Should().Contain("Transferencia");
            withdrawal.Memo.Should().Contain("💰");
            withdrawal.Comment.Should().Contain("✅");
        }

        [Fact]
        public void WithdrawalInfoModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var withdrawals = new[]
            {
                new WithdrawalInfoModel { WithdrawalId = 1, Amount = 100.00m, Status = 2 },
                new WithdrawalInfoModel { WithdrawalId = 2, Amount = 250.00m, Status = 2 },
                new WithdrawalInfoModel { WithdrawalId = 3, Amount = 75.00m, Status = 1 }
            };

            // Act & Assert
            withdrawals.Should().HaveCount(3);
            withdrawals.Where(w => w.Status == 2).Should().HaveCount(2); // Completed
            withdrawals.Sum(w => w.Amount ?? 0).Should().Be(425.00m); // Total amount
            withdrawals.Max(w => w.WithdrawalId).Should().Be(3);
        }
    }
}