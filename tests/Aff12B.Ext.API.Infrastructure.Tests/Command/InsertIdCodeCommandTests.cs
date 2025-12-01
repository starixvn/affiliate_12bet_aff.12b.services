using Aff12B.Ext.API.Infastructure.Command;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Command
{
    public class InsertIdCodeCommandTests
    {
        #region Property Tests

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            command.IdCode = "TEST123";
            command.SiteId = 1;
            command.StatusId = 2;

            // Assert
            command.IdCode.Should().Be("TEST123");
            command.SiteId.Should().Be(1);
            command.StatusId.Should().Be(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("TEST123")]
        [InlineData("ID_CODE_WITH_UNDERSCORES")]
        [InlineData("id-code-with-dashes")]
        [InlineData("IdCode!@#$%")]
        [InlineData("VERY_LONG_ID_CODE_WITH_MANY_CHARACTERS_TO_TEST_LENGTH_LIMITS")]
        [InlineData("中文代码")]
        [InlineData("12345")]
        [InlineData("   ")]
        public void IdCode_ShouldHandleVariousValues(string idCode)
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            command.IdCode = idCode;

            // Assert
            command.IdCode.Should().Be(idCode);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(12345)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void SiteId_ShouldHandleVariousValues(int siteId)
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            command.SiteId = siteId;

            // Assert
            command.SiteId.Should().Be(siteId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void StatusId_ShouldHandleVariousValues(int statusId)
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            command.StatusId = statusId;

            // Assert
            command.StatusId.Should().Be(statusId);
        }

        #endregion

        #region Method Tests

        [Fact]
        public void GetSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            var settingKey = command.GetSettingKey();

            // Assert
            settingKey.Should().Be("OH_Link_IDCode_Set");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithVariousPropertyValues_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = null,
                SiteId = -1,
                StatusId = int.MaxValue
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithEmptyIdCode_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "",
                SiteId = 0,
                StatusId = 0
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void InsertIdCodeCommand_ShouldInheritFromNonQueryCommand()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act & Assert
            command.Should().BeAssignableTo<Fanex.Data.Repository.NonQueryCommand>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void InsertIdCodeCommand_DefaultValues_ShouldBeZeroOrNull()
        {
            // Arrange & Act
            var command = new InsertIdCodeCommand();

            // Assert
            command.IdCode.Should().BeNull();
            command.SiteId.Should().Be(0);
            command.StatusId.Should().Be(0);
        }

        [Fact]
        public void InsertIdCodeCommand_RealWorldScenario_ShouldWork()
        {
            // Arrange & Act
            var command = new InsertIdCodeCommand
            {
                IdCode = "PREMIUM_CODE_001",
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            command.IdCode.Should().Be("PREMIUM_CODE_001");
            command.SiteId.Should().Be(1);
            command.StatusId.Should().Be(2);
            command.GetSettingKey().Should().Be("OH_Link_IDCode_Set");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var command1 = new InsertIdCodeCommand { IdCode = "CODE1", SiteId = 1 };
            var command2 = new InsertIdCodeCommand { IdCode = "CODE2", SiteId = 2 };

            // Act
            command1.IdCode = "MODIFIED_CODE1";
            command1.SiteId = 100;

            // Assert
            command1.IdCode.Should().Be("MODIFIED_CODE1");
            command1.SiteId.Should().Be(100);
            command2.IdCode.Should().Be("CODE2");
            command2.SiteId.Should().Be(2);
        }

        [Fact]
        public void InsertIdCodeCommand_AllPropertiesSet_ShouldMaintainValues()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act - Set all properties
            command.IdCode = "TEST_CODE";
            command.SiteId = 1;
            command.StatusId = 2;

            // Modify one property
            command.SiteId = 999;

            // Assert - All other properties should remain unchanged
            command.IdCode.Should().Be("TEST_CODE");
            command.SiteId.Should().Be(999);
            command.StatusId.Should().Be(2);
        }

        [Fact]
        public void InsertIdCodeCommand_MethodCallsMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var command = new InsertIdCodeCommand();

            // Act
            var settingKey1 = command.GetSettingKey();
            var settingKey2 = command.GetSettingKey();
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();

            // Assert
            settingKey1.Should().Be(settingKey2);
            settingKey1.Should().Be("OH_Link_IDCode_Set");
            isValid1.Should().Be(isValid2);
            isValid1.Should().BeTrue();
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void InsertIdCodeCommand_NullIdCode_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = null,
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            command.IdCode.Should().BeNull();
            command.SiteId.Should().Be(1);
            command.StatusId.Should().Be(2);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_EmptyIdCode_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "",
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            command.IdCode.Should().Be("");
            command.SiteId.Should().Be(1);
            command.StatusId.Should().Be(2);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_WhitespaceIdCode_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "   ",
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            command.IdCode.Should().Be("   ");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_ExtremeStringValues_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand();
            var longString = new string('A', 1000);

            // Act
            command.IdCode = longString;

            // Assert
            command.IdCode.Should().Be(longString);
            command.IdCode.Length.Should().Be(1000);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_SpecialCharacters_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand();
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            command.IdCode = specialChars;

            // Assert
            command.IdCode.Should().Be(specialChars);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_NonASCIICharacters_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "中文代码",
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            command.IdCode.Should().Be("中文代码");
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_NegativeSiteIdAndStatusId_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "TEST",
                SiteId = -1,
                StatusId = -100
            };

            // Assert
            command.IdCode.Should().Be("TEST");
            command.SiteId.Should().Be(-1);
            command.StatusId.Should().Be(-100);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_MaxIntegerValues_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "MAX",
                SiteId = int.MaxValue,
                StatusId = int.MaxValue
            };

            // Assert
            command.IdCode.Should().Be("MAX");
            command.SiteId.Should().Be(int.MaxValue);
            command.StatusId.Should().Be(int.MaxValue);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void InsertIdCodeCommand_MinIntegerValues_ShouldWork()
        {
            // Arrange
            var command = new InsertIdCodeCommand
            {
                IdCode = "MIN",
                SiteId = int.MinValue,
                StatusId = int.MinValue
            };

            // Assert
            command.IdCode.Should().Be("MIN");
            command.SiteId.Should().Be(int.MinValue);
            command.StatusId.Should().Be(int.MinValue);
            command.IsValid().Should().BeTrue();
        }

        [Theory]
        [InlineData("CODE1", 1, 1)]
        [InlineData("CODE2", 2, 2)]
        [InlineData("CODE3", 5, 10)]
        [InlineData("", 0, 0)]
        [InlineData(null, -1, -1)]
        [InlineData("ALPHA_123", 100, 200)]
        public void InsertIdCodeCommand_VariousCombinations_ShouldWork(string idCode, int siteId, int statusId)
        {
            // Arrange & Act
            var command = new InsertIdCodeCommand
            {
                IdCode = idCode,
                SiteId = siteId,
                StatusId = statusId
            };

            // Assert
            command.IdCode.Should().Be(idCode);
            command.SiteId.Should().Be(siteId);
            command.StatusId.Should().Be(statusId);
            command.GetSettingKey().Should().Be("OH_Link_IDCode_Set");
            command.IsValid().Should().BeTrue();
        }

        #endregion
    }
}

