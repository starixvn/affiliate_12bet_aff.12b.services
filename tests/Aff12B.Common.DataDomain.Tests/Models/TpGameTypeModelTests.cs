using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Streamlined unit tests for TpGameTypeModel class.
    /// Reduced from 196 lines to focus on meaningful business scenarios.
    /// </summary>
    public class TpGameTypeModelTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var gameType = new TpGameTypeModel();

            // Assert
            gameType.Type.Should().Be(0);
            gameType.TypeName.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange & Act
            var gameType = new TpGameTypeModel
            {
                Type = 5,
                TypeName = "Sports Betting"
            };

            // Assert
            gameType.Type.Should().Be(5);
            gameType.TypeName.Should().Be("Sports Betting");
        }

        [Theory]
        [InlineData(1, "Sports Betting")]
        [InlineData(2, "Casino Games")]
        [InlineData(3, "Live Casino")]
        [InlineData(9, "E-Sports")]
        [InlineData(0, null)]
        [InlineData(-1, "")]
        public void GameType_ShouldHandleVariousScenarios(int type, string typeName)
        {
            // Arrange & Act
            var gameType = new TpGameTypeModel
            {
                Type = type,
                TypeName = typeName
            };

            // Assert
            gameType.Type.Should().Be(type);
            gameType.TypeName.Should().Be(typeName);
        }

        [Fact]
        public void GameType_ShouldSupportUnicodeContent()
        {
            // Arrange & Act
            var gameType = new TpGameTypeModel
            {
                Type = 100,
                TypeName = "体育博彩_Sports_スポーツ"
            };

            // Assert
            gameType.Type.Should().Be(100);
            gameType.TypeName.Should().Contain("Sports");
            gameType.TypeName.Should().Contain("体育博彩");
            gameType.TypeName.Should().Contain("スポーツ");
        }

        [Fact]
        public void GameType_ShouldSupportCollectionOperations()
        {
            // Arrange & Act
            var gameTypes = new[]
            {
                new TpGameTypeModel { Type = 1, TypeName = "Sports" },
                new TpGameTypeModel { Type = 2, TypeName = "Casino" },
                new TpGameTypeModel { Type = 3, TypeName = "Live Casino" }
            };

            // Assert
            gameTypes.Should().HaveCount(3);
            gameTypes.Where(gt => gt.TypeName.Contains("Casino")).Should().HaveCount(2);
            gameTypes.Max(gt => gt.Type).Should().Be(3);
        }
    }
}
