using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for TagModel class.
    /// Reduced from 274 lines to focus on meaningful business scenarios.
    /// Tag model implementing IHasUpdateTime with status and abnormal flag support.
    /// </summary>
    public class TagModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var tag = new TagModel();

            // Assert
            tag.Should().NotBeNull();
            tag.Id.Should().Be(0);
            tag.TagName.Should().BeNull();
            tag.Description.Should().BeNull();
            tag.Status.Should().Be(0);
            tag.UpdateTime.Should().BeNull();
            tag.IsAbnormal.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var updateTime = DateTime.UtcNow;

            // Act
            var tag = new TagModel
            {
                Id = 12345,
                TagName = "VIP_PLAYER",
                Description = "High value VIP customer",
                Status = 1,
                UpdateTime = updateTime,
                IsAbnormal = 0
            };

            // Assert
            tag.Id.Should().Be(12345);
            tag.TagName.Should().Be("VIP_PLAYER");
            tag.Description.Should().Be("High value VIP customer");
            tag.Status.Should().Be(1);
            tag.UpdateTime.Should().Be(updateTime);
            tag.IsAbnormal.Should().Be(0);
        }

        [Fact]
        public void TagModel_ShouldImplementIHasUpdateTime()
        {
            // Act
            var tag = new TagModel();

            // Assert
            tag.Should().BeAssignableTo<IHasUpdateTime>();
            tag.UpdateTime.Should().BeNull(); // Nullable DateTime
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Status_ShouldSupportVariousStates(int status)
        {
            // Act
            var tag = new TagModel { Status = status };

            // Assert
            tag.Status.Should().Be(status);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void IsAbnormal_ShouldSupportBinaryStates(byte isAbnormal)
        {
            // Act
            var tag = new TagModel { IsAbnormal = isAbnormal };

            // Assert
            tag.IsAbnormal.Should().Be(isAbnormal);
        }

        [Fact]
        public void TagModel_ShouldSupportVIPPlayerScenario()
        {
            // Arrange - VIP player tag
            var tag = new TagModel
            {
                Id = 100,
                TagName = "VIP_GOLD",
                Description = "Gold tier VIP player with high lifetime value",
                Status = 1, // Active
                UpdateTime = DateTime.UtcNow.AddHours(-2),
                IsAbnormal = 0 // Normal
            };

            // Assert - VIP validations
            tag.TagName.Should().StartWith("VIP");
            tag.Status.Should().Be(1); // Active
            tag.IsAbnormal.Should().Be(0); // Normal
            tag.Description.Should().Contain("VIP");
            tag.UpdateTime.Should().NotBeNull();
            tag.UpdateTime.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void TagModel_ShouldSupportRiskPlayerScenario()
        {
            // Arrange - Risk/problem player tag
            var tag = new TagModel
            {
                Id = 200,
                TagName = "RISK_HIGH",
                Description = "High risk player - requires monitoring",
                Status = 1, // Active
                UpdateTime = DateTime.UtcNow.AddMinutes(-30),
                IsAbnormal = 1 // Abnormal/flagged
            };

            // Assert - Risk player validations
            tag.TagName.Should().StartWith("RISK");
            tag.IsAbnormal.Should().Be(1); // Flagged as abnormal
            tag.Status.Should().Be(1); // Still active for monitoring
            tag.Description.Should().Contain("risk");
            tag.Description.Should().Contain("monitoring");
        }

        [Fact]
        public void TagModel_ShouldSupportNewPlayerScenario()
        {
            // Arrange - New player tag
            var tag = new TagModel
            {
                Id = 300,
                TagName = "NEW_PLAYER",
                Description = "Recently registered player - eligible for welcome bonuses",
                Status = 1,
                UpdateTime = DateTime.UtcNow.AddDays(-1),
                IsAbnormal = 0
            };

            // Assert - New player validations
            tag.TagName.Should().Be("NEW_PLAYER");
            tag.Description.Should().Contain("welcome");
            tag.Status.Should().Be(1);
            tag.IsAbnormal.Should().Be(0);
            tag.UpdateTime.Should().BeAfter(DateTime.UtcNow.AddDays(-2));
        }

        [Fact]
        public void TagModel_ShouldSupportInactivePlayerScenario()
        {
            // Arrange - Inactive player tag
            var tag = new TagModel
            {
                Id = 400,
                TagName = "INACTIVE_90D",
                Description = "Player inactive for 90+ days - target for reactivation",
                Status = 2, // Inactive status
                UpdateTime = DateTime.UtcNow.AddDays(-90),
                IsAbnormal = 0
            };

            // Assert - Inactive player validations
            tag.TagName.Should().Contain("INACTIVE");
            tag.Status.Should().Be(2); // Inactive
            tag.UpdateTime.Should().BeBefore(DateTime.UtcNow.AddDays(-30));
            tag.Description.Should().Contain("reactivation");
        }

        [Fact]
        public void TagModel_ShouldSupportBonusAbuseScenario()
        {
            // Arrange - Bonus abuse detection tag
            var tag = new TagModel
            {
                Id = 500,
                TagName = "BONUS_ABUSE",
                Description = "Suspected bonus abuse - multiple accounts detected",
                Status = 1, // Active for monitoring
                UpdateTime = DateTime.UtcNow.AddHours(-6),
                IsAbnormal = 1 // Flagged as abnormal
            };

            // Assert - Bonus abuse validations
            tag.TagName.Should().Be("BONUS_ABUSE");
            tag.IsAbnormal.Should().Be(1); // Flagged
            tag.Status.Should().Be(1); // Active monitoring
            tag.Description.Should().Contain("abuse");
            tag.Description.Should().Contain("multiple accounts");
        }

        [Fact]
        public void TagModel_ShouldSupportArchivedTagScenario()
        {
            // Arrange - Archived/deleted tag
            var tag = new TagModel
            {
                Id = 600,
                TagName = "ARCHIVED_TAG",
                Description = "This tag is no longer in use",
                Status = 0, // Inactive/archived
                UpdateTime = DateTime.UtcNow.AddMonths(-6),
                IsAbnormal = 0
            };

            // Assert - Archived tag validations
            tag.Status.Should().Be(0); // Archived
            tag.TagName.Should().Contain("ARCHIVED");
            tag.UpdateTime.Should().BeBefore(DateTime.UtcNow.AddMonths(-3));
            tag.Description.Should().Contain("no longer in use");
        }

        [Fact]
        public void TagModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var tags = new[]
            {
                new TagModel { Id = 1, TagName = "VIP", Status = 1, IsAbnormal = 0 },
                new TagModel { Id = 2, TagName = "RISK", Status = 1, IsAbnormal = 1 },
                new TagModel { Id = 3, TagName = "INACTIVE", Status = 0, IsAbnormal = 0 }
            };

            // Act & Assert
            tags.Should().HaveCount(3);
            tags.Where(t => t.Status == 1).Should().HaveCount(2); // Active tags
            tags.Where(t => t.IsAbnormal == 1).Should().HaveCount(1); // Abnormal tags
            tags.Select(t => t.TagName).Should().Contain(new[] { "VIP", "RISK", "INACTIVE" });
            tags.Max(t => t.Id).Should().Be(3);
        }

        [Fact]
        public void TagModel_ShouldHandleUnicodeContent()
        {
            // Act
            var tag = new TagModel
            {
                TagName = "VIP_贵宾_VIP_会員",
                Description = "多语言VIP标签_Multilingual_多言語VIPタグ"
            };

            // Assert
            tag.TagName.Should().Contain("贵宾");
            tag.TagName.Should().Contain("VIP");
            tag.TagName.Should().Contain("会員");
            tag.Description.Should().Contain("Multilingual");
            tag.Description.Should().Contain("多语言");
            tag.Description.Should().Contain("多言語");
        }

        [Fact]
        public void TagModel_ShouldSupportUpdateTimeTracking()
        {
            // Arrange
            var initialTime = DateTime.UtcNow.AddDays(-1);
            var updateTime = DateTime.UtcNow;

            var tag = new TagModel
            {
                TagName = "TRACKED_TAG",
                UpdateTime = initialTime
            };

            // Act - Simulate update
            tag.UpdateTime = updateTime;

            // Assert - Update time tracking
            tag.UpdateTime.Should().Be(updateTime);
            tag.UpdateTime.Should().BeAfter(initialTime);
            tag.UpdateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void TagModel_ShouldSupportNullUpdateTime()
        {
            // Act
            var tag = new TagModel
            {
                TagName = "NO_UPDATE_TIME",
                UpdateTime = null
            };

            // Assert
            tag.UpdateTime.Should().BeNull();
            tag.Should().BeAssignableTo<IHasUpdateTime>();
        }

        [Fact]
        public void TagModel_ShouldSupportBusinessRuleValidations()
        {
            // Arrange - Business rule: VIP tags should not be abnormal
            var vipTag = new TagModel
            {
                TagName = "VIP_PLATINUM",
                Status = 1,
                IsAbnormal = 0 // VIP should be normal
            };

            // Arrange - Business rule: Risk tags should be abnormal
            var riskTag = new TagModel
            {
                TagName = "RISK_CRITICAL",
                Status = 1,
                IsAbnormal = 1 // Risk should be abnormal
            };

            // Assert - Business rule validations
            if (vipTag.TagName.StartsWith("VIP"))
            {
                vipTag.IsAbnormal.Should().Be(0); // VIP should be normal
            }

            if (riskTag.TagName.StartsWith("RISK"))
            {
                riskTag.IsAbnormal.Should().Be(1); // Risk should be abnormal
            }
        }

        [Theory]
        [InlineData("VIP_GOLD", 1, 0)]
        [InlineData("RISK_HIGH", 1, 1)]
        [InlineData("NEW_PLAYER", 1, 0)]
        [InlineData("INACTIVE_30D", 2, 0)]
        [InlineData("ARCHIVED_OLD", 0, 0)]
        public void TagModel_ShouldSupportCommonTagPatterns(string tagName, int expectedStatus, byte expectedAbnormal)
        {
            // Act
            var tag = new TagModel
            {
                TagName = tagName,
                Status = expectedStatus,
                IsAbnormal = expectedAbnormal
            };

            // Assert
            tag.TagName.Should().Be(tagName);
            tag.Status.Should().Be(expectedStatus);
            tag.IsAbnormal.Should().Be(expectedAbnormal);

            // Pattern-based validations
            if (tagName.StartsWith("VIP"))
            {
                tag.IsAbnormal.Should().Be(0); // VIP should be normal
            }
            else if (tagName.StartsWith("RISK"))
            {
                tag.IsAbnormal.Should().Be(1); // Risk should be abnormal
            }
        }
    }
}