using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.SpParams;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.SpParams
{
    /// <summary>
    /// Unit tests for SiteNameInfo class.
    /// Business-focused scenarios for site brand name configuration settings.
    /// Tests fixed setting name behavior and inheritance.
    /// </summary>
    public class SiteNameInfoTests
    {
        [Fact]
        public void SiteNameInfo_ShouldInitializeWithPrefixProvider()
        {
            // Arrange & Act
            var siteNameInfo = new SiteNameInfo("SITE_PREFIX");

            // Assert - Should initialize with prefix provider
            siteNameInfo.Should().NotBeNull();
            siteNameInfo.SName.Should().Be("BrandName");
            siteNameInfo.Should().BeAssignableTo<SettingInfo>();
            siteNameInfo.Should().BeAssignableTo<ISettingInfo>();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_BrandConfiguration()
        {
            // Arrange - Brand configuration scenario
            var siteNameInfo = new SiteNameInfo("BRAND_CONFIG");

            // Act & Assert - Should support brand name configuration
            siteNameInfo.SName.Should().Be("BrandName");
            siteNameInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_MultiSiteBranding()
        {
            // Arrange - Multi-site branding scenario
            var site1Info = new SiteNameInfo("SITE1_BRAND");
            var site2Info = new SiteNameInfo("SITE2_BRAND");
            var site3Info = new SiteNameInfo("SITE3_BRAND");

            // Act & Assert - Should support multiple site brand configurations
            var siteInfos = new[] { site1Info, site2Info, site3Info };
            
            siteInfos.Should().AllSatisfy(siteInfo =>
            {
                siteInfo.SName.Should().Be("BrandName");
                siteInfo.IsValid().Should().BeTrue();
            });
        }

        [Fact]
        public void SName_ShouldAlwaysReturnBrandName()
        {
            // Arrange - Test fixed setting name behavior
            var siteNameInfo1 = new SiteNameInfo("PREFIX1");
            var siteNameInfo2 = new SiteNameInfo("PREFIX2");
            var siteNameInfo3 = new SiteNameInfo("DIFFERENT_PREFIX");

            // Act & Assert - Should always return "BrandName" regardless of prefix
            siteNameInfo1.SName.Should().Be("BrandName");
            siteNameInfo2.SName.Should().Be("BrandName");
            siteNameInfo3.SName.Should().Be("BrandName");
        }

        [Theory]
        [InlineData("")]
        [InlineData("SIMPLE")]
        [InlineData("COMPLEX_PREFIX_WITH_NUMBERS_123")]
        [InlineData("SPECIAL_CHARS_!@#")]
        public void SiteNameInfo_ShouldAcceptVariousPrefixProviders(string prefixProvider)
        {
            // Arrange & Act
            var siteNameInfo = new SiteNameInfo(prefixProvider);

            // Assert - Should accept various prefix providers but always return BrandName
            siteNameInfo.SName.Should().Be("BrandName");
            siteNameInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_CasinoSiteBranding()
        {
            // Arrange - Casino site branding scenario
            var casinoSiteInfo = new SiteNameInfo("CASINO_SITE");

            // Act & Assert - Should support casino site brand configuration
            casinoSiteInfo.SName.Should().Be("BrandName");
            casinoSiteInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_SportsBettingSiteBranding()
        {
            // Arrange - Sports betting site branding scenario
            var sportsSiteInfo = new SiteNameInfo("SPORTS_BETTING");

            // Act & Assert - Should support sports betting site brand configuration
            sportsSiteInfo.SName.Should().Be("BrandName");
            sportsSiteInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldInheritFromSettingInfo()
        {
            // Arrange - Type checking for inheritance
            var siteNameInfo = new SiteNameInfo("TEST_PREFIX");

            // Act & Assert - Should inherit from SettingInfo
            siteNameInfo.Should().BeAssignableTo<SettingInfo>();
            siteNameInfo.Should().BeAssignableTo<ISettingInfo>();
        }

        [Fact]
        public void SiteNameInfo_ShouldOverrideBaseSettingName()
        {
            // Arrange - Test property override behavior
            var siteNameInfo = new SiteNameInfo("TEST_PREFIX");

            // Act & Assert - Should override base SName property with fixed value
            siteNameInfo.SName.Should().Be("BrandName");
            siteNameInfo.SName.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_WhiteLabelConfiguration()
        {
            // Arrange - White label configuration scenario
            var whiteLabelInfo = new SiteNameInfo("WHITE_LABEL");

            // Act & Assert - Should support white label brand configuration
            whiteLabelInfo.SName.Should().Be("BrandName");
            whiteLabelInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldSupportBusinessScenario_PartnerSiteBranding()
        {
            // Arrange - Partner site branding scenario
            var partnerSiteInfo = new SiteNameInfo("PARTNER_SITE");

            // Act & Assert - Should support partner site brand configuration
            partnerSiteInfo.SName.Should().Be("BrandName");
            partnerSiteInfo.IsValid().Should().BeTrue();
        }

        [Fact]
        public void SiteNameInfo_ShouldMaintainConsistentBehavior()
        {
            // Arrange - Consistency test across multiple instances
            var instances = new[]
            {
                new SiteNameInfo("INSTANCE1"),
                new SiteNameInfo("INSTANCE2"),
                new SiteNameInfo("INSTANCE3")
            };

            // Act & Assert - Should maintain consistent behavior across instances
            instances.Should().AllSatisfy(instance =>
            {
                instance.SName.Should().Be("BrandName");
                instance.IsValid().Should().BeTrue();
                instance.Should().BeAssignableTo<ISettingInfo>();
            });
        }
    }
}
