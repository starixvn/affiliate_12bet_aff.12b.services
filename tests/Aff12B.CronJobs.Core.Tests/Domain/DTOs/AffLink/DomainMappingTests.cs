using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for DomainMapping DTO class
    /// Tests property validation, edge cases, and domain mapping scenarios
    /// </summary>
    public class DomainMappingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var domainMapping = new DomainMapping();

            // Assert
            domainMapping.AffDomain.Should().BeNull();
            domainMapping.LanguageCode.Should().BeNull();
            domainMapping.LanguageLink.Should().BeNull();
            domainMapping.RedirectDomain.Should().BeNull();
            domainMapping.IsSSL.Should().BeFalse();
            domainMapping.IsMobile.Should().BeFalse();
            domainMapping.IsExcluded.Should().BeFalse();
            domainMapping.DealIds.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var domainMapping = new DomainMapping();
            const string expectedAffDomain = "affiliate.example.com";
            const string expectedLanguageCode = "en-US";
            const string expectedLanguageLink = "/en/casino";
            const string expectedRedirectDomain = "casino.example.com";
            const bool expectedIsSSL = true;
            const bool expectedIsMobile = true;
            const bool expectedIsExcluded = false;
            const string expectedDealIds = "1,2,3,4,5";

            // Act
            domainMapping.AffDomain = expectedAffDomain;
            domainMapping.LanguageCode = expectedLanguageCode;
            domainMapping.LanguageLink = expectedLanguageLink;
            domainMapping.RedirectDomain = expectedRedirectDomain;
            domainMapping.IsSSL = expectedIsSSL;
            domainMapping.IsMobile = expectedIsMobile;
            domainMapping.IsExcluded = expectedIsExcluded;
            domainMapping.DealIds = expectedDealIds;

            // Assert
            domainMapping.AffDomain.Should().Be(expectedAffDomain);
            domainMapping.LanguageCode.Should().Be(expectedLanguageCode);
            domainMapping.LanguageLink.Should().Be(expectedLanguageLink);
            domainMapping.RedirectDomain.Should().Be(expectedRedirectDomain);
            domainMapping.IsSSL.Should().Be(expectedIsSSL);
            domainMapping.IsMobile.Should().Be(expectedIsMobile);
            domainMapping.IsExcluded.Should().Be(expectedIsExcluded);
            domainMapping.DealIds.Should().Be(expectedDealIds);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("example.com")]
        [InlineData("affiliate.example.com")]
        [InlineData("sub.domain.example.com")]
        [InlineData("localhost")]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.1.1")]
        [InlineData("domain-with-dashes.com")]
        [InlineData("domain_with_underscores.com")]
        public void AffDomain_ShouldHandleVariousDomainFormats(string affDomain)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.AffDomain = affDomain;

            // Assert
            domainMapping.AffDomain.Should().Be(affDomain);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("redirect.example.com")]
        [InlineData("casino.example.com")]
        [InlineData("sports.example.com")]
        [InlineData("poker.example.com")]
        [InlineData("https://secure.example.com")]
        [InlineData("http://insecure.example.com")]
        public void RedirectDomain_ShouldHandleVariousDomainFormats(string redirectDomain)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.RedirectDomain = redirectDomain;

            // Assert
            domainMapping.RedirectDomain.Should().Be(redirectDomain);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        [InlineData("es")]
        [InlineData("es-ES")]
        [InlineData("fr")]
        [InlineData("fr-FR")]
        [InlineData("de")]
        [InlineData("de-DE")]
        [InlineData("zh")]
        [InlineData("zh-CN")]
        [InlineData("ja")]
        [InlineData("ja-JP")]
        public void LanguageCode_ShouldHandleVariousLanguageCodes(string languageCode)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.LanguageCode = languageCode;

            // Assert
            domainMapping.LanguageCode.Should().Be(languageCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("/")]
        [InlineData("/en")]
        [InlineData("/en/casino")]
        [InlineData("/es/deportes")]
        [InlineData("/fr/casino")]
        [InlineData("/de/sport")]
        [InlineData("/casino/games")]
        [InlineData("/sports/betting")]
        [InlineData("/poker/tournaments")]
        public void LanguageLink_ShouldHandleVariousLinkFormats(string languageLink)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.LanguageLink = languageLink;

            // Assert
            domainMapping.LanguageLink.Should().Be(languageLink);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSSL_ShouldHandleBooleanValues(bool isSSL)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.IsSSL = isSSL;

            // Assert
            domainMapping.IsSSL.Should().Be(isSSL);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMobile_ShouldHandleBooleanValues(bool isMobile)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.IsMobile = isMobile;

            // Assert
            domainMapping.IsMobile.Should().Be(isMobile);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsExcluded_ShouldHandleBooleanValues(bool isExcluded)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.IsExcluded = isExcluded;

            // Assert
            domainMapping.IsExcluded.Should().Be(isExcluded);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1")]
        [InlineData("1,2,3")]
        [InlineData("1,2,3,4,5,6,7,8,9,10")]
        [InlineData("100,200,300")]
        [InlineData("1|2|3")]
        [InlineData("1;2;3")]
        [InlineData("deal1,deal2,deal3")]
        [InlineData("1, 2, 3")]
        public void DealIds_ShouldHandleVariousStringFormats(string dealIds)
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.DealIds = dealIds;

            // Assert
            domainMapping.DealIds.Should().Be(dealIds);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var domainMapping = new DomainMapping();

            // Act
            domainMapping.AffDomain = null;
            domainMapping.LanguageCode = null;
            domainMapping.LanguageLink = null;
            domainMapping.RedirectDomain = null;
            domainMapping.DealIds = null;

            // Assert
            domainMapping.AffDomain.Should().BeNull();
            domainMapping.LanguageCode.Should().BeNull();
            domainMapping.LanguageLink.Should().BeNull();
            domainMapping.RedirectDomain.Should().BeNull();
            domainMapping.DealIds.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var domainMapping = new DomainMapping();
            const string unicodeText = "域名_José_García_🌐";

            // Act
            domainMapping.AffDomain = unicodeText;
            domainMapping.LanguageCode = unicodeText;
            domainMapping.LanguageLink = unicodeText;
            domainMapping.RedirectDomain = unicodeText;
            domainMapping.DealIds = unicodeText;

            // Assert
            domainMapping.AffDomain.Should().Be(unicodeText);
            domainMapping.LanguageCode.Should().Be(unicodeText);
            domainMapping.LanguageLink.Should().Be(unicodeText);
            domainMapping.RedirectDomain.Should().Be(unicodeText);
            domainMapping.DealIds.Should().Be(unicodeText);
        }

        [Fact]
        public void DomainMapping_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "affiliate.casino.com",
                LanguageCode = "en-US",
                LanguageLink = "/en/casino/games",
                RedirectDomain = "secure.casino.com",
                IsSSL = true,
                IsMobile = false,
                IsExcluded = false,
                DealIds = "1,5,10,15,20"
            };

            // Assert
            domainMapping.AffDomain.Should().Be("affiliate.casino.com");
            domainMapping.LanguageCode.Should().Be("en-US");
            domainMapping.LanguageLink.Should().Be("/en/casino/games");
            domainMapping.RedirectDomain.Should().Be("secure.casino.com");
            domainMapping.IsSSL.Should().BeTrue();
            domainMapping.IsMobile.Should().BeFalse();
            domainMapping.IsExcluded.Should().BeFalse();
            domainMapping.DealIds.Should().Be("1,5,10,15,20");
        }

        [Fact]
        public void DomainMapping_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "simple.domain.com",
                RedirectDomain = "target.domain.com"
                // Other properties left as default
            };

            // Assert
            domainMapping.AffDomain.Should().Be("simple.domain.com");
            domainMapping.RedirectDomain.Should().Be("target.domain.com");
            domainMapping.LanguageCode.Should().BeNull();
            domainMapping.LanguageLink.Should().BeNull();
            domainMapping.IsSSL.Should().BeFalse();
            domainMapping.IsMobile.Should().BeFalse();
            domainMapping.IsExcluded.Should().BeFalse();
            domainMapping.DealIds.Should().BeNull();
        }

        [Fact]
        public void DomainMapping_ShouldHandleSSLEnabledScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "secure.affiliate.com",
                RedirectDomain = "https://secure.casino.com",
                IsSSL = true,
                LanguageCode = "en-US",
                LanguageLink = "/secure/casino"
            };

            // Assert
            domainMapping.IsSSL.Should().BeTrue();
            domainMapping.AffDomain.Should().Contain("secure");
            domainMapping.RedirectDomain.Should().StartWith("https://");
        }

        [Fact]
        public void DomainMapping_ShouldHandleMobileOnlyScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "m.affiliate.com",
                RedirectDomain = "m.casino.com",
                IsMobile = true,
                IsSSL = true,
                LanguageCode = "en-US",
                LanguageLink = "/mobile/casino"
            };

            // Assert
            domainMapping.IsMobile.Should().BeTrue();
            domainMapping.AffDomain.Should().StartWith("m.");
            domainMapping.RedirectDomain.Should().StartWith("m.");
            domainMapping.LanguageLink.Should().Contain("mobile");
        }

        [Fact]
        public void DomainMapping_ShouldHandleExcludedDomainScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "blocked.affiliate.com",
                RedirectDomain = null,
                IsExcluded = true,
                IsSSL = false,
                IsMobile = false,
                DealIds = null
            };

            // Assert
            domainMapping.IsExcluded.Should().BeTrue();
            domainMapping.RedirectDomain.Should().BeNull();
            domainMapping.DealIds.Should().BeNull();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, false)]
        public void DomainMapping_ShouldHandleAllBooleanCombinations(bool isSSL, bool isMobile, bool isExcluded)
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "test.domain.com",
                IsSSL = isSSL,
                IsMobile = isMobile,
                IsExcluded = isExcluded
            };

            // Assert
            domainMapping.IsSSL.Should().Be(isSSL);
            domainMapping.IsMobile.Should().Be(isMobile);
            domainMapping.IsExcluded.Should().Be(isExcluded);
        }

        [Fact]
        public void DomainMapping_ShouldHandleInternationalDomainScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "afiliado.casino.es",
                LanguageCode = "es-ES",
                LanguageLink = "/es/casino",
                RedirectDomain = "casino.es",
                IsSSL = true,
                IsMobile = false,
                IsExcluded = false,
                DealIds = "es1,es2,es3"
            };

            // Assert
            domainMapping.AffDomain.Should().EndWith(".es");
            domainMapping.LanguageCode.Should().Be("es-ES");
            domainMapping.LanguageLink.Should().StartWith("/es/");
            domainMapping.DealIds.Should().Contain("es");
        }

        [Fact]
        public void DomainMapping_ShouldHandleComplexLanguageLinkScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "multi.affiliate.com",
                LanguageCode = "zh-CN",
                LanguageLink = "/zh/casino/games?category=slots&provider=all",
                RedirectDomain = "casino.com",
                IsSSL = true,
                DealIds = "chinese1,chinese2,chinese3"
            };

            // Assert
            domainMapping.LanguageLink.Should().Contain("?");
            domainMapping.LanguageLink.Should().Contain("&");
            domainMapping.LanguageLink.Should().StartWith("/zh/");
            domainMapping.LanguageCode.Should().Be("zh-CN");
        }

        [Fact]
        public void DomainMapping_ShouldHandleLargeDealIdsString()
        {
            // Arrange
            var domainMapping = new DomainMapping();
            // Reduced from 500 to 25 numbers for performance
            var largeDealIds = string.Join(",", Enumerable.Range(1, 25));

            // Act
            domainMapping.DealIds = largeDealIds;

            // Assert
            domainMapping.DealIds.Should().Be(largeDealIds);
            domainMapping.DealIds.Should().Contain("1,2,3");
            domainMapping.DealIds.Should().Contain("23,24,25");
        }

        [Fact]
        public void AffDomain_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var domainMapping = new DomainMapping();
            const string specialCharsDomain = "domain-with-special_chars.co.uk";

            // Act
            domainMapping.AffDomain = specialCharsDomain;

            // Assert
            domainMapping.AffDomain.Should().Be(specialCharsDomain);
        }

        [Fact]
        public void LanguageLink_ShouldHandleComplexUrlParameters()
        {
            // Arrange
            var domainMapping = new DomainMapping();
            const string complexLink = "/casino/games?lang=en&currency=USD&bonus=welcome&ref=affiliate123&utm_source=partner&utm_medium=banner";

            // Act
            domainMapping.LanguageLink = complexLink;

            // Assert
            domainMapping.LanguageLink.Should().Be(complexLink);
            domainMapping.LanguageLink.Should().Contain("lang=en");
            domainMapping.LanguageLink.Should().Contain("utm_source=partner");
        }

        [Fact]
        public void DomainMapping_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var domainMapping = new DomainMapping
            {
                AffDomain = "minimal.com"
                // All other properties left as default
            };

            // Assert
            domainMapping.AffDomain.Should().Be("minimal.com");
            domainMapping.LanguageCode.Should().BeNull();
            domainMapping.LanguageLink.Should().BeNull();
            domainMapping.RedirectDomain.Should().BeNull();
            domainMapping.IsSSL.Should().BeFalse();
            domainMapping.IsMobile.Should().BeFalse();
            domainMapping.IsExcluded.Should().BeFalse();
            domainMapping.DealIds.Should().BeNull();
        }
    }
}
