using Aff12B.Ext.API.Infastructure.Command;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Command
{
    public class UnlockUsersCommandTests
    {
        #region Property Tests

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var command = new UnlockUsersCommand();
            const string expectedCustIdList = "[1,2,3,4,5]";

            // Act
            command.CustIdList = expectedCustIdList;

            // Assert
            command.CustIdList.Should().Be(expectedCustIdList);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("[1]")]
        [InlineData("[1,2,3]")]
        [InlineData("[1,2,3,4,5,6,7,8,9,10]")]
        [InlineData("[]")]
        [InlineData("[100,200,300]")]
        [InlineData("invalid-json")]
        [InlineData("1,2,3")]
        [InlineData("{\"ids\":[1,2,3]}")]
        public void CustIdList_ShouldHandleVariousValues(string custIdList)
        {
            // Arrange
            var command = new UnlockUsersCommand();

            // Act
            command.CustIdList = custIdList;

            // Assert
            command.CustIdList.Should().Be(custIdList);
        }

        #endregion

        #region Method Tests

        [Fact]
        public void GetSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new UnlockUsersCommand();

            // Act
            var settingKey = command.GetSettingKey();

            // Assert
            settingKey.Should().Be("API_Customer_Unlock");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new UnlockUsersCommand();

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("[1,2,3]")]
        [InlineData("invalid-json")]
        [InlineData("[]")]
        public void IsValid_WithVariousCustIdListValues_ShouldAlwaysReturnTrue(string custIdList)
        {
            // Arrange
            var command = new UnlockUsersCommand { CustIdList = custIdList };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void UnlockUsersCommand_ShouldInheritFromNonQueryCommand()
        {
            // Arrange
            var command = new UnlockUsersCommand();

            // Act & Assert
            command.Should().BeAssignableTo<Fanex.Data.Repository.NonQueryCommand>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void UnlockUsersCommand_DefaultValues_ShouldBeNull()
        {
            // Arrange & Act
            var command = new UnlockUsersCommand();

            // Assert
            command.CustIdList.Should().BeNull();
        }

        [Fact]
        public void UnlockUsersCommand_RealWorldScenario_SingleUser_ShouldWork()
        {
            // Arrange & Act
            var command = new UnlockUsersCommand
            {
                CustIdList = "[12345]"
            };

            // Assert
            command.CustIdList.Should().Be("[12345]");
            command.GetSettingKey().Should().Be("API_Customer_Unlock");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void UnlockUsersCommand_RealWorldScenario_MultipleUsers_ShouldWork()
        {
            // Arrange & Act
            var command = new UnlockUsersCommand
            {
                CustIdList = "[12345,67890,11111,22222,33333]"
            };

            // Assert
            command.CustIdList.Should().Be("[12345,67890,11111,22222,33333]");
            command.GetSettingKey().Should().Be("API_Customer_Unlock");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void UnlockUsersCommand_EmptyArray_ShouldWork()
        {
            // Arrange & Act
            var command = new UnlockUsersCommand
            {
                CustIdList = "[]"
            };

            // Assert
            command.CustIdList.Should().Be("[]");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void UnlockUsersCommand_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var command1 = new UnlockUsersCommand { CustIdList = "[1,2,3]" };
            var command2 = new UnlockUsersCommand { CustIdList = "[4,5,6]" };

            // Act
            command1.CustIdList = "[100,200]";

            // Assert
            command1.CustIdList.Should().Be("[100,200]");
            command2.CustIdList.Should().Be("[4,5,6]");
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void UnlockUsersCommand_PropertyReassignment_ShouldOverwritePreviousValue()
        {
            // Arrange
            var command = new UnlockUsersCommand
            {
                CustIdList = "[1,2,3]"
            };

            // Act
            command.CustIdList = "[4,5,6]";

            // Assert
            command.CustIdList.Should().Be("[4,5,6]");
        }

        [Fact]
        public void UnlockUsersCommand_MethodCallsMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var command = new UnlockUsersCommand();

            // Act
            var settingKey1 = command.GetSettingKey();
            var settingKey2 = command.GetSettingKey();
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();

            // Assert
            settingKey1.Should().Be(settingKey2);
            settingKey1.Should().Be("API_Customer_Unlock");
            isValid1.Should().Be(isValid2);
            isValid1.Should().BeTrue();
        }

        [Fact]
        public void UnlockUsersCommand_LongJsonString_ShouldWork()
        {
            // Arrange
            var command = new UnlockUsersCommand();
            // Reduced from 1000 to 50 numbers for performance
            var longJsonArray = "[" + string.Join(",", Enumerable.Range(1, 50)) + "]";

            // Act
            command.CustIdList = longJsonArray;

            // Assert
            command.CustIdList.Should().Be(longJsonArray);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void UnlockUsersCommand_InvalidJsonFormats_ShouldStillWork()
        {
            // Arrange & Act
            var testCases = new[]
            {
                "invalid-json",
                "1,2,3,4,5",
                "{\"ids\":[1,2,3]}",
                "[1,2,3,]", // trailing comma
                "[,1,2,3]", // leading comma
                "[[1,2],[3,4]]", // nested arrays
                "not-json-at-all"
            };

            // Assert
            foreach (var testCase in testCases)
            {
                var command = new UnlockUsersCommand { CustIdList = testCase };
                command.CustIdList.Should().Be(testCase);
                command.IsValid().Should().BeTrue();
            }
        }

        #endregion
    }
}
