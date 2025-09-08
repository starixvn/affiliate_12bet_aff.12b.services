using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for SpecialDomainMapping DTO class
    /// Tests property validation, edge cases, and special domain mapping scenarios
    /// </summary>
    public class SpecialDomainMappingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var specialDomainMapping = new SpecialDomainMapping();

            // Assert
            specialDomainMapping.AffCode.Should().BeNull();
            specialDomainMapping.AffDomain.Should().BeNull();
            specialDomainMapping.RedirectDomain.Should().BeNull();
            specialDomainMapping.IsSSL.Should().BeFalse();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();
            const string expectedAffCode = "SPECIAL_AFF_001";
            const string expectedAffDomain = "special.affiliate.com";
            const string expectedRedirectDomain = "secure.casino.com";
            const bool expectedIsSSL = true;

            // Act
            specialDomainMapping.AffCode = expectedAffCode;
            specialDomainMapping.AffDomain = expectedAffDomain;
            specialDomainMapping.RedirectDomain = expectedRedirectDomain;
            specialDomainMapping.IsSSL = expectedIsSSL;

            // Assert
            specialDomainMapping.AffCode.Should().Be(expectedAffCode);
            specialDomainMapping.AffDomain.Should().Be(expectedAffDomain);
            specialDomainMapping.RedirectDomain.Should().Be(expectedRedirectDomain);
            specialDomainMapping.IsSSL.Should().Be(expectedIsSSL);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("AFF001")]
        [InlineData("SPECIAL_AFF_001")]
        [InlineData("special-aff-001")]
        [InlineData("SpecialAff001")]
        [InlineData("123456")]
        [InlineData("VIP_AFFILIATE")]
        [InlineData("PREMIUM_PARTNER")]
        public void AffCode_ShouldHandleVariousStringFormats(string affCode)
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.AffCode = affCode;

            // Assert
            specialDomainMapping.AffCode.Should().Be(affCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("example.com")]
        [InlineData("special.affiliate.com")]
        [InlineData("vip.partner.domain.com")]
        [InlineData("localhost")]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.1.100")]
        [InlineData("domain-with-dashes.com")]
        [InlineData("domain_with_underscores.com")]
        [InlineData("sub.domain.example.co.uk")]
        public void AffDomain_ShouldHandleVariousDomainFormats(string affDomain)
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.AffDomain = affDomain;

            // Assert
            specialDomainMapping.AffDomain.Should().Be(affDomain);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("redirect.example.com")]
        [InlineData("secure.casino.com")]
        [InlineData("vip.casino.domain.com")]
        [InlineData("https://secure.casino.com")]
        [InlineData("http://casino.com")]
        [InlineData("casino.example.co.uk")]
        [InlineData("sports.betting.domain.com")]
        [InlineData("poker.tournament.site.com")]
        public void RedirectDomain_ShouldHandleVariousDomainFormats(string redirectDomain)
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.RedirectDomain = redirectDomain;

            // Assert
            specialDomainMapping.RedirectDomain.Should().Be(redirectDomain);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSSL_ShouldHandleBooleanValues(bool isSSL)
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.IsSSL = isSSL;

            // Assert
            specialDomainMapping.IsSSL.Should().Be(isSSL);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.AffCode = null;
            specialDomainMapping.AffDomain = null;
            specialDomainMapping.RedirectDomain = null;

            // Assert
            specialDomainMapping.AffCode.Should().BeNull();
            specialDomainMapping.AffDomain.Should().BeNull();
            specialDomainMapping.RedirectDomain.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();
            const string unicodeText = "特殊域名_José_García_🌐";

            // Act
            specialDomainMapping.AffCode = unicodeText;
            specialDomainMapping.AffDomain = unicodeText;
            specialDomainMapping.RedirectDomain = unicodeText;

            // Assert
            specialDomainMapping.AffCode.Should().Be(unicodeText);
            specialDomainMapping.AffDomain.Should().Be(unicodeText);
            specialDomainMapping.RedirectDomain.Should().Be(unicodeText);
        }

        [Fact]
        public void SpecialDomainMapping_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "VIP_PARTNER_001",
                AffDomain = "vip.partner.affiliate.com",
                RedirectDomain = "secure.vip.casino.com",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.AffCode.Should().Be("VIP_PARTNER_001");
            specialDomainMapping.AffDomain.Should().Be("vip.partner.affiliate.com");
            specialDomainMapping.RedirectDomain.Should().Be("secure.vip.casino.com");
            specialDomainMapping.IsSSL.Should().BeTrue();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "BASIC_AFF",
                AffDomain = "basic.affiliate.com"
                // RedirectDomain and IsSSL left as default
            };

            // Assert
            specialDomainMapping.AffCode.Should().Be("BASIC_AFF");
            specialDomainMapping.AffDomain.Should().Be("basic.affiliate.com");
            specialDomainMapping.RedirectDomain.Should().BeNull();
            specialDomainMapping.IsSSL.Should().BeFalse();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleSSLEnabledScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "SECURE_AFF",
                AffDomain = "secure.affiliate.com",
                RedirectDomain = "https://secure.casino.com",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.IsSSL.Should().BeTrue();
            specialDomainMapping.AffDomain.Should().Contain("secure");
            specialDomainMapping.RedirectDomain.Should().StartWith("https://");
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleNonSSLScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "BASIC_AFF",
                AffDomain = "basic.affiliate.com",
                RedirectDomain = "http://basic.casino.com",
                IsSSL = false
            };

            // Assert
            specialDomainMapping.IsSSL.Should().BeFalse();
            specialDomainMapping.RedirectDomain.Should().StartWith("http://");
            specialDomainMapping.RedirectDomain.Should().NotStartWith("https://");
        }

        [Theory]
        [InlineData("VIP_AFF_001", "vip.affiliate.com", "vip.casino.com", true)]
        [InlineData("PREMIUM_AFF_002", "premium.affiliate.com", "premium.casino.com", true)]
        [InlineData("BASIC_AFF_003", "basic.affiliate.com", "basic.casino.com", false)]
        [InlineData("STANDARD_AFF_004", "standard.affiliate.com", "standard.casino.com", false)]
        public void SpecialDomainMapping_ShouldHandleMultipleSpecialMappingScenarios(
            string affCode, string affDomain, string redirectDomain, bool isSSL)
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = affCode,
                AffDomain = affDomain,
                RedirectDomain = redirectDomain,
                IsSSL = isSSL
            };

            // Assert
            specialDomainMapping.AffCode.Should().Be(affCode);
            specialDomainMapping.AffDomain.Should().Be(affDomain);
            specialDomainMapping.RedirectDomain.Should().Be(redirectDomain);
            specialDomainMapping.IsSSL.Should().Be(isSSL);
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleInternationalDomainScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "INTL_ES_AFF",
                AffDomain = "afiliado.casino.es",
                RedirectDomain = "casino.es",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.AffCode.Should().Contain("ES");
            specialDomainMapping.AffDomain.Should().EndWith(".es");
            specialDomainMapping.RedirectDomain.Should().EndWith(".es");
            specialDomainMapping.IsSSL.Should().BeTrue();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleSubdomainScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "SUBDOMAIN_AFF",
                AffDomain = "special.vip.premium.affiliate.com",
                RedirectDomain = "games.casino.vip.com",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.AffDomain.Should().Contain("special.vip.premium");
            specialDomainMapping.RedirectDomain.Should().Contain("games.casino.vip");
            specialDomainMapping.IsSSL.Should().BeTrue();
        }

        [Fact]
        public void AffCode_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var specialDomainMapping = new SpecialDomainMapping();
            const string specialCharsAffCode = "AFF!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            specialDomainMapping.AffCode = specialCharsAffCode;

            // Assert
            specialDomainMapping.AffCode.Should().Be(specialCharsAffCode);
        }

        [Fact]
        public void AffDomain_ShouldHandleIPAddressScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "IP_AFF",
                AffDomain = "192.168.1.100",
                RedirectDomain = "10.0.0.50",
                IsSSL = false
            };

            // Assert
            specialDomainMapping.AffDomain.Should().MatchRegex(@"\d+\.\d+\.\d+\.\d+");
            specialDomainMapping.RedirectDomain.Should().MatchRegex(@"\d+\.\d+\.\d+\.\d+");
        }

        [Fact]
        public void RedirectDomain_ShouldHandlePortNumberScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "PORT_AFF",
                AffDomain = "affiliate.com:8080",
                RedirectDomain = "casino.com:8443",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.AffDomain.Should().Contain(":8080");
            specialDomainMapping.RedirectDomain.Should().Contain(":8443");
            specialDomainMapping.IsSSL.Should().BeTrue();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleLargeDomainNamesScenario()
        {
            // Arrange
            var largeSubdomain = new string('a', 50);
            var specialDomainMapping = new SpecialDomainMapping();

            // Act
            specialDomainMapping.AffDomain = $"{largeSubdomain}.affiliate.com";
            specialDomainMapping.RedirectDomain = $"{largeSubdomain}.casino.com";

            // Assert
            specialDomainMapping.AffDomain.Should().StartWith(largeSubdomain);
            specialDomainMapping.RedirectDomain.Should().StartWith(largeSubdomain);
            specialDomainMapping.AffDomain.Length.Should().BeGreaterThan(50);
            specialDomainMapping.RedirectDomain.Length.Should().BeGreaterThan(50);
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleLocalhostScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "LOCAL_DEV_AFF",
                AffDomain = "localhost:3000",
                RedirectDomain = "localhost:8080",
                IsSSL = false
            };

            // Assert
            specialDomainMapping.AffCode.Should().Contain("LOCAL");
            specialDomainMapping.AffDomain.Should().StartWith("localhost");
            specialDomainMapping.RedirectDomain.Should().StartWith("localhost");
            specialDomainMapping.IsSSL.Should().BeFalse();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "MIN_AFF"
                // All other properties left as default
            };

            // Assert
            specialDomainMapping.AffCode.Should().Be("MIN_AFF");
            specialDomainMapping.AffDomain.Should().BeNull();
            specialDomainMapping.RedirectDomain.Should().BeNull();
            specialDomainMapping.IsSSL.Should().BeFalse();
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleDomainOnlyScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffDomain = "standalone.affiliate.com",
                RedirectDomain = "target.casino.com"
                // AffCode left as null, IsSSL as default
            };

            // Assert
            specialDomainMapping.AffCode.Should().BeNull();
            specialDomainMapping.AffDomain.Should().Be("standalone.affiliate.com");
            specialDomainMapping.RedirectDomain.Should().Be("target.casino.com");
            specialDomainMapping.IsSSL.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, "https://")]
        [InlineData(false, "http://")]
        public void SpecialDomainMapping_ShouldCorrelateSSLWithProtocol(bool isSSL, string expectedProtocol)
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "PROTOCOL_TEST",
                AffDomain = "test.affiliate.com",
                RedirectDomain = $"{expectedProtocol}test.casino.com",
                IsSSL = isSSL
            };

            // Assert
            specialDomainMapping.IsSSL.Should().Be(isSSL);
            specialDomainMapping.RedirectDomain.Should().StartWith(expectedProtocol);
        }

        [Fact]
        public void SpecialDomainMapping_ShouldHandleWildcardDomainScenario()
        {
            // Arrange & Act
            var specialDomainMapping = new SpecialDomainMapping
            {
                AffCode = "WILDCARD_AFF",
                AffDomain = "*.affiliate.com",
                RedirectDomain = "*.casino.com",
                IsSSL = true
            };

            // Assert
            specialDomainMapping.AffDomain.Should().StartWith("*.");
            specialDomainMapping.RedirectDomain.Should().StartWith("*.");
            specialDomainMapping.IsSSL.Should().BeTrue();
        }
    }
}
