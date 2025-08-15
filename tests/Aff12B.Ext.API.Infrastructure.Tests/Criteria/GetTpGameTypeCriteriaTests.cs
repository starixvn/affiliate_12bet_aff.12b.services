using Aff12B.Ext.API.Infastructure.Criteria;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Criteria
{
    /// <summary>
    /// Comprehensive unit tests for GetTpGameTypeCriteria class.
    /// Business-focused scenarios for third-party game type retrieval criteria.
    /// Covers setting key validation and criteria validity checks for gaming data.
    /// </summary>
    public class GetTpGameTypeCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var criteria = new GetTpGameTypeCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            settingKey.Should().Be("API_Get_TpGameType");
        }

        [Fact]
        public void GetSettingKey_ShouldReturnConsistentValue()
        {
            // Arrange - Multiple instances for consistency check
            var criteria1 = new GetTpGameTypeCriteria();
            var criteria2 = new GetTpGameTypeCriteria();

            // Act
            var key1 = criteria1.GetSettingKey();
            var key2 = criteria2.GetSettingKey();

            // Assert - Should return same key across instances
            key1.Should().Be(key2);
            key1.Should().Be("API_Get_TpGameType");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var criteria = new GetTpGameTypeCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var criteria = new GetTpGameTypeCriteria();

            // Act - Multiple calls
            var result1 = criteria.IsValid();
            var result2 = criteria.IsValid();
            var result3 = criteria.IsValid();

            // Assert - Should always return true
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldBeStateless()
        {
            // Arrange - Test statelessness for thread safety
            var criteria1 = new GetTpGameTypeCriteria();
            var criteria2 = new GetTpGameTypeCriteria();

            // Act
            var key1 = criteria1.GetSettingKey();
            var valid1 = criteria1.IsValid();
            var key2 = criteria2.GetSettingKey();
            var valid2 = criteria2.IsValid();

            // Assert - Both instances should behave identically
            key1.Should().Be(key2);
            valid1.Should().Be(valid2);
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldSupportGamingBusinessScenario()
        {
            // Arrange - Gaming business scenario: Game type classification
            var criteria = new GetTpGameTypeCriteria();

            // Act - Simulate usage in game type retrieval workflow
            var settingKey = criteria.GetSettingKey();
            var isValidForGameData = criteria.IsValid();

            // Assert - Should be ready for third-party game type API call
            settingKey.Should().NotBeNullOrEmpty();
            settingKey.Should().StartWith("API_");
            settingKey.Should().Contain("TpGameType");
            settingKey.Should().Contain("Tp"); // Third-party indicator
            isValidForGameData.Should().BeTrue();
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldSupportMultipleGameProviders()
        {
            // Arrange - Multi-provider scenario: Different game providers
            var criteriaForProvider1 = new GetTpGameTypeCriteria();
            var criteriaForProvider2 = new GetTpGameTypeCriteria();
            var criteriaForProvider3 = new GetTpGameTypeCriteria();

            var providers = new[] { criteriaForProvider1, criteriaForProvider2, criteriaForProvider3 };

            // Act & Assert - All should use same setting key (centralized configuration)
            foreach (var criteria in providers)
            {
                criteria.GetSettingKey().Should().Be("API_Get_TpGameType");
                criteria.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldHaveCorrectInheritance()
        {
            // Arrange
            var criteria = new GetTpGameTypeCriteria();

            // Act & Assert - Should inherit from CriteriaBase
            criteria.Should().BeAssignableTo<Fanex.Data.Repository.CriteriaBase>();
        }

        [Fact]
        public async Task GetTpGameTypeCriteria_ShouldHandleConcurrentGameTypeRequests()
        {
            // Arrange - Concurrent game type requests scenario
            var criteria = new GetTpGameTypeCriteria();
            var tasks = new List<Task<(string key, bool valid)>>();

            // Act - Simulate multiple concurrent game type API calls
            for (int i = 0; i < 15; i++)
            {
                tasks.Add(Task.Run(() => (criteria.GetSettingKey(), criteria.IsValid())));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All results should be consistent for game type queries
            results.Should().HaveCount(15);
            results.Should().AllSatisfy(result =>
            {
                result.key.Should().Be("API_Get_TpGameType");
                result.valid.Should().BeTrue();
            });
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldSupportGameCategoryScenarios()
        {
            // Arrange - Game category scenarios
            var slotGamesCriteria = new GetTpGameTypeCriteria();
            var tableGamesCriteria = new GetTpGameTypeCriteria();
            var liveGamesCriteria = new GetTpGameTypeCriteria();
            var sportsGamesCriteria = new GetTpGameTypeCriteria();

            var gameCategories = new[]
            {
                slotGamesCriteria,
                tableGamesCriteria,
                liveGamesCriteria,
                sportsGamesCriteria
            };

            // Act & Assert - All game categories should use same criteria
            foreach (var criteria in gameCategories)
            {
                var settingKey = criteria.GetSettingKey();
                var isValid = criteria.IsValid();

                settingKey.Should().Be("API_Get_TpGameType");
                isValid.Should().BeTrue();
            }
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldMaintainImmutability()
        {
            // Arrange
            var criteria = new GetTpGameTypeCriteria();

            // Act - Multiple operations that shouldn't change state
            var initialKey = criteria.GetSettingKey();
            var initialValid = criteria.IsValid();

            // Perform multiple operations (simulating heavy usage)
            for (int i = 0; i < 10; i++)
            {
                criteria.GetSettingKey();
                criteria.IsValid();
            }

            var finalKey = criteria.GetSettingKey();
            var finalValid = criteria.IsValid();

            // Assert - State should remain unchanged
            finalKey.Should().Be(initialKey);
            finalValid.Should().Be(initialValid);
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldSupportHighVolumeGameDataRetrieval()
        {
            // Arrange - High volume scenario: Casino with thousands of games
            var criteria = new GetTpGameTypeCriteria();
            var results = new List<(string key, bool valid)>();

            // Act - Simulate high volume game type queries
            for (int i = 0; i < 1000; i++)
            {
                results.Add((criteria.GetSettingKey(), criteria.IsValid()));
            }

            // Assert - Should handle high volume consistently
            results.Should().HaveCount(1000);
            results.Should().AllSatisfy(result =>
            {
                result.key.Should().Be("API_Get_TpGameType");
                result.valid.Should().BeTrue();
            });
        }

        [Fact]
        public void GetTpGameTypeCriteria_ShouldDifferentiateFromOtherCriteria()
        {
            // Arrange - Comparison with other criteria types
            var gameTypeCriteria = new GetTpGameTypeCriteria();
            var serverInfoCriteria = new GetServerInfoCriteria();

            // Act
            var gameTypeKey = gameTypeCriteria.GetSettingKey();
            var serverInfoKey = serverInfoCriteria.GetSettingKey();

            // Assert - Should have different setting keys
            gameTypeKey.Should().Be("API_Get_TpGameType");
            serverInfoKey.Should().Be("API_Get_ServerInfo");
            gameTypeKey.Should().NotBe(serverInfoKey);

            // Both should be valid
            gameTypeCriteria.IsValid().Should().BeTrue();
            serverInfoCriteria.IsValid().Should().BeTrue();
        }
    }
}
