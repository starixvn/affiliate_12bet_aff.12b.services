using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs.AffLink
{
    /// <summary>
    /// Unit tests for LanguageReference DTO class
    /// Tests property validation, edge cases, and language reference scenarios
    /// </summary>
    public class LanguageReferenceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var languageReference = new LanguageReference();

            // Assert
            languageReference.LanguageCode.Should().BeNull();
            languageReference.LanguageLink.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var languageReference = new LanguageReference();
            const string expectedLanguageCode = "en-US";
            const string expectedLanguageLink = "/en/casino/games";

            // Act
            languageReference.LanguageCode = expectedLanguageCode;
            languageReference.LanguageLink = expectedLanguageLink;

            // Assert
            languageReference.LanguageCode.Should().Be(expectedLanguageCode);
            languageReference.LanguageLink.Should().Be(expectedLanguageLink);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        [InlineData("es")]
        [InlineData("es-ES")]
        [InlineData("es-MX")]
        [InlineData("fr")]
        [InlineData("fr-FR")]
        [InlineData("fr-CA")]
        [InlineData("de")]
        [InlineData("de-DE")]
        [InlineData("de-AT")]
        [InlineData("it")]
        [InlineData("it-IT")]
        [InlineData("pt")]
        [InlineData("pt-BR")]
        [InlineData("pt-PT")]
        [InlineData("zh")]
        [InlineData("zh-CN")]
        [InlineData("zh-TW")]
        [InlineData("ja")]
        [InlineData("ja-JP")]
        [InlineData("ko")]
        [InlineData("ko-KR")]
        [InlineData("ru")]
        [InlineData("ru-RU")]
        [InlineData("ar")]
        [InlineData("ar-SA")]
        [InlineData("hi")]
        [InlineData("hi-IN")]
        public void LanguageCode_ShouldHandleVariousLanguageCodes(string languageCode)
        {
            // Arrange
            var languageReference = new LanguageReference();

            // Act
            languageReference.LanguageCode = languageCode;

            // Assert
            languageReference.LanguageCode.Should().Be(languageCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("/")]
        [InlineData("/en")]
        [InlineData("/en/casino")]
        [InlineData("/en/casino/games")]
        [InlineData("/es/deportes")]
        [InlineData("/es/casino/juegos")]
        [InlineData("/fr/casino")]
        [InlineData("/fr/casino/jeux")]
        [InlineData("/de/casino")]
        [InlineData("/de/casino/spiele")]
        [InlineData("/it/casino")]
        [InlineData("/it/casino/giochi")]
        [InlineData("/pt/casino")]
        [InlineData("/pt/casino/jogos")]
        [InlineData("/zh/casino")]
        [InlineData("/zh/casino/游戏")]
        [InlineData("/ja/casino")]
        [InlineData("/ja/casino/ゲーム")]
        [InlineData("/ru/casino")]
        [InlineData("/ru/casino/игры")]
        public void LanguageLink_ShouldHandleVariousLinkFormats(string languageLink)
        {
            // Arrange
            var languageReference = new LanguageReference();

            // Act
            languageReference.LanguageLink = languageLink;

            // Assert
            languageReference.LanguageLink.Should().Be(languageLink);
        }

        [Fact]
        public void LanguageCode_ShouldHandleNullValue()
        {
            // Arrange
            var languageReference = new LanguageReference();

            // Act
            languageReference.LanguageCode = null;

            // Assert
            languageReference.LanguageCode.Should().BeNull();
        }

        [Fact]
        public void LanguageLink_ShouldHandleNullValue()
        {
            // Arrange
            var languageReference = new LanguageReference();

            // Act
            languageReference.LanguageLink = null;

            // Assert
            languageReference.LanguageLink.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var languageReference = new LanguageReference();
            const string unicodeLanguageCode = "语言代码_José_García_🌐";
            const string unicodeLanguageLink = "/语言链接_José_García_🔗";

            // Act
            languageReference.LanguageCode = unicodeLanguageCode;
            languageReference.LanguageLink = unicodeLanguageLink;

            // Assert
            languageReference.LanguageCode.Should().Be(unicodeLanguageCode);
            languageReference.LanguageLink.Should().Be(unicodeLanguageLink);
        }

        [Fact]
        public void LanguageReference_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "en-US",
                LanguageLink = "/en/casino/games?category=slots"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("en-US");
            languageReference.LanguageLink.Should().Be("/en/casino/games?category=slots");
        }

        [Fact]
        public void LanguageReference_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "es"
                // LanguageLink left as default/null
            };

            // Assert
            languageReference.LanguageCode.Should().Be("es");
            languageReference.LanguageLink.Should().BeNull();
        }

        [Theory]
        [InlineData("en-US", "/en/casino")]
        [InlineData("es-ES", "/es/casino")]
        [InlineData("fr-FR", "/fr/casino")]
        [InlineData("de-DE", "/de/casino")]
        [InlineData("it-IT", "/it/casino")]
        [InlineData("pt-BR", "/pt/casino")]
        [InlineData("zh-CN", "/zh/casino")]
        [InlineData("ja-JP", "/ja/casino")]
        [InlineData("ru-RU", "/ru/casino")]
        [InlineData("ar-SA", "/ar/casino")]
        public void LanguageReference_ShouldHandleMultipleLanguageScenarios(string languageCode, string languageLink)
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = languageCode,
                LanguageLink = languageLink
            };

            // Assert
            languageReference.LanguageCode.Should().Be(languageCode);
            languageReference.LanguageLink.Should().Be(languageLink);

            // Validate that language code and link are consistent
            var expectedLanguagePrefix = languageCode.Split('-')[0];
            languageReference.LanguageLink.Should().Contain($"/{expectedLanguagePrefix}/");
        }

        [Fact]
        public void LanguageReference_ShouldHandleComplexLanguageLinkScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "en-US",
                LanguageLink = "/en/casino/games?category=slots&provider=netent&bonus=welcome&utm_source=affiliate"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("en-US");
            languageReference.LanguageLink.Should().Contain("?");
            languageReference.LanguageLink.Should().Contain("&");
            languageReference.LanguageLink.Should().Contain("category=slots");
            languageReference.LanguageLink.Should().Contain("utm_source=affiliate");
        }

        [Fact]
        public void LanguageReference_ShouldHandleRootLanguageLinkScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "en",
                LanguageLink = "/"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("en");
            languageReference.LanguageLink.Should().Be("/");
        }

        [Fact]
        public void LanguageReference_ShouldHandleDeepLanguageLinkScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "es-MX",
                LanguageLink = "/es/casino/juegos/tragamonedas/categoria/aventura/proveedor/playtech"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("es-MX");
            languageReference.LanguageLink.Should().StartWith("/es/");
            languageReference.LanguageLink.Should().Contain("tragamonedas");
            languageReference.LanguageLink.Should().Contain("aventura");
            languageReference.LanguageLink.Should().Contain("playtech");
        }

        [Fact]
        public void LanguageCode_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var languageReference = new LanguageReference();
            const string specialCharsLanguageCode = "lang-CODE_with.special@chars";

            // Act
            languageReference.LanguageCode = specialCharsLanguageCode;

            // Assert
            languageReference.LanguageCode.Should().Be(specialCharsLanguageCode);
        }

        [Fact]
        public void LanguageLink_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var languageReference = new LanguageReference();
            const string specialCharsLanguageLink = "/special-chars_in.link@path?param=value&other=test";

            // Act
            languageReference.LanguageLink = specialCharsLanguageLink;

            // Assert
            languageReference.LanguageLink.Should().Be(specialCharsLanguageLink);
        }

        [Fact]
        public void LanguageLink_ShouldHandleLargeString()
        {
            // Arrange
            var languageReference = new LanguageReference();
            var largeLanguageLink = "/en/casino/games" + new string('/', 1000) + "very-deep-path";

            // Act
            languageReference.LanguageLink = largeLanguageLink;

            // Assert
            languageReference.LanguageLink.Should().Be(largeLanguageLink);
            languageReference.LanguageLink.Should().StartWith("/en/casino/games");
            languageReference.LanguageLink.Should().EndWith("very-deep-path");
        }

        [Fact]
        public void LanguageReference_ShouldHandleAbsoluteUrlScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "fr-CA",
                LanguageLink = "https://fr.casino.com/jeux/machines-a-sous"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("fr-CA");
            languageReference.LanguageLink.Should().StartWith("https://");
            languageReference.LanguageLink.Should().Contain("fr.casino.com");
            languageReference.LanguageLink.Should().Contain("machines-a-sous");
        }

        [Fact]
        public void LanguageReference_ShouldHandleHashFragmentScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "de-AT",
                LanguageLink = "/de/casino/spiele#slots-bereich"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("de-AT");
            languageReference.LanguageLink.Should().Contain("#");
            languageReference.LanguageLink.Should().EndWith("slots-bereich");
        }

        [Fact]
        public void LanguageReference_ShouldHandleMinimalValidDataScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "en"
                // LanguageLink left as default/null
            };

            // Assert
            languageReference.LanguageCode.Should().Be("en");
            languageReference.LanguageLink.Should().BeNull();
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var languageReference = new LanguageReference();

            // Act
            languageReference.LanguageCode = null;
            languageReference.LanguageLink = null;

            // Assert
            languageReference.LanguageCode.Should().BeNull();
            languageReference.LanguageLink.Should().BeNull();
        }

        [Fact]
        public void LanguageReference_ShouldHandleAsianLanguageScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "zh-CN",
                LanguageLink = "/zh/赌场/游戏?类别=老虎机"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("zh-CN");
            languageReference.LanguageLink.Should().Contain("赌场");
            languageReference.LanguageLink.Should().Contain("游戏");
            languageReference.LanguageLink.Should().Contain("老虎机");
        }

        [Fact]
        public void LanguageReference_ShouldHandleRightToLeftLanguageScenario()
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = "ar-SA",
                LanguageLink = "/ar/كازينو/العاب?فئة=سلوتس"
            };

            // Assert
            languageReference.LanguageCode.Should().Be("ar-SA");
            languageReference.LanguageLink.Should().Contain("كازينو");
            languageReference.LanguageLink.Should().Contain("العاب");
            languageReference.LanguageLink.Should().Contain("سلوتس");
        }

        [Theory]
        [InlineData("EN-US", "/EN/CASINO")]
        [InlineData("en-us", "/en/casino")]
        [InlineData("En-Us", "/En/Casino")]
        public void LanguageReference_ShouldPreserveCasing(string languageCode, string languageLink)
        {
            // Arrange & Act
            var languageReference = new LanguageReference
            {
                LanguageCode = languageCode,
                LanguageLink = languageLink
            };

            // Assert - Should preserve original casing
            languageReference.LanguageCode.Should().Be(languageCode);
            languageReference.LanguageLink.Should().Be(languageLink);
        }
    }
}
