using Aff12B.CronJobs.Core.Domain.DTOs;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs
{
    /// <summary>
    /// Unit tests for DataTrackerItem DTO class
    /// Tests property validation, edge cases, and data tracking scenarios
    /// </summary>
    public class DataTrackerItemTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var dataTrackerItem = new DataTrackerItem();

            // Assert
            dataTrackerItem.PrefixSiteName.Should().BeNull();
            dataTrackerItem.PushTrackerId.Should().Be(0);
            dataTrackerItem.StatusId.Should().Be(0);
            dataTrackerItem.PushDataType.Should().Be(0);
            dataTrackerItem.PushDataInfo.Should().BeNull();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            const string expectedPrefixSiteName = "SITE001";
            const long expectedPushTrackerId = 123456789L;
            const int expectedStatusId = 1;
            const int expectedPushDataType = 2;
            const string expectedPushDataInfo = "Sample push data information";

            // Act
            dataTrackerItem.PrefixSiteName = expectedPrefixSiteName;
            dataTrackerItem.PushTrackerId = expectedPushTrackerId;
            dataTrackerItem.StatusId = expectedStatusId;
            dataTrackerItem.PushDataType = expectedPushDataType;
            dataTrackerItem.PushDataInfo = expectedPushDataInfo;

            // Assert
            dataTrackerItem.PrefixSiteName.Should().Be(expectedPrefixSiteName);
            dataTrackerItem.PushTrackerId.Should().Be(expectedPushTrackerId);
            dataTrackerItem.StatusId.Should().Be(expectedStatusId);
            dataTrackerItem.PushDataType.Should().Be(expectedPushDataType);
            dataTrackerItem.PushDataInfo.Should().Be(expectedPushDataInfo);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("SITE001")]
        [InlineData("site_with_underscores")]
        [InlineData("site-with-dashes")]
        [InlineData("site.with.dots")]
        [InlineData("123456")]
        [InlineData("MixedCaseSite")]
        public void PrefixSiteName_ShouldHandleVariousStringFormats(string prefixSiteName)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PrefixSiteName = prefixSiteName;

            // Assert
            dataTrackerItem.PrefixSiteName.Should().Be(prefixSiteName);
        }

        [Fact]
        public void PrefixSiteName_ShouldHandleNullValue()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PrefixSiteName = null;

            // Assert
            dataTrackerItem.PrefixSiteName.Should().BeNull();
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(-1L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        [InlineData(123456789012345L)]
        public void PushTrackerId_ShouldHandleVariousLongValues(long pushTrackerId)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PushTrackerId = pushTrackerId;

            // Assert
            dataTrackerItem.PushTrackerId.Should().Be(pushTrackerId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void StatusId_ShouldHandleVariousIntegerValues(int statusId)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.StatusId = statusId;

            // Assert
            dataTrackerItem.StatusId.Should().Be(statusId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void PushDataType_ShouldHandleVariousIntegerValues(int pushDataType)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PushDataType = pushDataType;

            // Assert
            dataTrackerItem.PushDataType.Should().Be(pushDataType);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Simple data info")]
        [InlineData("Data with special characters !@#$%^&*()")]
        [InlineData("Data\nwith\nnewlines")]
        [InlineData("Data\twith\ttabs")]
        public void PushDataInfo_ShouldHandleVariousStringFormats(string pushDataInfo)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PushDataInfo = pushDataInfo;

            // Assert
            dataTrackerItem.PushDataInfo.Should().Be(pushDataInfo);
        }

        [Fact]
        public void PushDataInfo_ShouldHandleNullValue()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PushDataInfo = null;

            // Assert
            dataTrackerItem.PushDataInfo.Should().BeNull();
        }

        [Fact]
        public void PushDataInfo_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            const string unicodeData = "数据信息 José García Müller 🚀📊";

            // Act
            dataTrackerItem.PushDataInfo = unicodeData;

            // Assert
            dataTrackerItem.PushDataInfo.Should().Be(unicodeData);
        }

        [Fact]
        public void PushDataInfo_ShouldHandleLargeString()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            var largeData = new string('D', 50000); // 50KB string

            // Act
            dataTrackerItem.PushDataInfo = largeData;

            // Assert
            dataTrackerItem.PushDataInfo.Should().Be(largeData);
            dataTrackerItem.PushDataInfo.Length.Should().Be(50000);
        }

        [Fact]
        public void DataTrackerItem_ShouldSupportCompleteDataScenario()
        {
            // Arrange & Act
            var dataTrackerItem = new DataTrackerItem
            {
                PrefixSiteName = "PROD_SITE",
                PushTrackerId = 987654321L,
                StatusId = 1,
                PushDataType = 3,
                PushDataInfo = "Complete data tracking information with all fields populated"
            };

            // Assert
            dataTrackerItem.PrefixSiteName.Should().Be("PROD_SITE");
            dataTrackerItem.PushTrackerId.Should().Be(987654321L);
            dataTrackerItem.StatusId.Should().Be(1);
            dataTrackerItem.PushDataType.Should().Be(3);
            dataTrackerItem.PushDataInfo.Should().Be("Complete data tracking information with all fields populated");
        }

        [Fact]
        public void DataTrackerItem_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var dataTrackerItem = new DataTrackerItem
            {
                PushTrackerId = 123456L,
                StatusId = 2
                // Other properties left as default/null
            };

            // Assert
            dataTrackerItem.PushTrackerId.Should().Be(123456L);
            dataTrackerItem.StatusId.Should().Be(2);
            dataTrackerItem.PrefixSiteName.Should().BeNull();
            dataTrackerItem.PushDataType.Should().Be(0);
            dataTrackerItem.PushDataInfo.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(0)]
        [InlineData(-1)]
        public void StatusId_ShouldRepresentDifferentStates(int statusId)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.StatusId = statusId;

            // Assert
            dataTrackerItem.StatusId.Should().Be(statusId);
            // Note: This test validates that different status IDs can be set
            // The expected description is for documentation purposes only
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(0)]
        public void PushDataType_ShouldRepresentDifferentDataTypes(int pushDataType)
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PushDataType = pushDataType;

            // Assert
            dataTrackerItem.PushDataType.Should().Be(pushDataType);
            // Note: This test validates that different data types can be set
            // The expected description is for documentation purposes only
        }

        [Fact]
        public void PushDataInfo_ShouldHandleJsonData()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            const string jsonData = "{\"recordCount\":1500,\"lastProcessed\":\"2024-12-16T14:30:00Z\",\"errors\":[]}";

            // Act
            dataTrackerItem.PushDataInfo = jsonData;

            // Assert
            dataTrackerItem.PushDataInfo.Should().Be(jsonData);
        }

        [Fact]
        public void PushDataInfo_ShouldHandleXmlData()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            const string xmlData = "<data><recordCount>1500</recordCount><lastProcessed>2024-12-16T14:30:00Z</lastProcessed></data>";

            // Act
            dataTrackerItem.PushDataInfo = xmlData;

            // Assert
            dataTrackerItem.PushDataInfo.Should().Be(xmlData);
        }

        [Fact]
        public void PrefixSiteName_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();
            const string unicodeSiteName = "网站前缀_José_García_🏢";

            // Act
            dataTrackerItem.PrefixSiteName = unicodeSiteName;

            // Assert
            dataTrackerItem.PrefixSiteName.Should().Be(unicodeSiteName);
        }

        [Fact]
        public void AllStringProperties_ShouldHandleNullValues()
        {
            // Arrange
            var dataTrackerItem = new DataTrackerItem();

            // Act
            dataTrackerItem.PrefixSiteName = null;
            dataTrackerItem.PushDataInfo = null;

            // Assert
            dataTrackerItem.PrefixSiteName.Should().BeNull();
            dataTrackerItem.PushDataInfo.Should().BeNull();
        }

        [Fact]
        public void DataTrackerItem_ShouldHandleMinimumValidScenario()
        {
            // Arrange & Act
            var dataTrackerItem = new DataTrackerItem
            {
                PushTrackerId = 1L
                // All other properties left as default
            };

            // Assert
            dataTrackerItem.PushTrackerId.Should().Be(1L);
            dataTrackerItem.StatusId.Should().Be(0);
            dataTrackerItem.PushDataType.Should().Be(0);
            dataTrackerItem.PrefixSiteName.Should().BeNull();
            dataTrackerItem.PushDataInfo.Should().BeNull();
        }

        [Fact]
        public void DataTrackerItem_ShouldHandleMaximumDataScenario()
        {
            // Arrange & Act
            var dataTrackerItem = new DataTrackerItem
            {
                PrefixSiteName = new string('S', 1000), // Large site name
                PushTrackerId = long.MaxValue,
                StatusId = int.MaxValue,
                PushDataType = int.MaxValue,
                PushDataInfo = new string('I', 10000) // Large data info
            };

            // Assert
            dataTrackerItem.PrefixSiteName.Length.Should().Be(1000);
            dataTrackerItem.PushTrackerId.Should().Be(long.MaxValue);
            dataTrackerItem.StatusId.Should().Be(int.MaxValue);
            dataTrackerItem.PushDataType.Should().Be(int.MaxValue);
            dataTrackerItem.PushDataInfo.Length.Should().Be(10000);
        }
    }
}
