using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for DepositInfoModel class.
    /// Reduced from 414 lines to focus on meaningful business scenarios.
    /// Deposit transaction model with third-party integration support.
    /// </summary>
    public class DepositInfoModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var deposit = new DepositInfoModel();

            // Assert
            deposit.Should().NotBeNull();
            deposit.DepositId.Should().Be(0);
            deposit.TransId.Should().BeNull();
            deposit.DepositType.Should().BeNull();
            deposit.CustId.Should().BeNull();
            deposit.UserName.Should().BeNull();
            deposit.Amount.Should().BeNull();
            deposit.ActualDeposit.Should().BeNull();
            deposit.CreateTime.Should().BeNull();
            deposit.TPCreateTime.Should().BeNull();
            deposit.CallbackTime.Should().BeNull();
            deposit.FirstDeposit.Should().BeNull();
            deposit.Status.Should().BeNull();
            deposit.Flag.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTime = DateTime.UtcNow.AddHours(-2);
            var tpCreateTime = DateTime.UtcNow.AddHours(-2);
            var callbackTime = DateTime.UtcNow.AddHours(-1);
            var depositTime = DateTime.UtcNow.AddMinutes(-30);
            var updateTime = DateTime.UtcNow;

            // Act
            var deposit = new DepositInfoModel
            {
                DepositId = 12345,
                TransId = "TXN_DEP_001",
                DepositType = 1,
                CustId = 67890,
                UserName = "premium_user",
                CurrencyId = 1,
                SiteId = 100,
                DespositInfo = "Credit Card Deposit",
                Amount = 500.00m,
                ActualDeposit = 495.00m,
                CreateTime = createTime,
                TPCreateTime = tpCreateTime,
                CallbackTime = callbackTime,
                FirstDeposit = 1,
                ThirdId = "THIRD_12345",
                RefId = "REF_CC_001",
                DepositTime = depositTime,
                Memo = "First deposit bonus eligible",
                PostScript = "Processed via Visa",
                Comment = "Successful transaction",
                Status = 2,
                Flag = 1,
                Operator = "SYSTEM",
                UpdateTime = updateTime
            };

            // Assert
            deposit.DepositId.Should().Be(12345);
            deposit.TransId.Should().Be("TXN_DEP_001");
            deposit.DepositType.Should().Be(1);
            deposit.CustId.Should().Be(67890);
            deposit.UserName.Should().Be("premium_user");
            deposit.Amount.Should().Be(500.00m);
            deposit.ActualDeposit.Should().Be(495.00m);
            deposit.CreateTime.Should().Be(createTime);
            deposit.TPCreateTime.Should().Be(tpCreateTime);
            deposit.CallbackTime.Should().Be(callbackTime);
            deposit.FirstDeposit.Should().Be(1);
            deposit.Status.Should().Be(2);
            deposit.Flag.Should().Be(1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void DepositType_ShouldRepresentVariousPaymentMethods(int type)
        {
            // Act
            var deposit = new DepositInfoModel { DepositType = type };

            // Assert
            deposit.DepositType.Should().Be(type);

        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Status_ShouldRepresentDepositStates(int status)
        {
            // Act
            var deposit = new DepositInfoModel { Status = status };

            // Assert
            deposit.Status.Should().Be(status);

        }

        [Fact]
        public void DepositInfoModel_ShouldSupportFirstDepositScenario()
        {
            // Arrange - First deposit with bonus eligibility
            var deposit = new DepositInfoModel
            {
                DepositId = 98765,
                TransId = "FIRST_DEP_001",
                DepositType = 1, // Credit card
                CustId = 12345,
                UserName = "new_player",
                CurrencyId = 1, // USD
                SiteId = 200,
                DespositInfo = "Welcome Bonus Deposit - Visa ending 4567",
                Amount = 100.00m,
                ActualDeposit = 100.00m, // No fees for first deposit
                CreateTime = DateTime.UtcNow.AddHours(-1),
                TPCreateTime = DateTime.UtcNow.AddHours(-1),
                CallbackTime = DateTime.UtcNow.AddMinutes(-30),
                FirstDeposit = 1, // Yes, first deposit
                ThirdId = "VISA_TXN_789",
                RefId = "REF_WELCOME_001",
                DepositTime = DateTime.UtcNow.AddMinutes(-25),
                Memo = "First deposit - bonus eligible",
                Comment = "Welcome bonus applied",
                Status = 2, // Completed
                Flag = 1, // Active
                Operator = "AUTO_SYSTEM"
            };

            // Assert - First deposit validations
            deposit.FirstDeposit.Should().Be(1);
            deposit.Amount.Should().Be(deposit.ActualDeposit); // No fees
            deposit.Status.Should().Be(2); // Completed
            deposit.TransId.Should().StartWith("FIRST_");
            deposit.Memo.Should().Contain("bonus eligible");
            deposit.Comment.Should().Contain("bonus applied");
            deposit.DespositInfo.Should().Contain("Welcome Bonus");
        }

        [Fact]
        public void DepositInfoModel_ShouldSupportThirdPartyIntegration()
        {
            // Arrange - Third-party payment integration
            var tpCreateTime = DateTime.UtcNow.AddHours(-2);
            var callbackTime = DateTime.UtcNow.AddHours(-1);

            var deposit = new DepositInfoModel
            {
                DepositId = 55555,
                TransId = "TP_INTEGRATION_001",
                DepositType = 3, // E-wallet
                CustId = 88888,
                UserName = "ewallet_user",
                Amount = 250.00m,
                ActualDeposit = 245.00m, // 5.00 fee
                TPCreateTime = tpCreateTime,
                CallbackTime = callbackTime,
                ThirdId = "PAYPAL_TXN_456789",
                RefId = "REF_PAYPAL_001",
                DespositInfo = "PayPal Integration",
                PostScript = "Processed via PayPal API v2",
                Status = 2,
                Operator = "PAYPAL_GATEWAY"
            };

            // Assert - Third-party integration validations
            deposit.ThirdId.Should().StartWith("PAYPAL_");
            deposit.TPCreateTime.Should().NotBeNull();
            deposit.CallbackTime.Should().NotBeNull();
            deposit.CallbackTime.Should().BeAfter(deposit.TPCreateTime.Value);
            deposit.ActualDeposit.Should().BeLessThan(deposit.Amount.Value); // Fee deducted
            deposit.PostScript.Should().Contain("API");
            deposit.Operator.Should().Contain("GATEWAY");
        }

        [Fact]
        public void DepositInfoModel_ShouldSupportFailedDepositScenario()
        {
            // Arrange - Failed deposit scenario
            var deposit = new DepositInfoModel
            {
                DepositId = 99999,
                TransId = "FAILED_DEP_001",
                DepositType = 1,
                CustId = 11111,
                UserName = "unlucky_user",
                Amount = 1000.00m,
                ActualDeposit = 0.00m, // No actual deposit due to failure
                CreateTime = DateTime.UtcNow.AddHours(-3),
                TPCreateTime = DateTime.UtcNow.AddHours(-3),
                CallbackTime = DateTime.UtcNow.AddHours(-2),
                ThirdId = "FAILED_CC_001",
                Status = 3, // Failed
                Flag = 0, // Inactive
                Comment = "Insufficient funds",
                Memo = "Card declined by issuer"
            };

            // Assert - Failed deposit validations
            deposit.Status.Should().Be(3); // Failed
            deposit.ActualDeposit.Should().Be(0); // No money deposited
            deposit.Flag.Should().Be(0); // Inactive
            deposit.Comment.Should().Contain("Insufficient");
            deposit.Memo.Should().Contain("declined");
            deposit.TransId.Should().StartWith("FAILED_");
        }

        [Fact]
        public void DepositInfoModel_ShouldSupportHighRollerDeposit()
        {
            // Arrange - High roller deposit
            var deposit = new DepositInfoModel
            {
                DepositId = 777777,
                TransId = "VIP_DEP_001",
                DepositType = 2, // Bank transfer
                CustId = 99999,
                UserName = "high_roller_vip",
                CurrencyId = 1,
                SiteId = 500, // VIP site
                Amount = 50000.00m,
                ActualDeposit = 50000.00m, // No fees for VIP
                FirstDeposit = 0, // Not first deposit
                DespositInfo = "VIP Bank Transfer - Chase Private Client",
                Status = 2,
                Flag = 1,
                Memo = "VIP customer - priority processing",
                Comment = "Manual verification completed",
                Operator = "VIP_MANAGER"
            };

            // Assert - High roller validations
            deposit.Amount.Should().BeGreaterThan(10000); // High amount
            deposit.ActualDeposit.Should().Be(deposit.Amount); // No fees for VIP
            deposit.SiteId.Should().Be(500); // VIP site
            deposit.UserName.Should().Contain("vip");
            deposit.Memo.Should().Contain("VIP");
            deposit.Operator.Should().Contain("VIP_MANAGER");
        }

        [Fact]
        public void DepositInfoModel_ShouldHandleNullableProperties()
        {
            // Act
            var deposit = new DepositInfoModel
            {
                DepositType = null,
                CustId = null,
                CurrencyId = null,
                SiteId = null,
                Amount = null,
                ActualDeposit = null,
                CreateTime = null,
                TPCreateTime = null,
                CallbackTime = null,
                FirstDeposit = null,
                DepositTime = null,
                Status = null,
                Flag = null,
                UpdateTime = null
            };

            // Assert
            deposit.DepositType.Should().BeNull();
            deposit.CustId.Should().BeNull();
            deposit.Amount.Should().BeNull();
            deposit.ActualDeposit.Should().BeNull();
            deposit.CreateTime.Should().BeNull();
            deposit.Status.Should().BeNull();
        }

        [Fact]
        public void DepositInfoModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var deposits = new[]
            {
                new DepositInfoModel { DepositId = 1, Amount = 100.00m, Status = 2, FirstDeposit = 1 },
                new DepositInfoModel { DepositId = 2, Amount = 250.00m, Status = 2, FirstDeposit = 0 },
                new DepositInfoModel { DepositId = 3, Amount = 500.00m, Status = 3, FirstDeposit = 0 }
            };

            // Act & Assert
            deposits.Should().HaveCount(3);
            deposits.Where(d => d.Status == 2).Should().HaveCount(2); // Successful deposits
            deposits.Where(d => d.FirstDeposit == 1).Should().HaveCount(1); // First deposits
            deposits.Sum(d => d.Amount ?? 0).Should().Be(850.00m); // Total attempted
            deposits.Where(d => d.Status == 2).Sum(d => d.Amount ?? 0).Should().Be(350.00m); // Successful amount
        }

        [Theory]
        [InlineData(100.00, 2.00, 98.00)]
        [InlineData(500.00, 10.00, 490.00)]
        [InlineData(1000.00, 0.00, 1000.00)]
        public void DepositCalculation_ShouldCalculateActualDeposit(decimal amount, decimal fee, decimal expectedActual)
        {
            // Arrange
            var deposit = new DepositInfoModel
            {
                Amount = amount,
                ActualDeposit = amount - fee
            };

            // Act & Assert
            deposit.ActualDeposit.Should().Be(expectedActual);
            if (fee > 0)
            {
                deposit.ActualDeposit.Should().BeLessThan(deposit.Amount.Value);
            }
            else
            {
                deposit.ActualDeposit.Should().Be(deposit.Amount.Value);
            }
        }
    }
}