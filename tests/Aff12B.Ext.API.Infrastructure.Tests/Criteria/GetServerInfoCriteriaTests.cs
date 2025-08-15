using Aff12B.Ext.API.Infastructure.Criteria;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Criteria
{
    /// <summary>
    /// Comprehensive unit tests for GetServerInfoCriteria class.
    /// Business-focused scenarios for server information retrieval criteria.
    /// Covers setting key validation and criteria validity checks.
    /// </summary>
    public class GetServerInfoCriteriaTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var criteria = new GetServerInfoCriteria();

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            settingKey.Should().Be("API_Get_ServerInfo");
        }

        [Fact]
        public void GetSettingKey_ShouldReturnConsistentValue()
        {
            // Arrange - Multiple instances for consistency check
            var criteria1 = new GetServerInfoCriteria();
            var criteria2 = new GetServerInfoCriteria();

            // Act
            var key1 = criteria1.GetSettingKey();
            var key2 = criteria2.GetSettingKey();

            // Assert - Should return same key across instances
            key1.Should().Be(key2);
            key1.Should().Be("API_Get_ServerInfo");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var criteria = new GetServerInfoCriteria();

            // Act
            var isValid = criteria.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldBeConsistentAcrossMultipleCalls()
        {
            // Arrange
            var criteria = new GetServerInfoCriteria();

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
        public void GetServerInfoCriteria_ShouldBeStateless()
        {
            // Arrange - Test statelessness for thread safety
            var criteria1 = new GetServerInfoCriteria();
            var criteria2 = new GetServerInfoCriteria();

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
        public void GetServerInfoCriteria_ShouldSupportMultipleInstancesSimultaneously()
        {
            // Arrange - Business scenario: Multiple concurrent server info requests
            var criteriaList = new List<GetServerInfoCriteria>();

            for (int i = 0; i < 10; i++)
            {
                criteriaList.Add(new GetServerInfoCriteria());
            }

            // Act & Assert - All instances should behave consistently
            foreach (var criteria in criteriaList)
            {
                criteria.GetSettingKey().Should().Be("API_Get_ServerInfo");
                criteria.IsValid().Should().BeTrue();
            }
        }

        [Fact]
        public void GetServerInfoCriteria_ShouldHaveCorrectInheritance()
        {
            // Arrange
            var criteria = new GetServerInfoCriteria();

            // Act & Assert - Should inherit from CriteriaBase
            criteria.Should().BeAssignableTo<Fanex.Data.Repository.CriteriaBase>();
        }

        [Fact]
        public void GetServerInfoCriteria_ShouldSupportBusinessScenario()
        {
            // Arrange - Business scenario: Server configuration retrieval
            var criteria = new GetServerInfoCriteria();

            // Act - Simulate usage in server info retrieval workflow
            var settingKey = criteria.GetSettingKey();
            var isValidForProcessing = criteria.IsValid();

            // Assert - Should be ready for server info API call
            settingKey.Should().NotBeNullOrEmpty();
            settingKey.Should().StartWith("API_");
            settingKey.Should().Contain("ServerInfo");
            isValidForProcessing.Should().BeTrue();
        }

        [Fact]
        public async Task GetServerInfoCriteria_ShouldHandleConcurrentAccess()
        {
            // Arrange - Concurrent access scenario
            var criteria = new GetServerInfoCriteria();
            var tasks = new List<Task<(string key, bool valid)>>();

            // Act - Multiple concurrent calls
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() => (criteria.GetSettingKey(), criteria.IsValid())));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All results should be consistent
            results.Should().HaveCount(20);
            results.Should().AllSatisfy(result =>
            {
                result.key.Should().Be("API_Get_ServerInfo");
                result.valid.Should().BeTrue();
            });
        }

        [Fact]
        public void GetServerInfoCriteria_ShouldMaintainImmutability()
        {
            // Arrange
            var criteria = new GetServerInfoCriteria();

            // Act - Multiple operations that shouldn't change state
            var initialKey = criteria.GetSettingKey();
            var initialValid = criteria.IsValid();

            // Perform multiple operations
            for (int i = 0; i < 5; i++)
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
    }
}
