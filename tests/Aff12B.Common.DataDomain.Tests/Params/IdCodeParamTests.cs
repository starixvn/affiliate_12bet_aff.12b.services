using Aff12B.Common.DataDomain.Params;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Params
{
    /// <summary>
    /// Unit tests for IdCodeParam - Target: 95% Coverage
    /// </summary>
    public class IdCodeParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var param = new IdCodeParam();

            // Assert
            param.Should().NotBeNull();
            param.IdCode.Should().BeNull();
            param.SiteId.Should().Be(0);
            param.StatusId.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "TEST123";
            param.SiteId = 1;
            param.StatusId = 2;

            // Assert
            param.IdCode.Should().Be("TEST123");
            param.SiteId.Should().Be(1);
            param.StatusId.Should().Be(2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("TEST123")]
        [InlineData("ID_CODE_WITH_UNDERSCORES")]
        [InlineData("id-code-with-dashes")]
        [InlineData("IdCode123!@#")]
        [InlineData("VERY_LONG_ID_CODE_WITH_MANY_CHARACTERS_TO_TEST_LENGTH")]
        [InlineData("12345")]
        [InlineData("中文代码")]
        [InlineData("   ")]
        public void IdCode_ShouldHandleVariousStringValues(string idCode)
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = idCode;

            // Assert
            param.IdCode.Should().Be(idCode);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        [InlineData(12345)]
        [InlineData(int.MaxValue)]
        public void SiteId_ShouldHandleVariousIntegerValues(int siteId)
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.SiteId = siteId;

            // Assert
            param.SiteId.Should().Be(siteId);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void StatusId_ShouldHandleVariousIntegerValues(int statusId)
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.StatusId = statusId;

            // Assert
            param.StatusId.Should().Be(statusId);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleCompleteIdCodeInfo()
        {
            // Arrange & Act
            var param = new IdCodeParam
            {
                IdCode = "PREMIUM_CODE_001",
                SiteId = 1,
                StatusId = 2
            };

            // Assert
            param.IdCode.Should().Be("PREMIUM_CODE_001");
            param.SiteId.Should().Be(1);
            param.StatusId.Should().Be(2);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleMinimalInfo()
        {
            // Arrange & Act
            var param = new IdCodeParam
            {
                IdCode = "MIN"
            };

            // Assert
            param.IdCode.Should().Be("MIN");
            param.SiteId.Should().Be(0);
            param.StatusId.Should().Be(0);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var param = new IdCodeParam();
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            param.IdCode = specialChars;

            // Assert
            param.IdCode.Should().Be(specialChars);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleEmptyString()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "";

            // Assert
            param.IdCode.Should().Be("");
        }

        [Fact]
        public void IdCodeParam_ShouldHandleWhitespaceString()
        {
            // Arrange
            var param = new IdCodeParam();
            var whitespace = "   ";

            // Act
            param.IdCode = whitespace;

            // Assert
            param.IdCode.Should().Be(whitespace);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleNullIdCode()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = null;
            param.SiteId = 1;
            param.StatusId = 2;

            // Assert
            param.IdCode.Should().BeNull();
            param.SiteId.Should().Be(1);
            param.StatusId.Should().Be(2);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleZeroSiteIdAndStatusId()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "TEST";
            param.SiteId = 0;
            param.StatusId = 0;

            // Assert
            param.IdCode.Should().Be("TEST");
            param.SiteId.Should().Be(0);
            param.StatusId.Should().Be(0);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleNegativeSiteIdAndStatusId()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "TEST";
            param.SiteId = -1;
            param.StatusId = -100;

            // Assert
            param.IdCode.Should().Be("TEST");
            param.SiteId.Should().Be(-1);
            param.StatusId.Should().Be(-100);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleMaxIntegerValues()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "MAX_VALUES";
            param.SiteId = int.MaxValue;
            param.StatusId = int.MaxValue;

            // Assert
            param.IdCode.Should().Be("MAX_VALUES");
            param.SiteId.Should().Be(int.MaxValue);
            param.StatusId.Should().Be(int.MaxValue);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleMinIntegerValues()
        {
            // Arrange
            var param = new IdCodeParam();

            // Act
            param.IdCode = "MIN_VALUES";
            param.SiteId = int.MinValue;
            param.StatusId = int.MinValue;

            // Assert
            param.IdCode.Should().Be("MIN_VALUES");
            param.SiteId.Should().Be(int.MinValue);
            param.StatusId.Should().Be(int.MinValue);
        }

        [Fact]
        public void IdCodeParam_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var param1 = new IdCodeParam { IdCode = "CODE1", SiteId = 1, StatusId = 1 };
            var param2 = new IdCodeParam { IdCode = "CODE2", SiteId = 2, StatusId = 2 };

            // Act
            param1.IdCode = "MODIFIED_CODE1";
            param1.SiteId = 100;

            // Assert
            param1.IdCode.Should().Be("MODIFIED_CODE1");
            param1.SiteId.Should().Be(100);
            param1.StatusId.Should().Be(1);
            param2.IdCode.Should().Be("CODE2");
            param2.SiteId.Should().Be(2);
            param2.StatusId.Should().Be(2);
        }

        [Fact]
        public void IdCodeParam_PropertyModification_ShouldNotAffectOtherProperties()
        {
            // Arrange
            var param = new IdCodeParam
            {
                IdCode = "TEST",
                SiteId = 1,
                StatusId = 2
            };

            // Act - Modify one property
            param.SiteId = 999;

            // Assert - Other properties should remain unchanged
            param.IdCode.Should().Be("TEST");
            param.SiteId.Should().Be(999);
            param.StatusId.Should().Be(2);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleLongString()
        {
            // Arrange
            var param = new IdCodeParam();
            var longString = new string('A', 1000);

            // Act
            param.IdCode = longString;

            // Assert
            param.IdCode.Should().Be(longString);
            param.IdCode.Length.Should().Be(1000);
        }

        [Fact]
        public void IdCodeParam_ShouldHandleAlphanumericCodes()
        {
            // Arrange & Act
            var param = new IdCodeParam
            {
                IdCode = "ABC123XYZ789",
                SiteId = 42,
                StatusId = 3
            };

            // Assert
            param.IdCode.Should().Be("ABC123XYZ789");
            param.SiteId.Should().Be(42);
            param.StatusId.Should().Be(3);
        }

        [Theory]
        [InlineData("CODE1", 1, 1)]
        [InlineData("CODE2", 2, 2)]
        [InlineData("CODE3", 5, 10)]
        [InlineData("", 0, 0)]
        [InlineData(null, -1, -1)]
        public void IdCodeParam_ShouldHandleVariousCombinations(string idCode, int siteId, int statusId)
        {
            // Arrange & Act
            var param = new IdCodeParam
            {
                IdCode = idCode,
                SiteId = siteId,
                StatusId = statusId
            };

            // Assert
            param.IdCode.Should().Be(idCode);
            param.SiteId.Should().Be(siteId);
            param.StatusId.Should().Be(statusId);
        }
    }
}

