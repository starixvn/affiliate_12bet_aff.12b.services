using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for AffiliateDeal DTO class
    /// Tests property validation, edge cases, and affiliate deal scenarios
    /// </summary>
    public class AffiliateDealTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var affiliateDeal = new AffiliateDeal();

            // Assert
            affiliateDeal.AffCode.Should().BeNull();
            affiliateDeal.DealId.Should().Be(0);
            affiliateDeal.DealName.Should().BeNull();
            affiliateDeal.TargetCountryCode.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();
            const string expectedAffCode = "AFF001";
            const int expectedDealId = 12345;
            const string expectedDealName = "Premium Deal 2024";
            const string expectedTargetCountryCode = "US";

            // Act
            affiliateDeal.AffCode = expectedAffCode;
            affiliateDeal.DealId = expectedDealId;
            affiliateDeal.DealName = expectedDealName;
            affiliateDeal.TargetCountryCode = expectedTargetCountryCode;

            // Assert
            affiliateDeal.AffCode.Should().Be(expectedAffCode);
            affiliateDeal.DealId.Should().Be(expectedDealId);
            affiliateDeal.DealName.Should().Be(expectedDealName);
            affiliateDeal.TargetCountryCode.Should().Be(expectedTargetCountryCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("AFF001")]
        [InlineData("AFFILIATE_CODE_123")]
        [InlineData("aff-code-with-dashes")]
        [InlineData("AFF_CODE_WITH_UNDERSCORES")]
        [InlineData("123456")]
        [InlineData("MixedCaseAffCode")]
        public void AffCode_ShouldHandleVariousStringFormats(string affCode)
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.AffCode = affCode;

            // Assert
            affiliateDeal.AffCode.Should().Be(affCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(12345)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void DealId_ShouldHandleVariousIntegerValues(int dealId)
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.DealId = dealId;

            // Assert
            affiliateDeal.DealId.Should().Be(dealId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Premium Deal")]
        [InlineData("Deal with Special Characters !@#$%")]
        [InlineData("Deal\nwith\nnewlines")]
        [InlineData("Deal\twith\ttabs")]
        [InlineData("VeryLongDealNameThatExceedsTypicalLengthLimitsButShouldStillBeHandledCorrectly")]
        public void DealName_ShouldHandleVariousStringFormats(string dealName)
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.DealName = dealName;

            // Assert
            affiliateDeal.DealName.Should().Be(dealName);
        }

        [Theory]
        [InlineData("US")]
        [InlineData("GB")]
        [InlineData("CA")]
        [InlineData("AU")]
        [InlineData("DE")]
        [InlineData("FR")]
        [InlineData("JP")]
        [InlineData("CN")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("USA")] // 3-letter code
        [InlineData("us")] // lowercase
        [InlineData("UNITED_STATES")] // full name
        public void TargetCountryCode_ShouldHandleVariousCountryFormats(string countryCode)
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.TargetCountryCode = countryCode;

            // Assert
            affiliateDeal.TargetCountryCode.Should().Be(countryCode);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.AffCode = null;
            affiliateDeal.DealName = null;
            affiliateDeal.TargetCountryCode = null;

            // Assert
            affiliateDeal.AffCode.Should().BeNull();
            affiliateDeal.DealName.Should().BeNull();
            affiliateDeal.TargetCountryCode.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();
            const string unicodeText = "联盟代码_José_García_🎯";

            // Act
            affiliateDeal.AffCode = unicodeText;
            affiliateDeal.DealName = unicodeText;
            affiliateDeal.TargetCountryCode = unicodeText;

            // Assert
            affiliateDeal.AffCode.Should().Be(unicodeText);
            affiliateDeal.DealName.Should().Be(unicodeText);
            affiliateDeal.TargetCountryCode.Should().Be(unicodeText);
        }

        [Fact]
        public void AffiliateDeal_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var affiliateDeal = new AffiliateDeal
            {
                AffCode = "PREMIUM_AFF_001",
                DealId = 98765,
                DealName = "Premium Casino Deal Q4 2024",
                TargetCountryCode = "US"
            };

            // Assert
            affiliateDeal.AffCode.Should().Be("PREMIUM_AFF_001");
            affiliateDeal.DealId.Should().Be(98765);
            affiliateDeal.DealName.Should().Be("Premium Casino Deal Q4 2024");
            affiliateDeal.TargetCountryCode.Should().Be("US");
        }

        [Fact]
        public void AffiliateDeal_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var affiliateDeal = new AffiliateDeal
            {
                AffCode = "AFF001",
                DealId = 123
                // DealName and TargetCountryCode left as default/null
            };

            // Assert
            affiliateDeal.AffCode.Should().Be("AFF001");
            affiliateDeal.DealId.Should().Be(123);
            affiliateDeal.DealName.Should().BeNull();
            affiliateDeal.TargetCountryCode.Should().BeNull();
        }

        [Theory]
        [InlineData("AFF001", 1, "Basic Deal", "US")]
        [InlineData("AFF002", 2, "Premium Deal", "GB")]
        [InlineData("AFF003", 3, "VIP Deal", "CA")]
        [InlineData("AFF004", 4, "Standard Deal", "AU")]
        public void AffiliateDeal_ShouldHandleMultipleDealScenarios(string affCode, int dealId, string dealName, string countryCode)
        {
            // Arrange & Act
            var affiliateDeal = new AffiliateDeal
            {
                AffCode = affCode,
                DealId = dealId,
                DealName = dealName,
                TargetCountryCode = countryCode
            };

            // Assert
            affiliateDeal.AffCode.Should().Be(affCode);
            affiliateDeal.DealId.Should().Be(dealId);
            affiliateDeal.DealName.Should().Be(dealName);
            affiliateDeal.TargetCountryCode.Should().Be(countryCode);
        }

        [Fact]
        public void DealName_ShouldHandleLargeString()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();
            var largeDealName = new string('D', 5000); // 5KB string

            // Act
            affiliateDeal.DealName = largeDealName;

            // Assert
            affiliateDeal.DealName.Should().Be(largeDealName);
            affiliateDeal.DealName.Length.Should().Be(5000);
        }

        [Fact]
        public void AffCode_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();
            const string specialCharsAffCode = "AFF!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            affiliateDeal.AffCode = specialCharsAffCode;

            // Assert
            affiliateDeal.AffCode.Should().Be(specialCharsAffCode);
        }

        [Fact]
        public void TargetCountryCode_ShouldPreserveCase()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act & Assert - Uppercase
            affiliateDeal.TargetCountryCode = "US";
            affiliateDeal.TargetCountryCode.Should().Be("US");

            // Act & Assert - Lowercase
            affiliateDeal.TargetCountryCode = "us";
            affiliateDeal.TargetCountryCode.Should().Be("us");

            // Act & Assert - Mixed case
            affiliateDeal.TargetCountryCode = "Us";
            affiliateDeal.TargetCountryCode.Should().Be("Us");
        }

        [Fact]
        public void AffiliateDeal_ShouldHandleInternationalDealsScenario()
        {
            // Arrange & Act
            var affiliateDeal = new AffiliateDeal
            {
                AffCode = "INTL_AFF_EU",
                DealId = 999,
                DealName = "International European Deal - José García Partnership",
                TargetCountryCode = "ES"
            };

            // Assert
            affiliateDeal.AffCode.Should().Be("INTL_AFF_EU");
            affiliateDeal.DealId.Should().Be(999);
            affiliateDeal.DealName.Should().Contain("José García");
            affiliateDeal.TargetCountryCode.Should().Be("ES");
        }

        [Fact]
        public void DealId_ShouldSupportNegativeValues()
        {
            // Arrange
            var affiliateDeal = new AffiliateDeal();

            // Act
            affiliateDeal.DealId = -12345;

            // Assert
            affiliateDeal.DealId.Should().Be(-12345);
        }

        [Fact]
        public void AffiliateDeal_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var affiliateDeal = new AffiliateDeal
            {
                DealId = 1
                // All other properties left as default/null
            };

            // Assert
            affiliateDeal.DealId.Should().Be(1);
            affiliateDeal.AffCode.Should().BeNull();
            affiliateDeal.DealName.Should().BeNull();
            affiliateDeal.TargetCountryCode.Should().BeNull();
        }
    }
}
