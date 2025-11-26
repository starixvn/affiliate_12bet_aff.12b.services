using Aff12B.Ext.API.Infastructure.Command;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Command
{
    public class InsertAffInfosCommandTests
    {
        #region Property Tests

        [Fact]
        public void AffInfosJson_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var command = new InsertAffInfosCommand();
            var jsonString = "[{\"_AffiliateId\":123,\"_AffiliateCode\":\"AFF001\"}]";

            // Act
            command.AffInfosJson = jsonString;

            // Assert
            command.AffInfosJson.Should().Be(jsonString);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("[]")]
        [InlineData("[{\"_AffiliateId\":1}]")]
        [InlineData("[{\"_AffiliateId\":1},{\"_AffiliateId\":2}]")]
        public void AffInfosJson_ShouldHandleVariousValues(string jsonValue)
        {
            // Arrange
            var command = new InsertAffInfosCommand();

            // Act
            command.AffInfosJson = jsonValue;

            // Assert
            command.AffInfosJson.Should().Be(jsonValue);
        }

        [Fact]
        public void AffInfosJson_DefaultValue_ShouldBeNull()
        {
            // Arrange & Act
            var command = new InsertAffInfosCommand();

            // Assert
            command.AffInfosJson.Should().BeNull();
        }

        #endregion

        #region Method Tests

        [Fact]
        public void GetSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new InsertAffInfosCommand();

            // Act
            var settingKey = command.GetSettingKey();

            // Assert
            settingKey.Should().Be("Affiliate_AffiliateInfo_Set");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertAffInfosCommand();

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithVariousPropertyValues_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertAffInfosCommand
            {
                AffInfosJson = null
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithValidJson_ShouldReturnTrue()
        {
            // Arrange
            var command = new InsertAffInfosCommand
            {
                AffInfosJson = "[{\"_AffiliateId\":123}]"
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithInvalidJson_ShouldStillReturnTrue()
        {
            // Arrange - IsValid doesn't validate JSON format
            var command = new InsertAffInfosCommand
            {
                AffInfosJson = "invalid json"
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void InsertAffInfosCommand_ShouldInheritFromNonQueryCommand()
        {
            // Arrange
            var command = new InsertAffInfosCommand();

            // Act & Assert
            command.Should().BeAssignableTo<Fanex.Data.Repository.NonQueryCommand>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertAffInfosCommand_RealWorldScenario_ShouldWork()
        {
            // Arrange & Act
            var jsonData = "[{\"_SiteId\":101,\"_AffiliateId\":67890,\"_AffiliateCode\":\"PREM001\",\"_ParentAffiliate\":\"Master\",\"_AffiliateName\":\"Premium\",\"_CustId\":12345,\"_Username\":\"premium_user\",\"_FirstName\":\"John\",\"_LastName\":\"Smith\",\"_ContactNumber\":\"+1-555-123-4567\",\"_Email\":\"john.smith@premiumpartner.com\",\"_Status\":\"Active\"}]";
            var command = new InsertAffInfosCommand
            {
                AffInfosJson = jsonData
            };

            // Assert
            command.AffInfosJson.Should().Be(jsonData);
            command.GetSettingKey().Should().Be("Affiliate_AffiliateInfo_Set");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffInfosCommand_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var command1 = new InsertAffInfosCommand { AffInfosJson = "[{\"_AffiliateId\":1}]" };
            var command2 = new InsertAffInfosCommand { AffInfosJson = "[{\"_AffiliateId\":2}]" };

            // Act
            command1.AffInfosJson = "[{\"_AffiliateId\":100}]";

            // Assert
            command1.AffInfosJson.Should().Be("[{\"_AffiliateId\":100}]");
            command2.AffInfosJson.Should().Be("[{\"_AffiliateId\":2}]");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void InsertAffInfosCommand_MethodCallsMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var command = new InsertAffInfosCommand();

            // Act
            var settingKey1 = command.GetSettingKey();
            var settingKey2 = command.GetSettingKey();
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();

            // Assert
            settingKey1.Should().Be(settingKey2);
            settingKey1.Should().Be("Affiliate_AffiliateInfo_Set");
            isValid1.Should().Be(isValid2);
            isValid1.Should().BeTrue();
        }

        [Fact]
        public void InsertAffInfosCommand_ExtremeStringValues_ShouldWork()
        {
            // Arrange
            var command = new InsertAffInfosCommand();
            var longString = new string('A', 1000);

            // Act
            command.AffInfosJson = longString;

            // Assert
            command.AffInfosJson.Should().Be(longString);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertAffInfosCommand_LargeJsonArray_ShouldWork()
        {
            // Arrange
            var command = new InsertAffInfosCommand();
            var largeJson = "[" + string.Join(",", Enumerable.Range(1, 100).Select(i => $"{{\"_AffiliateId\":{i}}}")) + "]";

            // Act
            command.AffInfosJson = largeJson;

            // Assert
            command.AffInfosJson.Should().Be(largeJson);
            command.IsValid().Should().BeTrue();
        }

        #endregion
    }
}


