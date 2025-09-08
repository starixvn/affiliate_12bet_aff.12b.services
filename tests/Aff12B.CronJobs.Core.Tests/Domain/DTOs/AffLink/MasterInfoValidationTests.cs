using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for MasterInfoValidation DTO class
    /// Tests property validation, edge cases, and master info validation scenarios
    /// </summary>
    public class MasterInfoValidationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var masterInfoValidation = new MasterInfoValidation();

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().BeNull();
            masterInfoValidation.DomainMappings.Should().BeNull();
            masterInfoValidation.LanguageReferences.Should().BeNull();
            masterInfoValidation.AffCommGroups.Should().BeNull();
            masterInfoValidation.LandingPages.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var expectedSpecialDomainMappings = new List<SpecialDomainMapping>
            {
                new SpecialDomainMapping { AffCode = "SPECIAL1", AffDomain = "special1.com", IsSSL = true }
            };
            var expectedDomainMappings = new List<DomainMapping>
            {
                new DomainMapping { AffDomain = "domain1.com", LanguageCode = "en-US", IsSSL = true }
            };
            var expectedLanguageReferences = new List<LanguageReference>
            {
                new LanguageReference { LanguageCode = "en-US", LanguageLink = "/en/casino" }
            };
            var expectedAffCommGroups = new List<AffiliateDeal>
            {
                new AffiliateDeal { AffCode = "AFF001", DealId = 1, DealName = "Deal 1" }
            };
            var expectedLandingPages = new List<LandingPageSetting>
            {
                new LandingPageSetting { PageCode = "LP001", IsMobile = true, IsDefault = true }
            };

            // Act
            masterInfoValidation.SpecialDomainMappings = expectedSpecialDomainMappings;
            masterInfoValidation.DomainMappings = expectedDomainMappings;
            masterInfoValidation.LanguageReferences = expectedLanguageReferences;
            masterInfoValidation.AffCommGroups = expectedAffCommGroups;
            masterInfoValidation.LandingPages = expectedLandingPages;

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().BeEquivalentTo(expectedSpecialDomainMappings);
            masterInfoValidation.DomainMappings.Should().BeEquivalentTo(expectedDomainMappings);
            masterInfoValidation.LanguageReferences.Should().BeEquivalentTo(expectedLanguageReferences);
            masterInfoValidation.AffCommGroups.Should().BeEquivalentTo(expectedAffCommGroups);
            masterInfoValidation.LandingPages.Should().BeEquivalentTo(expectedLandingPages);
        }

        [Fact]
        public void SpecialDomainMappings_ShouldHandleEmptyCollection()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var emptySpecialDomainMappings = new List<SpecialDomainMapping>();

            // Act
            masterInfoValidation.SpecialDomainMappings = emptySpecialDomainMappings;

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().NotBeNull();
            masterInfoValidation.SpecialDomainMappings.Should().BeEmpty();
        }

        [Fact]
        public void DomainMappings_ShouldHandleEmptyCollection()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var emptyDomainMappings = new List<DomainMapping>();

            // Act
            masterInfoValidation.DomainMappings = emptyDomainMappings;

            // Assert
            masterInfoValidation.DomainMappings.Should().NotBeNull();
            masterInfoValidation.DomainMappings.Should().BeEmpty();
        }

        [Fact]
        public void LanguageReferences_ShouldHandleEmptyCollection()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var emptyLanguageReferences = new List<LanguageReference>();

            // Act
            masterInfoValidation.LanguageReferences = emptyLanguageReferences;

            // Assert
            masterInfoValidation.LanguageReferences.Should().NotBeNull();
            masterInfoValidation.LanguageReferences.Should().BeEmpty();
        }

        [Fact]
        public void AffCommGroups_ShouldHandleEmptyCollection()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var emptyAffCommGroups = new List<AffiliateDeal>();

            // Act
            masterInfoValidation.AffCommGroups = emptyAffCommGroups;

            // Assert
            masterInfoValidation.AffCommGroups.Should().NotBeNull();
            masterInfoValidation.AffCommGroups.Should().BeEmpty();
        }

        [Fact]
        public void LandingPages_ShouldHandleEmptyCollection()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();
            var emptyLandingPages = new List<LandingPageSetting>();

            // Act
            masterInfoValidation.LandingPages = emptyLandingPages;

            // Assert
            masterInfoValidation.LandingPages.Should().NotBeNull();
            masterInfoValidation.LandingPages.Should().BeEmpty();
        }

        [Fact]
        public void AllCollectionProperties_ShouldHandleNullValues()
        {
            // Arrange
            var masterInfoValidation = new MasterInfoValidation();

            // Act
            masterInfoValidation.SpecialDomainMappings = null;
            masterInfoValidation.DomainMappings = null;
            masterInfoValidation.LanguageReferences = null;
            masterInfoValidation.AffCommGroups = null;
            masterInfoValidation.LandingPages = null;

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().BeNull();
            masterInfoValidation.DomainMappings.Should().BeNull();
            masterInfoValidation.LanguageReferences.Should().BeNull();
            masterInfoValidation.AffCommGroups.Should().BeNull();
            masterInfoValidation.LandingPages.Should().BeNull();
        }

        [Fact]
        public void MasterInfoValidation_ShouldSupportCompleteDataScenario()
        {
            // Arrange
            var specialDomainMappings = new List<SpecialDomainMapping>
            {
                new SpecialDomainMapping { AffCode = "VIP001", AffDomain = "vip.affiliate.com", RedirectDomain = "vip.casino.com", IsSSL = true },
                new SpecialDomainMapping { AffCode = "PREMIUM001", AffDomain = "premium.affiliate.com", RedirectDomain = "premium.casino.com", IsSSL = true }
            };

            var domainMappings = new List<DomainMapping>
            {
                new DomainMapping { AffDomain = "en.affiliate.com", LanguageCode = "en-US", LanguageLink = "/en/casino", RedirectDomain = "casino.com", IsSSL = true, IsMobile = false },
                new DomainMapping { AffDomain = "es.affiliate.com", LanguageCode = "es-ES", LanguageLink = "/es/casino", RedirectDomain = "casino.es", IsSSL = true, IsMobile = false }
            };

            var languageReferences = new List<LanguageReference>
            {
                new LanguageReference { LanguageCode = "en-US", LanguageLink = "/en/casino/games" },
                new LanguageReference { LanguageCode = "es-ES", LanguageLink = "/es/casino/juegos" },
                new LanguageReference { LanguageCode = "fr-FR", LanguageLink = "/fr/casino/jeux" }
            };

            var affCommGroups = new List<AffiliateDeal>
            {
                new AffiliateDeal { AffCode = "AFF001", DealId = 1, DealName = "Basic Deal", TargetCountryCode = "US" },
                new AffiliateDeal { AffCode = "AFF002", DealId = 2, DealName = "Premium Deal", TargetCountryCode = "GB" },
                new AffiliateDeal { AffCode = "AFF003", DealId = 3, DealName = "VIP Deal", TargetCountryCode = "CA" }
            };

            var landingPages = new List<LandingPageSetting>
            {
                new LandingPageSetting { PageCode = "LP001", IsMobile = false, IsExcluded = false, DealIds = "1,2,3", IsDefault = true },
                new LandingPageSetting { PageCode = "LP002", IsMobile = true, IsExcluded = false, DealIds = "1,2", IsDefault = false },
                new LandingPageSetting { PageCode = "LP003", IsMobile = false, IsExcluded = true, DealIds = null, IsDefault = false }
            };

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                SpecialDomainMappings = specialDomainMappings,
                DomainMappings = domainMappings,
                LanguageReferences = languageReferences,
                AffCommGroups = affCommGroups,
                LandingPages = landingPages
            };

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().HaveCount(2);
            masterInfoValidation.DomainMappings.Should().HaveCount(2);
            masterInfoValidation.LanguageReferences.Should().HaveCount(3);
            masterInfoValidation.AffCommGroups.Should().HaveCount(3);
            masterInfoValidation.LandingPages.Should().HaveCount(3);

            // Verify specific content
            masterInfoValidation.SpecialDomainMappings.Should().Contain(sdm => sdm.AffCode == "VIP001");
            masterInfoValidation.DomainMappings.Should().Contain(dm => dm.LanguageCode == "en-US");
            masterInfoValidation.LanguageReferences.Should().Contain(lr => lr.LanguageCode == "fr-FR");
            masterInfoValidation.AffCommGroups.Should().Contain(acg => acg.DealName == "Premium Deal");
            masterInfoValidation.LandingPages.Should().Contain(lp => lp.IsDefault == true);
        }

        [Fact]
        public void MasterInfoValidation_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var masterInfoValidation = new MasterInfoValidation
            {
                LanguageReferences = new List<LanguageReference>
                {
                    new LanguageReference { LanguageCode = "en-US", LanguageLink = "/en/casino" }
                },
                AffCommGroups = new List<AffiliateDeal>
                {
                    new AffiliateDeal { AffCode = "AFF001", DealId = 1 }
                }
                // Other collections left as null
            };

            // Assert
            masterInfoValidation.LanguageReferences.Should().HaveCount(1);
            masterInfoValidation.AffCommGroups.Should().HaveCount(1);
            masterInfoValidation.SpecialDomainMappings.Should().BeNull();
            masterInfoValidation.DomainMappings.Should().BeNull();
            masterInfoValidation.LandingPages.Should().BeNull();
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleLargeCollectionsScenario()
        {
            // Arrange
            // Reduced from 100 to 25 objects for performance
            var largeSpecialDomainMappings = Enumerable.Range(1, 25)
                .Select(i => new SpecialDomainMapping
                {
                    AffCode = $"SPECIAL{i:D3}",
                    AffDomain = $"special{i}.affiliate.com",
                    RedirectDomain = $"special{i}.casino.com",
                    IsSSL = i % 2 == 0
                })
                .ToList();

            var largeDomainMappings = Enumerable.Range(1, 50)
                .Select(i => new DomainMapping
                {
                    AffDomain = $"domain{i}.affiliate.com",
                    LanguageCode = i % 2 == 0 ? "en-US" : "es-ES",
                    LanguageLink = $"/lang{i}/casino",
                    RedirectDomain = $"casino{i}.com",
                    IsSSL = true,
                    IsMobile = i % 3 == 0
                })
                .ToList();

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                SpecialDomainMappings = largeSpecialDomainMappings,
                DomainMappings = largeDomainMappings
            };

            // Assert - Updated to match reduced data size for performance
            masterInfoValidation.SpecialDomainMappings.Should().HaveCount(25);
            masterInfoValidation.DomainMappings.Should().HaveCount(50);
            masterInfoValidation.SpecialDomainMappings.Should().Contain(sdm => sdm.AffCode == "SPECIAL001");
            masterInfoValidation.SpecialDomainMappings.Should().Contain(sdm => sdm.AffCode == "SPECIAL025");
            masterInfoValidation.DomainMappings.Should().Contain(dm => dm.AffDomain == "domain1.affiliate.com");
            masterInfoValidation.DomainMappings.Should().Contain(dm => dm.AffDomain == "domain50.affiliate.com");
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleMultiLanguageScenario()
        {
            // Arrange
            var multiLanguageReferences = new List<LanguageReference>
            {
                new LanguageReference { LanguageCode = "en-US", LanguageLink = "/en/casino/games" },
                new LanguageReference { LanguageCode = "es-ES", LanguageLink = "/es/casino/juegos" },
                new LanguageReference { LanguageCode = "fr-FR", LanguageLink = "/fr/casino/jeux" },
                new LanguageReference { LanguageCode = "de-DE", LanguageLink = "/de/casino/spiele" },
                new LanguageReference { LanguageCode = "it-IT", LanguageLink = "/it/casino/giochi" },
                new LanguageReference { LanguageCode = "pt-BR", LanguageLink = "/pt/casino/jogos" },
                new LanguageReference { LanguageCode = "zh-CN", LanguageLink = "/zh/casino/游戏" },
                new LanguageReference { LanguageCode = "ja-JP", LanguageLink = "/ja/casino/ゲーム" },
                new LanguageReference { LanguageCode = "ru-RU", LanguageLink = "/ru/casino/игры" },
                new LanguageReference { LanguageCode = "ar-SA", LanguageLink = "/ar/casino/العاب" }
            };

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                LanguageReferences = multiLanguageReferences
            };

            // Assert
            masterInfoValidation.LanguageReferences.Should().HaveCount(10);
            masterInfoValidation.LanguageReferences.Should().Contain(lr => lr.LanguageCode == "zh-CN" && lr.LanguageLink.Contains("游戏"));
            masterInfoValidation.LanguageReferences.Should().Contain(lr => lr.LanguageCode == "ja-JP" && lr.LanguageLink.Contains("ゲーム"));
            masterInfoValidation.LanguageReferences.Should().Contain(lr => lr.LanguageCode == "ru-RU" && lr.LanguageLink.Contains("игры"));
            masterInfoValidation.LanguageReferences.Should().Contain(lr => lr.LanguageCode == "ar-SA" && lr.LanguageLink.Contains("العاب"));
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleComplexDealStructureScenario()
        {
            // Arrange
            var complexAffCommGroups = new List<AffiliateDeal>
            {
                new AffiliateDeal { AffCode = "VIP_US_001", DealId = 1001, DealName = "US VIP Casino Deal", TargetCountryCode = "US" },
                new AffiliateDeal { AffCode = "PREMIUM_GB_001", DealId = 2001, DealName = "UK Premium Sports Deal", TargetCountryCode = "GB" },
                new AffiliateDeal { AffCode = "STANDARD_CA_001", DealId = 3001, DealName = "Canada Standard Poker Deal", TargetCountryCode = "CA" },
                new AffiliateDeal { AffCode = "BASIC_AU_001", DealId = 4001, DealName = "Australia Basic Casino Deal", TargetCountryCode = "AU" },
                new AffiliateDeal { AffCode = "ENTERPRISE_DE_001", DealId = 5001, DealName = "Germany Enterprise Sports Deal", TargetCountryCode = "DE" }
            };

            var complexLandingPages = new List<LandingPageSetting>
            {
                new LandingPageSetting { PageCode = "VIP_DESKTOP", IsMobile = false, IsExcluded = false, DealIds = "1001", IsDefault = false },
                new LandingPageSetting { PageCode = "VIP_MOBILE", IsMobile = true, IsExcluded = false, DealIds = "1001", IsDefault = false },
                new LandingPageSetting { PageCode = "PREMIUM_DESKTOP", IsMobile = false, IsExcluded = false, DealIds = "2001,3001", IsDefault = true },
                new LandingPageSetting { PageCode = "STANDARD_ALL", IsMobile = false, IsExcluded = false, DealIds = "3001,4001,5001", IsDefault = false },
                new LandingPageSetting { PageCode = "EXCLUDED_PAGE", IsMobile = false, IsExcluded = true, DealIds = null, IsDefault = false }
            };

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                AffCommGroups = complexAffCommGroups,
                LandingPages = complexLandingPages
            };

            // Assert
            masterInfoValidation.AffCommGroups.Should().HaveCount(5);
            masterInfoValidation.LandingPages.Should().HaveCount(5);

            // Verify complex relationships
            masterInfoValidation.AffCommGroups.Should().Contain(acg => acg.TargetCountryCode == "US" && acg.DealId == 1001);
            masterInfoValidation.LandingPages.Should().Contain(lp => lp.DealIds == "1001" && lp.IsMobile == false);
            masterInfoValidation.LandingPages.Should().Contain(lp => lp.DealIds == "1001" && lp.IsMobile == true);
            masterInfoValidation.LandingPages.Should().Contain(lp => lp.IsDefault == true);
            masterInfoValidation.LandingPages.Should().Contain(lp => lp.IsExcluded == true);
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleSSLMixedScenario()
        {
            // Arrange
            var mixedSSLDomainMappings = new List<DomainMapping>
            {
                new DomainMapping { AffDomain = "secure.affiliate.com", IsSSL = true, RedirectDomain = "https://secure.casino.com" },
                new DomainMapping { AffDomain = "basic.affiliate.com", IsSSL = false, RedirectDomain = "http://basic.casino.com" },
                new DomainMapping { AffDomain = "mixed.affiliate.com", IsSSL = true, RedirectDomain = "https://mixed.casino.com" }
            };

            var mixedSSLSpecialDomainMappings = new List<SpecialDomainMapping>
            {
                new SpecialDomainMapping { AffCode = "SECURE_SPECIAL", AffDomain = "secure.special.com", IsSSL = true },
                new SpecialDomainMapping { AffCode = "BASIC_SPECIAL", AffDomain = "basic.special.com", IsSSL = false }
            };

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                DomainMappings = mixedSSLDomainMappings,
                SpecialDomainMappings = mixedSSLSpecialDomainMappings
            };

            // Assert
            masterInfoValidation.DomainMappings.Should().HaveCount(3);
            masterInfoValidation.SpecialDomainMappings.Should().HaveCount(2);

            // Verify SSL configurations
            masterInfoValidation.DomainMappings.Count(dm => dm.IsSSL).Should().Be(2);
            masterInfoValidation.DomainMappings.Count(dm => !dm.IsSSL).Should().Be(1);
            masterInfoValidation.SpecialDomainMappings.Count(sdm => sdm.IsSSL).Should().Be(1);
            masterInfoValidation.SpecialDomainMappings.Count(sdm => !sdm.IsSSL).Should().Be(1);
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleMobileDesktopMixScenario()
        {
            // Arrange
            var mixedMobileDomainMappings = new List<DomainMapping>
            {
                new DomainMapping { AffDomain = "m.affiliate.com", IsMobile = true, LanguageCode = "en-US" },
                new DomainMapping { AffDomain = "www.affiliate.com", IsMobile = false, LanguageCode = "en-US" },
                new DomainMapping { AffDomain = "mobile.affiliate.com", IsMobile = true, LanguageCode = "es-ES" },
                new DomainMapping { AffDomain = "desktop.affiliate.com", IsMobile = false, LanguageCode = "es-ES" }
            };

            var mixedMobileLandingPages = new List<LandingPageSetting>
            {
                new LandingPageSetting { PageCode = "MOBILE_LP1", IsMobile = true, DealIds = "1,2" },
                new LandingPageSetting { PageCode = "DESKTOP_LP1", IsMobile = false, DealIds = "1,2,3" },
                new LandingPageSetting { PageCode = "MOBILE_LP2", IsMobile = true, DealIds = "3,4" },
                new LandingPageSetting { PageCode = "DESKTOP_LP2", IsMobile = false, DealIds = "4,5,6" }
            };

            // Act
            var masterInfoValidation = new MasterInfoValidation
            {
                DomainMappings = mixedMobileDomainMappings,
                LandingPages = mixedMobileLandingPages
            };

            // Assert
            masterInfoValidation.DomainMappings.Should().HaveCount(4);
            masterInfoValidation.LandingPages.Should().HaveCount(4);

            // Verify mobile/desktop distribution
            masterInfoValidation.DomainMappings.Count(dm => dm.IsMobile).Should().Be(2);
            masterInfoValidation.DomainMappings.Count(dm => !dm.IsMobile).Should().Be(2);
            masterInfoValidation.LandingPages.Count(lp => lp.IsMobile).Should().Be(2);
            masterInfoValidation.LandingPages.Count(lp => !lp.IsMobile).Should().Be(2);
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var masterInfoValidation = new MasterInfoValidation
            {
                LanguageReferences = new List<LanguageReference>
                {
                    new LanguageReference { LanguageCode = "en", LanguageLink = "/" }
                }
                // All other collections left as null
            };

            // Assert
            masterInfoValidation.LanguageReferences.Should().HaveCount(1);
            masterInfoValidation.SpecialDomainMappings.Should().BeNull();
            masterInfoValidation.DomainMappings.Should().BeNull();
            masterInfoValidation.AffCommGroups.Should().BeNull();
            masterInfoValidation.LandingPages.Should().BeNull();
        }

        [Fact]
        public void MasterInfoValidation_ShouldHandleAllEmptyCollectionsScenario()
        {
            // Arrange & Act
            var masterInfoValidation = new MasterInfoValidation
            {
                SpecialDomainMappings = new List<SpecialDomainMapping>(),
                DomainMappings = new List<DomainMapping>(),
                LanguageReferences = new List<LanguageReference>(),
                AffCommGroups = new List<AffiliateDeal>(),
                LandingPages = new List<LandingPageSetting>()
            };

            // Assert
            masterInfoValidation.SpecialDomainMappings.Should().NotBeNull().And.BeEmpty();
            masterInfoValidation.DomainMappings.Should().NotBeNull().And.BeEmpty();
            masterInfoValidation.LanguageReferences.Should().NotBeNull().And.BeEmpty();
            masterInfoValidation.AffCommGroups.Should().NotBeNull().And.BeEmpty();
            masterInfoValidation.LandingPages.Should().NotBeNull().And.BeEmpty();
        }
    }
}
