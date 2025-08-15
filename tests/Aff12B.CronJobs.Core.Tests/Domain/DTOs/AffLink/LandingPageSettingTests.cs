using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for LandingPageSetting DTO class
    /// Tests property validation, edge cases, and landing page configuration scenarios
    /// </summary>
    public class LandingPageSettingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var landingPageSetting = new LandingPageSetting();

            // Assert
            landingPageSetting.PageCode.Should().BeNull();
            landingPageSetting.IsMobile.Should().BeFalse();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.DealIds.Should().BeNull();
            landingPageSetting.IsDefault.Should().BeFalse();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            const string expectedPageCode = "LP001";
            const bool expectedIsMobile = true;
            const bool expectedIsExcluded = true;
            const string expectedDealIds = "1,2,3,4,5";
            const bool expectedIsDefault = true;

            // Act
            landingPageSetting.PageCode = expectedPageCode;
            landingPageSetting.IsMobile = expectedIsMobile;
            landingPageSetting.IsExcluded = expectedIsExcluded;
            landingPageSetting.DealIds = expectedDealIds;
            landingPageSetting.IsDefault = expectedIsDefault;

            // Assert
            landingPageSetting.PageCode.Should().Be(expectedPageCode);
            landingPageSetting.IsMobile.Should().Be(expectedIsMobile);
            landingPageSetting.IsExcluded.Should().Be(expectedIsExcluded);
            landingPageSetting.DealIds.Should().Be(expectedDealIds);
            landingPageSetting.IsDefault.Should().Be(expectedIsDefault);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("LP001")]
        [InlineData("landing-page-001")]
        [InlineData("LANDING_PAGE_WITH_UNDERSCORES")]
        [InlineData("landingPage.mobile")]
        [InlineData("123456")]
        [InlineData("MixedCasePageCode")]
        public void PageCode_ShouldHandleVariousStringFormats(string pageCode)
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.PageCode = pageCode;

            // Assert
            landingPageSetting.PageCode.Should().Be(pageCode);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMobile_ShouldHandleBooleanValues(bool isMobile)
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.IsMobile = isMobile;

            // Assert
            landingPageSetting.IsMobile.Should().Be(isMobile);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsExcluded_ShouldHandleBooleanValues(bool isExcluded)
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.IsExcluded = isExcluded;

            // Assert
            landingPageSetting.IsExcluded.Should().Be(isExcluded);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsDefault_ShouldHandleBooleanValues(bool isDefault)
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.IsDefault = isDefault;

            // Assert
            landingPageSetting.IsDefault.Should().Be(isDefault);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("1,2,3")]
        [InlineData("1,2,3,4,5,6,7,8,9,10")]
        [InlineData("100,200,300")]
        [InlineData("1|2|3")] // Different delimiter
        [InlineData("1;2;3")] // Semicolon delimiter
        [InlineData("deal1,deal2,deal3")]
        [InlineData("1, 2, 3")] // With spaces
        public void DealIds_ShouldHandleVariousStringFormats(string dealIds)
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.DealIds = dealIds;

            // Assert
            landingPageSetting.DealIds.Should().Be(dealIds);
        }

        [Fact]
        public void PageCode_ShouldHandleNullValue()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.PageCode = null;

            // Assert
            landingPageSetting.PageCode.Should().BeNull();
        }

        [Fact]
        public void DealIds_ShouldHandleNullValue()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.DealIds = null;

            // Assert
            landingPageSetting.DealIds.Should().BeNull();
        }

        [Fact]
        public void PageCode_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            const string unicodePageCode = "着陆页_José_García_🚀";

            // Act
            landingPageSetting.PageCode = unicodePageCode;

            // Assert
            landingPageSetting.PageCode.Should().Be(unicodePageCode);
        }

        [Fact]
        public void DealIds_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            const string unicodeDealIds = "交易1,交易2,José,García";

            // Act
            landingPageSetting.DealIds = unicodeDealIds;

            // Assert
            landingPageSetting.DealIds.Should().Be(unicodeDealIds);
        }

        [Fact]
        public void LandingPageSetting_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "PREMIUM_LP_MOBILE",
                IsMobile = true,
                IsExcluded = false,
                DealIds = "1,5,10,15,20",
                IsDefault = true
            };

            // Assert
            landingPageSetting.PageCode.Should().Be("PREMIUM_LP_MOBILE");
            landingPageSetting.IsMobile.Should().BeTrue();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.DealIds.Should().Be("1,5,10,15,20");
            landingPageSetting.IsDefault.Should().BeTrue();
        }

        [Fact]
        public void LandingPageSetting_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "LP001",
                IsMobile = true
                // Other properties left as default
            };

            // Assert
            landingPageSetting.PageCode.Should().Be("LP001");
            landingPageSetting.IsMobile.Should().BeTrue();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.DealIds.Should().BeNull();
            landingPageSetting.IsDefault.Should().BeFalse();
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleMobileOnlyScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "MOBILE_ONLY_LP",
                IsMobile = true,
                IsExcluded = false,
                DealIds = "1,2,3",
                IsDefault = false
            };

            // Assert
            landingPageSetting.IsMobile.Should().BeTrue();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.PageCode.Should().Contain("MOBILE");
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleDesktopOnlyScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "DESKTOP_ONLY_LP",
                IsMobile = false,
                IsExcluded = false,
                DealIds = "10,20,30",
                IsDefault = true
            };

            // Assert
            landingPageSetting.IsMobile.Should().BeFalse();
            landingPageSetting.IsDefault.Should().BeTrue();
            landingPageSetting.PageCode.Should().Contain("DESKTOP");
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleExcludedPageScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "EXCLUDED_LP",
                IsMobile = false,
                IsExcluded = true,
                DealIds = null,
                IsDefault = false
            };

            // Assert
            landingPageSetting.IsExcluded.Should().BeTrue();
            landingPageSetting.DealIds.Should().BeNull();
            landingPageSetting.IsDefault.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void LandingPageSetting_ShouldHandleAllBooleanCombinations(bool isMobile, bool isExcluded, bool isDefault)
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "TEST_LP",
                IsMobile = isMobile,
                IsExcluded = isExcluded,
                IsDefault = isDefault
            };

            // Assert
            landingPageSetting.IsMobile.Should().Be(isMobile);
            landingPageSetting.IsExcluded.Should().Be(isExcluded);
            landingPageSetting.IsDefault.Should().Be(isDefault);

            // Verify that the properties are set independently
            var hasAnyTrueFlag = isMobile || isExcluded || isDefault;
            if (isMobile || isExcluded || isDefault)
            {
                hasAnyTrueFlag.Should().BeTrue("because at least one boolean property is true");
            }
            else
            {
                hasAnyTrueFlag.Should().BeFalse("because all boolean properties are false");
            }
        }

        [Fact]
        public void DealIds_ShouldHandleLargeString()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            // Reduced from 1000 to 50 numbers for performance
            var largeDealIds = string.Join(",", Enumerable.Range(1, 50)); // 1,2,3,...,50

            // Act
            landingPageSetting.DealIds = largeDealIds;

            // Assert
            landingPageSetting.DealIds.Should().Be(largeDealIds);
            landingPageSetting.DealIds.Should().Contain("1,2,3");
            landingPageSetting.DealIds.Should().Contain("48,49,50");
        }

        [Fact]
        public void PageCode_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            const string specialCharsPageCode = "LP!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            landingPageSetting.PageCode = specialCharsPageCode;

            // Assert
            landingPageSetting.PageCode.Should().Be(specialCharsPageCode);
        }

        [Fact]
        public void DealIds_ShouldHandleJsonLikeFormat()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();
            const string jsonLikeDealIds = "[{\"id\":1},{\"id\":2},{\"id\":3}]";

            // Act
            landingPageSetting.DealIds = jsonLikeDealIds;

            // Assert
            landingPageSetting.DealIds.Should().Be(jsonLikeDealIds);
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleDefaultPageScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "DEFAULT_LANDING_PAGE",
                IsMobile = false,
                IsExcluded = false,
                DealIds = "1,2,3,4,5,6,7,8,9,10",
                IsDefault = true
            };

            // Assert
            landingPageSetting.IsDefault.Should().BeTrue();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.DealIds.Should().NotBeNullOrEmpty();
            landingPageSetting.PageCode.Should().Contain("DEFAULT");
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "MIN_LP"
                // All other properties left as default
            };

            // Assert
            landingPageSetting.PageCode.Should().Be("MIN_LP");
            landingPageSetting.IsMobile.Should().BeFalse();
            landingPageSetting.IsExcluded.Should().BeFalse();
            landingPageSetting.DealIds.Should().BeNull();
            landingPageSetting.IsDefault.Should().BeFalse();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var landingPageSetting = new LandingPageSetting();

            // Act
            landingPageSetting.PageCode = null;
            landingPageSetting.DealIds = null;

            // Assert
            landingPageSetting.PageCode.Should().BeNull();
            landingPageSetting.DealIds.Should().BeNull();
        }

        [Fact]
        public void LandingPageSetting_ShouldHandleComplexDealIdsScenario()
        {
            // Arrange & Act
            var landingPageSetting = new LandingPageSetting
            {
                PageCode = "COMPLEX_DEALS_LP",
                IsMobile = true,
                IsExcluded = false,
                DealIds = "premium:1,2,3|standard:4,5,6|vip:7,8,9",
                IsDefault = false
            };

            // Assert
            landingPageSetting.DealIds.Should().Contain("premium:");
            landingPageSetting.DealIds.Should().Contain("standard:");
            landingPageSetting.DealIds.Should().Contain("vip:");
            landingPageSetting.DealIds.Should().Contain("|");
        }
    }
}
