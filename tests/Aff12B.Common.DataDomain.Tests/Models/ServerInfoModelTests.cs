using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for ServerInfoModel class.
    /// Reduced from 423 lines to focus on meaningful business scenarios.
    /// Server configuration model for multi-site, multi-language, multi-currency platform.
    /// </summary>
    public class ServerInfoModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Act
            var serverInfo = new ServerInfoModel();

            // Assert
            serverInfo.Should().NotBeNull();
            serverInfo.SiteId.Should().Be(0);
            serverInfo.SiteName.Should().BeNull();
            serverInfo.LangList.Should().BeNull();
            serverInfo.CurrencyList.Should().BeNull();
            serverInfo.DefLang.Should().BeNull();
            serverInfo.DefCurrency.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Act
            var serverInfo = new ServerInfoModel
            {
                SiteId = 100,
                SiteName = "Premium Casino",
                LangList = "en,es,pt,zh",
                CurrencyList = "USD,EUR,GBP,CNY",
                DefLang = "en",
                DefCurrency = 1
            };

            // Assert
            serverInfo.SiteId.Should().Be(100);
            serverInfo.SiteName.Should().Be("Premium Casino");
            serverInfo.LangList.Should().Be("en,es,pt,zh");
            serverInfo.CurrencyList.Should().Be("USD,EUR,GBP,CNY");
            serverInfo.DefLang.Should().Be("en");
            serverInfo.DefCurrency.Should().Be(1);
        }

        [Theory]
        [InlineData("en")]
        [InlineData("es")]
        [InlineData("pt")]
        [InlineData("zh")]
        [InlineData("ja")]
        public void DefLang_ShouldSupportVariousLanguageCodes(string langCode)
        {
            // Act
            var serverInfo = new ServerInfoModel { DefLang = langCode };

            // Assert
            serverInfo.DefLang.Should().Be(langCode);
            serverInfo.DefLang.Should().HaveLength(2);
        }

        [Fact]
        public void ServerInfoModel_ShouldSupportMultiSiteConfiguration()
        {
            // Arrange - Multi-site casino configuration
            var serverInfo = new ServerInfoModel
            {
                SiteId = 200,
                SiteName = "Global Sports Betting",
                LangList = "en,es,pt,de,fr,it,zh,ja,ko",
                CurrencyList = "USD,EUR,GBP,CNY,JPY,KRW,BRL",
                DefLang = "en",
                DefCurrency = 1 // USD
            };

            // Assert - Multi-site validations
            serverInfo.SiteId.Should().BePositive();
            serverInfo.SiteName.Should().NotBeNullOrEmpty();
            serverInfo.LangList.Should().Contain(",");
            serverInfo.CurrencyList.Should().Contain(",");
            serverInfo.DefLang.Should().Be("en");
            serverInfo.DefCurrency.Should().BePositive();

            // Language list validations
            var languages = serverInfo.LangList.Split(',');
            languages.Should().Contain("en");
            languages.Should().Contain("zh");
            languages.Should().HaveCountGreaterThan(5);

            // Currency list validations
            var currencies = serverInfo.CurrencyList.Split(',');
            currencies.Should().Contain("USD");
            currencies.Should().Contain("EUR");
            currencies.Should().HaveCountGreaterThan(5);
        }

        [Fact]
        public void ServerInfoModel_ShouldSupportSingleLanguageSite()
        {
            // Arrange - Single language site
            var serverInfo = new ServerInfoModel
            {
                SiteId = 300,
                SiteName = "English Only Casino",
                LangList = "en",
                CurrencyList = "USD,GBP",
                DefLang = "en",
                DefCurrency = 1
            };

            // Assert - Single language validations
            serverInfo.LangList.Should().Be("en");
            serverInfo.LangList.Should().NotContain(",");
            serverInfo.DefLang.Should().Be(serverInfo.LangList);
        }

        [Fact]
        public void ServerInfoModel_ShouldSupportAsianMarketConfiguration()
        {
            // Arrange - Asian market configuration
            var serverInfo = new ServerInfoModel
            {
                SiteId = 888,
                SiteName = "亚洲赌场_Asian_Casino",
                LangList = "zh,ja,ko,th,vi",
                CurrencyList = "CNY,JPY,KRW,THB,VND,USD",
                DefLang = "zh",
                DefCurrency = 3 // CNY
            };

            // Assert - Asian market validations
            serverInfo.SiteName.Should().Contain("亚洲");
            serverInfo.SiteName.Should().Contain("Asian");
            serverInfo.DefLang.Should().Be("zh");
            serverInfo.LangList.Should().Contain("zh,ja,ko");
            serverInfo.CurrencyList.Should().Contain("CNY,JPY,KRW");
        }

        [Fact]
        public void ServerInfoModel_ShouldSupportEuropeanConfiguration()
        {
            // Arrange - European market configuration
            var serverInfo = new ServerInfoModel
            {
                SiteId = 777,
                SiteName = "European Gaming Hub",
                LangList = "en,de,fr,it,es,pt,nl,sv,no,da",
                CurrencyList = "EUR,GBP,CHF,SEK,NOK,DKK",
                DefLang = "en",
                DefCurrency = 2 // EUR
            };

            // Assert - European validations
            var europeanLanguages = serverInfo.LangList.Split(',');
            europeanLanguages.Should().Contain(new[] { "de", "fr", "it", "es" });

            var europeanCurrencies = serverInfo.CurrencyList.Split(',');
            europeanCurrencies.Should().Contain(new[] { "EUR", "GBP", "CHF" });
        }

        [Fact]
        public void ServerInfoModel_ShouldHandleNullableDefCurrency()
        {
            // Act
            var serverInfo = new ServerInfoModel
            {
                SiteId = 123,
                SiteName = "Test Site",
                DefCurrency = null
            };

            // Assert
            serverInfo.DefCurrency.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void DefCurrency_ShouldSupportMajorCurrencies(int currencyId)
        {
            // Act
            var serverInfo = new ServerInfoModel { DefCurrency = currencyId };

            // Assert
            serverInfo.DefCurrency.Should().Be(currencyId);

        }

        [Fact]
        public void ServerInfoModel_ShouldSupportCollectionOperations()
        {
            // Arrange
            var servers = new[]
            {
                new ServerInfoModel { SiteId = 1, SiteName = "Site A", DefLang = "en" },
                new ServerInfoModel { SiteId = 2, SiteName = "Site B", DefLang = "es" },
                new ServerInfoModel { SiteId = 3, SiteName = "Site C", DefLang = "zh" }
            };

            // Act & Assert
            servers.Should().HaveCount(3);
            servers.Where(s => s.DefLang == "en").Should().HaveCount(1);
            servers.Select(s => s.SiteName).Should().Contain(new[] { "Site A", "Site B", "Site C" });
            servers.Max(s => s.SiteId).Should().Be(3);
        }

        [Fact]
        public void ServerInfoModel_ShouldHandleComplexLanguageList()
        {
            // Arrange
            var serverInfo = new ServerInfoModel
            {
                LangList = "en-US,es-ES,pt-BR,zh-CN,zh-TW,ja-JP,ko-KR"
            };

            // Act
            var languages = serverInfo.LangList.Split(',');

            // Assert
            languages.Should().HaveCount(7);
            languages.Should().Contain(lang => lang.Contains("en"));
            languages.Should().Contain(lang => lang.Contains("zh"));
            languages.Where(lang => lang.Contains("-")).Should().HaveCount(7); // All with region codes
        }

        [Fact]
        public void ServerInfoModel_ShouldHandleComplexCurrencyList()
        {
            // Arrange
            var serverInfo = new ServerInfoModel
            {
                CurrencyList = "USD:1,EUR:2,GBP:3,CNY:4,JPY:5,KRW:6,BRL:7"
            };

            // Act
            var currencies = serverInfo.CurrencyList.Split(',');

            // Assert
            currencies.Should().HaveCount(7);
            currencies.Should().Contain(curr => curr.Contains("USD:"));
            currencies.Should().Contain(curr => curr.Contains("EUR:"));
            currencies.Where(curr => curr.Contains(":")).Should().HaveCount(7); // All with IDs
        }
    }
}