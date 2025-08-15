using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for CustomerModel class.
    /// Reduced from 244 lines to focus on meaningful business scenarios.
    /// Customer model with agent hierarchy, status tracking, and tag management.
    /// </summary>
    public class CustomerModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var customer = new CustomerModel();

            // Assert
            customer.Should().NotBeNull();
            customer.CustId.Should().Be(0);
            customer.SiteId.Should().BeNull();
            customer.UserName.Should().BeNull();
            customer.CurrencyId.Should().BeNull();
            customer.UserLang.Should().BeNull();
            customer.ActStatus.Should().BeNull();
            customer.AgentId.Should().BeNull();
            customer.AllAgentId.Should().BeNull();
            customer.UserLevel.Should().BeNull();
            customer.RegTime.Should().BeNull();
            customer.IsTest.Should().BeNull();
            customer.Status.Should().BeNull();
            customer.MemberType.Should().Be(0);
            customer.TagList.Should().BeNull();
            customer.LastTagUpdateTime.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var regTime = DateTime.UtcNow.AddDays(-30);
            var tagUpdateTime = DateTime.UtcNow.AddDays(-1);

            // Act
            var customer = new CustomerModel
            {
                CustId = 12345,
                SiteId = 100,
                UserName = "premium_player",
                CurrencyId = 1,
                UserLang = "en",
                ActStatus = 1,
                AgentId = 500,
                AllAgentId = "100,200,300,500",
                UserLevel = 3,
                RegTime = regTime,
                IsTest = false,
                Status = 1,
                MemberType = 2,
                TagList = "vip,high_roller,loyal",
                LastTagUpdateTime = tagUpdateTime
            };

            // Assert
            customer.CustId.Should().Be(12345);
            customer.SiteId.Should().Be(100);
            customer.UserName.Should().Be("premium_player");
            customer.CurrencyId.Should().Be(1);
            customer.UserLang.Should().Be("en");
            customer.ActStatus.Should().Be(1);
            customer.AgentId.Should().Be(500);
            customer.AllAgentId.Should().Be("100,200,300,500");
            customer.UserLevel.Should().Be(3);
            customer.RegTime.Should().Be(regTime);
            customer.IsTest.Should().BeFalse();
            customer.Status.Should().Be(1);
            customer.MemberType.Should().Be(2);
            customer.TagList.Should().Be("vip,high_roller,loyal");
            customer.LastTagUpdateTime.Should().Be(tagUpdateTime);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ActStatus_ShouldSupportVariousStates(int actStatus)
        {
            // Act
            var customer = new CustomerModel { ActStatus = actStatus };

            // Assert
            customer.ActStatus.Should().Be(actStatus);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void UserLevel_ShouldSupportVariousLevels(int userLevel)
        {
            // Act
            var customer = new CustomerModel { UserLevel = userLevel };

            // Assert
            customer.UserLevel.Should().Be(userLevel);
        }

        [Fact]
        public void CustomerModel_ShouldSupportVIPCustomerScenario()
        {
            // Arrange - VIP customer scenario
            var customer = new CustomerModel
            {
                CustId = 88888,
                SiteId = 500, // VIP site
                UserName = "vip_whale_player",
                CurrencyId = 1, // USD
                UserLang = "en",
                ActStatus = 1, // Active
                AgentId = 1000, // VIP agent
                AllAgentId = "1,10,100,1000", // Agent hierarchy
                UserLevel = 5, // Highest level
                RegTime = DateTime.UtcNow.AddYears(-2), // Long-term customer
                IsTest = false,
                Status = 1, // Active
                MemberType = 3, // VIP member type
                TagList = "vip,whale,high_roller,loyal,platinum",
                LastTagUpdateTime = DateTime.UtcNow.AddDays(-7)
            };

            // Assert - VIP customer validations
            customer.UserLevel.Should().BeGreaterOrEqualTo(4); // High level
            customer.MemberType.Should().BeGreaterOrEqualTo(2); // Premium member type
            customer.SiteId.Should().BeGreaterOrEqualTo(500); // VIP site
            customer.TagList.Should().Contain("vip");
            customer.TagList.Should().Contain("high_roller");
            customer.UserName.Should().Contain("vip");
            customer.IsTest.Should().BeFalse(); // Real customer
            customer.RegTime.Should().BeBefore(DateTime.UtcNow.AddMonths(-6)); // Established customer
        }

        [Fact]
        public void CustomerModel_ShouldSupportNewCustomerScenario()
        {
            // Arrange - New customer scenario
            var customer = new CustomerModel
            {
                CustId = 99999,
                SiteId = 100,
                UserName = "new_player_2024",
                CurrencyId = 1,
                UserLang = "en",
                ActStatus = 1, // Active
                AgentId = 200,
                AllAgentId = "1,50,200",
                UserLevel = 1, // Starting level
                RegTime = DateTime.UtcNow.AddDays(-3), // Recently registered
                IsTest = false,
                Status = 1,
                MemberType = 1, // Regular member
                TagList = "new_player,welcome_bonus_eligible",
                LastTagUpdateTime = DateTime.UtcNow.AddDays(-3)
            };

            // Assert - New customer validations
            customer.UserLevel.Should().Be(1); // Starting level
            customer.MemberType.Should().Be(1); // Regular member
            customer.TagList.Should().Contain("new_player");
            customer.TagList.Should().Contain("welcome_bonus_eligible");
            customer.RegTime.Should().BeAfter(DateTime.UtcNow.AddDays(-7)); // Recent registration
            customer.LastTagUpdateTime.Should().BeCloseTo(customer.RegTime.Value, TimeSpan.FromDays(1));
        }

        [Fact]
        public void CustomerModel_ShouldSupportInactiveCustomerScenario()
        {
            // Arrange - Inactive customer scenario
            var customer = new CustomerModel
            {
                CustId = 77777,
                SiteId = 100,
                UserName = "dormant_player",
                CurrencyId = 1,
                UserLang = "en",
                ActStatus = 0, // Inactive
                AgentId = 300,
                AllAgentId = "1,30,300",
                UserLevel = 2,
                RegTime = DateTime.UtcNow.AddYears(-1),
                IsTest = false,
                Status = 2, // Inactive status
                MemberType = 1,
                TagList = "inactive,reactivation_target",
                LastTagUpdateTime = DateTime.UtcNow.AddDays(-90)
            };

            // Assert - Inactive customer validations
            customer.ActStatus.Should().Be(0); // Inactive
            customer.Status.Should().Be(2); // Inactive status
            customer.TagList.Should().Contain("inactive");
            customer.TagList.Should().Contain("reactivation_target");
            customer.LastTagUpdateTime.Should().BeBefore(DateTime.UtcNow.AddDays(-30)); // Old tag update
        }

        [Fact]
        public void CustomerModel_ShouldSupportTestCustomerScenario()
        {
            // Arrange - Test customer scenario
            var customer = new CustomerModel
            {
                CustId = 11111,
                SiteId = 999, // Test site
                UserName = "test_user_qa",
                CurrencyId = 1,
                UserLang = "en",
                ActStatus = 1,
                AgentId = 9999, // Test agent
                AllAgentId = "9999",
                UserLevel = 1,
                RegTime = DateTime.UtcNow.AddDays(-1),
                IsTest = true, // Test customer
                Status = 1,
                MemberType = 0, // Test member type
                TagList = "test_account,qa_testing",
                LastTagUpdateTime = DateTime.UtcNow
            };

            // Assert - Test customer validations
            customer.IsTest.Should().BeTrue(); // Test account
            customer.SiteId.Should().BeGreaterOrEqualTo(999); // Test site
            customer.AgentId.Should().BeGreaterOrEqualTo(9999); // Test agent
            customer.TagList.Should().Contain("test_account");
            customer.UserName.Should().Contain("test");
            customer.MemberType.Should().Be(0); // Test member type
        }

        [Fact]
        public void CustomerModel_ShouldSupportAgentHierarchyScenario()
        {
            // Arrange - Complex agent hierarchy
            var customer = new CustomerModel
            {
                CustId = 55555,
                AgentId = 12345, // Direct agent
                AllAgentId = "1,100,1000,5000,12345", // Full hierarchy
                UserName = "agent_referred_player"
            };

            // Assert - Agent hierarchy validations
            customer.AllAgentId.Should().Contain(customer.AgentId.ToString());
            customer.AllAgentId.Should().Contain(","); // Multiple agents
            var agentIds = customer.AllAgentId.Split(',');
            agentIds.Should().HaveCountGreaterThan(1);
            agentIds.Last().Should().Be(customer.AgentId.ToString()); // Direct agent is last
        }

        [Fact]
        public void CustomerModel_ShouldSupportMultiLanguageScenario()
        {
            // Arrange - Multi-language customer
            var customer = new CustomerModel
            {
                CustId = 66666,
                UserName = "国际玩家_International_Player",
                UserLang = "zh-CN",
                TagList = "chinese_speaker,asia_region,高价值客户"
            };

            // Assert - Multi-language validations
            customer.UserLang.Should().Be("zh-CN");
            customer.UserName.Should().Contain("国际");
            customer.UserName.Should().Contain("International");
            customer.TagList.Should().Contain("chinese_speaker");
            customer.TagList.Should().Contain("高价值客户");
        }

        [Fact]
        public void CustomerModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var customers = new[]
            {
                new CustomerModel { CustId = 1, UserLevel = 5, MemberType = 3, IsTest = false },
                new CustomerModel { CustId = 2, UserLevel = 1, MemberType = 1, IsTest = false },
                new CustomerModel { CustId = 3, UserLevel = 3, MemberType = 2, IsTest = true }
            };

            // Act & Assert
            customers.Should().HaveCount(3);
            customers.Where(c => c.IsTest == false).Should().HaveCount(2); // Real customers
            customers.Where(c => c.UserLevel >= 3).Should().HaveCount(2); // High level customers
            customers.Where(c => c.MemberType >= 2).Should().HaveCount(2); // Premium members
            customers.Max(c => c.UserLevel).Should().Be(5); // Highest level
        }

        [Fact]
        public void CustomerModel_ShouldSupportTagManagement()
        {
            // Arrange
            var customer = new CustomerModel
            {
                TagList = "vip,high_roller,loyal,bonus_eligible,weekend_warrior",
                LastTagUpdateTime = DateTime.UtcNow.AddDays(-5)
            };

            // Act
            var tags = customer.TagList.Split(',');

            // Assert - Tag management validations
            tags.Should().HaveCount(5);
            tags.Should().Contain("vip");
            tags.Should().Contain("high_roller");
            tags.Should().Contain("loyal");
            customer.LastTagUpdateTime.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void CustomerModel_ShouldHandleNullableProperties()
        {
            // Act
            var customer = new CustomerModel
            {
                SiteId = null,
                CurrencyId = null,
                ActStatus = null,
                AgentId = null,
                UserLevel = null,
                RegTime = null,
                IsTest = null,
                Status = null,
                LastTagUpdateTime = null
            };

            // Assert
            customer.SiteId.Should().BeNull();
            customer.CurrencyId.Should().BeNull();
            customer.ActStatus.Should().BeNull();
            customer.AgentId.Should().BeNull();
            customer.UserLevel.Should().BeNull();
            customer.RegTime.Should().BeNull();
            customer.IsTest.Should().BeNull();
            customer.Status.Should().BeNull();
            customer.LastTagUpdateTime.Should().BeNull();
        }

        [Theory]
        [InlineData("en", 1, 1)]
        [InlineData("zh-CN", 1, 1)]
        [InlineData("es", 1, 1)]
        [InlineData("ja", 1, 1)]
        public void CustomerModel_ShouldSupportInternationalization(string userLang, int currencyId, int siteId)
        {
            // Act
            var customer = new CustomerModel
            {
                UserLang = userLang,
                CurrencyId = currencyId,
                SiteId = siteId
            };

            // Assert
            customer.UserLang.Should().Be(userLang);
            customer.CurrencyId.Should().Be(currencyId);
            customer.SiteId.Should().Be(siteId);
            customer.UserLang.Length.Should().BeOneOf(2, 5); // "en" or "zh-CN" format
        }

        [Fact]
        public void CustomerModel_ShouldSupportBusinessRuleValidations()
        {
            // Arrange - Business rule: VIP customers should have high user level
            var vipCustomer = new CustomerModel
            {
                TagList = "vip,platinum",
                UserLevel = 5,
                MemberType = 3
            };

            // Arrange - Business rule: Test customers should be on test sites
            var testCustomer = new CustomerModel
            {
                IsTest = true,
                SiteId = 999,
                MemberType = 0
            };

            // Assert - Business rule validations
            if (vipCustomer.TagList?.Contains("vip") == true)
            {
                vipCustomer.UserLevel.Should().BeGreaterOrEqualTo(3); // VIP should have high level
                vipCustomer.MemberType.Should().BeGreaterOrEqualTo(2); // VIP should be premium member
            }

            if (testCustomer.IsTest == true)
            {
                testCustomer.SiteId.Should().BeGreaterOrEqualTo(999); // Test customers on test sites
                testCustomer.MemberType.Should().Be(0); // Test member type
            }
        }
    }
}