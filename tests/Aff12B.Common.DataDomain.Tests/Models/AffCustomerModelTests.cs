using Aff12B.Common.DataDomain.Models;
using Aff12B.Common.Tests.Builders;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for AffCustomerModel.
    /// Reduced from 372 lines to focus on meaningful business scenarios.
    /// </summary>
    public class AffCustomerModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var affCustomer = new AffCustomerModel();

            // Assert
            affCustomer.Should().NotBeNull();
            affCustomer.SiteId.Should().Be(0);
            affCustomer.CustId.Should().Be(0);
            affCustomer.SiteName.Should().BeNull();
            affCustomer.UserName.Should().BeNull();
            affCustomer.RegTime.Should().BeNull();
            affCustomer.FirstDepositDate.Should().BeNull();
            affCustomer.IsTest.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var regTime = DateTime.UtcNow.AddDays(-30);
            var firstDepositDate = DateTime.UtcNow.AddDays(-15);

            // Act
            var affCustomer = new AffCustomerModel
            {
                SiteId = 1,
                SiteName = "TestSite",
                CustId = 12345,
                UserName = "testuser",
                CurrencyId = 2,
                ActStatus = 1,
                RegTime = regTime,
                FirstDepositDate = firstDepositDate,
                Status = 1,
                AffId = 100,
                IsTest = false,
                AffCode = "AFF001",
                ClickId = 98765L,
                TagList = "vip,highroller"
            };

            // Assert
            affCustomer.SiteId.Should().Be(1);
            affCustomer.SiteName.Should().Be("TestSite");
            affCustomer.CustId.Should().Be(12345);
            affCustomer.UserName.Should().Be("testuser");
            affCustomer.CurrencyId.Should().Be(2);
            affCustomer.ActStatus.Should().Be(1);
            affCustomer.RegTime.Should().Be(regTime);
            affCustomer.FirstDepositDate.Should().Be(firstDepositDate);
            affCustomer.Status.Should().Be(1);
            affCustomer.AffId.Should().Be(100);
            affCustomer.IsTest.Should().Be(false);
            affCustomer.AffCode.Should().Be("AFF001");
            affCustomer.ClickId.Should().Be(98765L);
            affCustomer.TagList.Should().Be("vip,highroller");
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(1, "TestSite", "testuser")]
        [InlineData(999, "", "")]
        [InlineData(-1, "Special Site", "special_user")]
        public void BasicProperties_ShouldHandleVariousScenarios(int? siteId, string siteName, string userName)
        {
            // Act
            var affCustomer = new AffCustomerModel
            {
                SiteId = siteId ?? 0,
                SiteName = siteName,
                UserName = userName
            };

            // Assert
            affCustomer.SiteId.Should().Be(siteId ?? 0);
            affCustomer.SiteName.Should().Be(siteName);
            affCustomer.UserName.Should().Be(userName);
        }

        [Fact]
        public void AffCustomerModel_ShouldWorkWithBuilder()
        {
            // Act
            var affCustomer = new AffCustomerModelBuilder()
                .WithCustId(12345)
                .WithUserName("testuser")
                .WithClickId(98765L)
                .FirstDepositDaysAgo(7)
                .RegisteredDaysAgo(30)
                .Build();

            // Assert
            affCustomer.Should().NotBeNull();
            affCustomer.CustId.Should().Be(12345);
            affCustomer.UserName.Should().Be("testuser");
            affCustomer.ClickId.Should().Be(98765L);
            affCustomer.FirstDepositDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-7), TimeSpan.FromSeconds(1));
            affCustomer.RegTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(-30), TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AffCustomerModel_ShouldHandleBusinessScenario()
        {
            // Arrange - Real affiliate customer scenario
            var affCustomer = new AffCustomerModel
            {
                SiteId = 12,
                SiteName = "BetSite",
                CustId = 98765,
                UserName = "high_roller_123",
                CurrencyId = 1, // USD
                ActStatus = 1, // Active
                RegTime = DateTime.UtcNow.AddMonths(-6),
                FirstDepositDate = DateTime.UtcNow.AddMonths(-5),
                Status = 1, // Active
                AffId = 500,
                IsTest = false,
                AffCode = "PARTNER_ABC",
                ClickId = 1234567890L,
                TagList = "vip,high_roller,premium"
            };

            // Assert - Business validations
            affCustomer.SiteId.Should().BePositive();
            affCustomer.CustId.Should().BePositive();
            affCustomer.RegTime.Should().BeBefore(affCustomer.FirstDepositDate.Value);
            affCustomer.IsTest.Should().BeFalse();
            affCustomer.TagList.Should().Contain("vip");
            affCustomer.AffCode.Should().StartWith("PARTNER");
        }

        [Fact]
        public void AffCustomerModel_ShouldHandleNullableProperties()
        {
            // Act
            var affCustomer = new AffCustomerModel
            {
                CurrencyId = null,
                ActStatus = null,
                RegTime = null,
                FirstDepositDate = null,
                Status = null,
                AffId = null,
                IsTest = null,
                ClickId = null
            };

            // Assert
            affCustomer.CurrencyId.Should().BeNull();
            affCustomer.ActStatus.Should().BeNull();
            affCustomer.RegTime.Should().BeNull();
            affCustomer.FirstDepositDate.Should().BeNull();
            affCustomer.Status.Should().BeNull();
            affCustomer.AffId.Should().BeNull();
            affCustomer.IsTest.Should().BeNull();
            affCustomer.ClickId.Should().BeNull();
        }
    }
}