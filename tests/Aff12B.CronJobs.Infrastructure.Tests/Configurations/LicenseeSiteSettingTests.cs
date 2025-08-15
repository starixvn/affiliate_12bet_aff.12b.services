using Aff12B.CronJobs.Infrastructure.Configurations;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Configurations
{
    /// <summary>
    /// Streamlined unit tests for LicenseeSiteSetting class.
    /// Reduced from 376 lines to focus on meaningful business scenarios.
    /// Configuration class with actual business logic for site mapping and code prefix resolution.
    /// </summary>
    public class LicenseeSiteSettingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var setting = new LicenseeSiteSetting();

            // Assert
            setting.Should().NotBeNull();
            setting.SupportedSites.Should().BeNull();
            setting.LicenseeSiteMapping.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var supportedSites = new[] { "Site1", "Site2", "Site3" };
            var mappings = new[]
            {
                new LicenseeSiteMapping { SiteName = "Site1", SettingCodePrefix = "S1" },
                new LicenseeSiteMapping { SiteName = "Site2", SettingCodePrefix = "S2" }
            };

            // Act
            var setting = new LicenseeSiteSetting
            {
                SupportedSites = supportedSites,
                LicenseeSiteMapping = mappings
            };

            // Assert
            setting.SupportedSites.Should().BeEquivalentTo(supportedSites);
            setting.LicenseeSiteMapping.Should().BeEquivalentTo(mappings);
        }

        [Fact]
        public void GetCodePrefix_ShouldReturnCorrectPrefix_WhenSiteExists()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "PremiumCasino", SettingCodePrefix = "PC" },
                    new LicenseeSiteMapping { SiteName = "SportsBetting", SettingCodePrefix = "SB" },
                    new LicenseeSiteMapping { SiteName = "LiveCasino", SettingCodePrefix = "LC" }
                }
            };

            // Act & Assert
            setting.GetCodePrefix("PremiumCasino").Should().Be("PC");
            setting.GetCodePrefix("SportsBetting").Should().Be("SB");
            setting.GetCodePrefix("LiveCasino").Should().Be("LC");
        }

        [Fact]
        public void GetCodePrefix_ShouldReturnSiteName_WhenSiteNotFound()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "ExistingSite", SettingCodePrefix = "ES" }
                }
            };

            // Act & Assert
            setting.GetCodePrefix("NonExistentSite").Should().Be("NonExistentSite");
            setting.GetCodePrefix("AnotherMissingSite").Should().Be("AnotherMissingSite");
        }

        [Fact]
        public void GetCodePrefix_ShouldBeCaseInsensitive()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "CasinoSite", SettingCodePrefix = "CS" }
                }
            };

            // Act & Assert
            setting.GetCodePrefix("CasinoSite").Should().Be("CS");
            setting.GetCodePrefix("casinosite").Should().Be("CS");
            setting.GetCodePrefix("CASINOSITE").Should().Be("CS");
            setting.GetCodePrefix("CaSiNoSiTe").Should().Be("CS");
        }

        [Fact]
        public void GetCodePrefix_ShouldHandleNullMapping()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = null
            };

            // Act & Assert
            setting.Invoking(s => s.GetCodePrefix("AnySite"))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetCodePrefix_ShouldHandleEmptyMapping()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new LicenseeSiteMapping[0]
            };

            // Act & Assert
            setting.GetCodePrefix("AnySite").Should().Be("AnySite");
        }

        [Fact]
        public void LicenseeSiteSetting_ShouldSupportMultiTenantScenario()
        {
            // Arrange - Multi-tenant casino platform
            var setting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "CasinoA", "CasinoB", "CasinoC", "SportsA", "SportsB" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "CasinoA", SettingCodePrefix = "CA" },
                    new LicenseeSiteMapping { SiteName = "CasinoB", SettingCodePrefix = "CB" },
                    new LicenseeSiteMapping { SiteName = "CasinoC", SettingCodePrefix = "CC" },
                    new LicenseeSiteMapping { SiteName = "SportsA", SettingCodePrefix = "SA" },
                    new LicenseeSiteMapping { SiteName = "SportsB", SettingCodePrefix = "SB" }
                }
            };

            // Assert - Multi-tenant validations
            setting.SupportedSites.Should().HaveCount(5);
            setting.LicenseeSiteMapping.Should().HaveCount(5);

            // Verify casino sites
            setting.GetCodePrefix("CasinoA").Should().Be("CA");
            setting.GetCodePrefix("CasinoB").Should().Be("CB");
            setting.GetCodePrefix("CasinoC").Should().Be("CC");

            // Verify sports sites
            setting.GetCodePrefix("SportsA").Should().Be("SA");
            setting.GetCodePrefix("SportsB").Should().Be("SB");
        }

        [Fact]
        public void LicenseeSiteSetting_ShouldSupportInternationalSites()
        {
            // Arrange - International sites with regional prefixes
            var setting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "Casino_US", "Casino_UK", "Casino_DE", "Casino_JP", "Casino_CN" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "Casino_US", SettingCodePrefix = "US" },
                    new LicenseeSiteMapping { SiteName = "Casino_UK", SettingCodePrefix = "UK" },
                    new LicenseeSiteMapping { SiteName = "Casino_DE", SettingCodePrefix = "DE" },
                    new LicenseeSiteMapping { SiteName = "Casino_JP", SettingCodePrefix = "JP" },
                    new LicenseeSiteMapping { SiteName = "Casino_CN", SettingCodePrefix = "CN" }
                }
            };

            // Assert - International validations
            setting.GetCodePrefix("Casino_US").Should().Be("US");
            setting.GetCodePrefix("Casino_UK").Should().Be("UK");
            setting.GetCodePrefix("Casino_DE").Should().Be("DE");
            setting.GetCodePrefix("Casino_JP").Should().Be("JP");
            setting.GetCodePrefix("Casino_CN").Should().Be("CN");
        }

        [Fact]
        public void LicenseeSiteSetting_ShouldSupportBrandHierarchy()
        {
            // Arrange - Brand hierarchy with parent-child relationships
            var setting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "MegaCasino", "MegaCasino_VIP", "MegaCasino_Mobile", "MiniCasino", "MiniCasino_Lite" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "MegaCasino", SettingCodePrefix = "MC" },
                    new LicenseeSiteMapping { SiteName = "MegaCasino_VIP", SettingCodePrefix = "MCV" },
                    new LicenseeSiteMapping { SiteName = "MegaCasino_Mobile", SettingCodePrefix = "MCM" },
                    new LicenseeSiteMapping { SiteName = "MiniCasino", SettingCodePrefix = "MI" },
                    new LicenseeSiteMapping { SiteName = "MiniCasino_Lite", SettingCodePrefix = "MIL" }
                }
            };

            // Assert - Brand hierarchy validations
            setting.GetCodePrefix("MegaCasino").Should().Be("MC");
            setting.GetCodePrefix("MegaCasino_VIP").Should().Be("MCV");
            setting.GetCodePrefix("MegaCasino_Mobile").Should().Be("MCM");
            setting.GetCodePrefix("MiniCasino").Should().Be("MI");
            setting.GetCodePrefix("MiniCasino_Lite").Should().Be("MIL");
        }

        [Theory]
        [InlineData("TestSite1", "TS1")]
        [InlineData("TestSite2", "TS2")]
        [InlineData("TestSite3", "TS3")]
        public void GetCodePrefix_ShouldHandleParameterizedSites(string siteName, string expectedPrefix)
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "TestSite1", SettingCodePrefix = "TS1" },
                    new LicenseeSiteMapping { SiteName = "TestSite2", SettingCodePrefix = "TS2" },
                    new LicenseeSiteMapping { SiteName = "TestSite3", SettingCodePrefix = "TS3" }
                }
            };

            // Act & Assert
            setting.GetCodePrefix(siteName).Should().Be(expectedPrefix);
        }

        [Fact]
        public void LicenseeSiteMapping_ShouldSetAndGetProperties()
        {
            // Act
            var mapping = new LicenseeSiteMapping
            {
                SiteName = "TestCasino",
                SettingCodePrefix = "TC"
            };

            // Assert
            mapping.SiteName.Should().Be("TestCasino");
            mapping.SettingCodePrefix.Should().Be("TC");
        }

        [Fact]
        public void LicenseeSiteMapping_ShouldSupportCollectionOperations()
        {
            // Arrange
            var mappings = new[]
            {
                new LicenseeSiteMapping { SiteName = "Site1", SettingCodePrefix = "S1" },
                new LicenseeSiteMapping { SiteName = "Site2", SettingCodePrefix = "S2" },
                new LicenseeSiteMapping { SiteName = "Site3", SettingCodePrefix = "S3" }
            };

            // Act & Assert
            mappings.Should().HaveCount(3);
            mappings.Select(m => m.SiteName).Should().Contain(new[] { "Site1", "Site2", "Site3" });
            mappings.Select(m => m.SettingCodePrefix).Should().Contain(new[] { "S1", "S2", "S3" });
            mappings.First(m => m.SiteName == "Site2").SettingCodePrefix.Should().Be("S2");
        }

        [Fact]
        public void LicenseeSiteSetting_ShouldSupportConfigurationValidation()
        {
            // Arrange - Configuration with validation rules
            var setting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "ValidSite1", "ValidSite2" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "ValidSite1", SettingCodePrefix = "VS1" },
                    new LicenseeSiteMapping { SiteName = "ValidSite2", SettingCodePrefix = "VS2" },
                    new LicenseeSiteMapping { SiteName = "ExtraSite", SettingCodePrefix = "ES" } // Not in supported sites
                }
            };

            // Assert - Configuration validation
            setting.SupportedSites.Should().HaveCount(2);
            setting.LicenseeSiteMapping.Should().HaveCount(3);

            // Verify supported sites work
            setting.GetCodePrefix("ValidSite1").Should().Be("VS1");
            setting.GetCodePrefix("ValidSite2").Should().Be("VS2");

            // Verify extra mapping still works (business decision)
            setting.GetCodePrefix("ExtraSite").Should().Be("ES");
        }

        [Fact]
        public void GetCodePrefix_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "Site-With-Dashes", SettingCodePrefix = "SWD" },
                    new LicenseeSiteMapping { SiteName = "Site_With_Underscores", SettingCodePrefix = "SWU" },
                    new LicenseeSiteMapping { SiteName = "Site.With.Dots", SettingCodePrefix = "SWD2" }
                }
            };

            // Act & Assert
            setting.GetCodePrefix("Site-With-Dashes").Should().Be("SWD");
            setting.GetCodePrefix("Site_With_Underscores").Should().Be("SWU");
            setting.GetCodePrefix("Site.With.Dots").Should().Be("SWD2");
        }

        [Fact]
        public void LicenseeSiteSetting_ShouldSupportDuplicatePrefixScenario()
        {
            // Arrange - Multiple sites with same prefix (edge case)
            var setting = new LicenseeSiteSetting
            {
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "TestSiteA", SettingCodePrefix = "TS" },
                    new LicenseeSiteMapping { SiteName = "TestSiteB", SettingCodePrefix = "TS" } // Same prefix
                }
            };

            // Act & Assert - Both should return the same prefix
            setting.GetCodePrefix("TestSiteA").Should().Be("TS");
            setting.GetCodePrefix("TestSiteB").Should().Be("TS");
        }
    }
}