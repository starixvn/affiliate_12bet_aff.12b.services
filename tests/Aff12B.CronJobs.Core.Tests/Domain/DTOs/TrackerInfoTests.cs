using Aff12B.CronJobs.Core.Domain.DTOs;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs
{
    /// <summary>
    /// Unit tests for TrackerInfo DTO class
    /// Tests property validation, edge cases, and time handling
    /// </summary>
    public class TrackerInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var trackerInfo = new TrackerInfo();

            // Assert
            trackerInfo.CalledTime.Should().Be(default(DateTime));
            trackerInfo.KeyValue.Should().BeNull();
            trackerInfo.IsReset.Should().BeFalse();
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var expectedCalledTime = new DateTime(2024, 12, 16, 14, 30, 0);
            const string expectedKeyValue = "test-key-12345";
            const bool expectedIsReset = true;

            // Act
            trackerInfo.CalledTime = expectedCalledTime;
            trackerInfo.KeyValue = expectedKeyValue;
            trackerInfo.IsReset = expectedIsReset;

            // Assert
            trackerInfo.CalledTime.Should().Be(expectedCalledTime);
            trackerInfo.KeyValue.Should().Be(expectedKeyValue);
            trackerInfo.IsReset.Should().Be(expectedIsReset);
        }

        [Theory]
        [InlineData(2024, 1, 1, 0, 0, 0)]
        [InlineData(2024, 12, 31, 23, 59, 59)]
        [InlineData(2000, 1, 1, 12, 0, 0)]
        [InlineData(2099, 12, 31, 12, 0, 0)]
        public void CalledTime_ShouldHandleVariousDateTimeValues(int year, int month, int day, int hour, int minute, int second)
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var dateTime = new DateTime(year, month, day, hour, minute, second);

            // Act
            trackerInfo.CalledTime = dateTime;

            // Assert
            trackerInfo.CalledTime.Should().Be(dateTime);
            trackerInfo.CalledTime.Year.Should().Be(year);
            trackerInfo.CalledTime.Month.Should().Be(month);
            trackerInfo.CalledTime.Day.Should().Be(day);
            trackerInfo.CalledTime.Hour.Should().Be(hour);
            trackerInfo.CalledTime.Minute.Should().Be(minute);
            trackerInfo.CalledTime.Second.Should().Be(second);
        }

        [Fact]
        public void CalledTime_ShouldHandleMinMaxValues()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();

            // Act & Assert - Min value
            trackerInfo.CalledTime = DateTime.MinValue;
            trackerInfo.CalledTime.Should().Be(DateTime.MinValue);

            // Act & Assert - Max value
            trackerInfo.CalledTime = DateTime.MaxValue;
            trackerInfo.CalledTime.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public void CalledTime_ShouldHandleUtcNow()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var utcNow = DateTime.UtcNow;

            // Act
            trackerInfo.CalledTime = utcNow;

            // Assert
            trackerInfo.CalledTime.Should().Be(utcNow);
            trackerInfo.CalledTime.Kind.Should().Be(utcNow.Kind);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("simple-key")]
        [InlineData("key_with_underscores")]
        [InlineData("key-with-dashes")]
        [InlineData("key.with.dots")]
        [InlineData("123456789")]
        [InlineData("UPPERCASE_KEY")]
        [InlineData("mixedCase_Key")]
        public void KeyValue_ShouldHandleVariousStringFormats(string keyValue)
        {
            // Arrange
            var trackerInfo = new TrackerInfo();

            // Act
            trackerInfo.KeyValue = keyValue;

            // Assert
            trackerInfo.KeyValue.Should().Be(keyValue);
        }

        [Fact]
        public void KeyValue_ShouldHandleNullValue()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();

            // Act
            trackerInfo.KeyValue = null;

            // Assert
            trackerInfo.KeyValue.Should().BeNull();
        }

        [Fact]
        public void KeyValue_ShouldHandleUnicodeCharacters()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            const string unicodeKey = "追踪器键值_José_García_🔑";

            // Act
            trackerInfo.KeyValue = unicodeKey;

            // Assert
            trackerInfo.KeyValue.Should().Be(unicodeKey);
        }

        [Fact]
        public void KeyValue_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            const string specialCharsKey = "key!@#$%^&*()+=[]{}|;:,.<>?/~`";

            // Act
            trackerInfo.KeyValue = specialCharsKey;

            // Assert
            trackerInfo.KeyValue.Should().Be(specialCharsKey);
        }

        [Fact]
        public void KeyValue_ShouldHandleLargeString()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var largeKey = new string('K', 10000); // 10KB string

            // Act
            trackerInfo.KeyValue = largeKey;

            // Assert
            trackerInfo.KeyValue.Should().Be(largeKey);
            trackerInfo.KeyValue.Length.Should().Be(10000);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsReset_ShouldHandleBooleanValues(bool isReset)
        {
            // Arrange
            var trackerInfo = new TrackerInfo();

            // Act
            trackerInfo.IsReset = isReset;

            // Assert
            trackerInfo.IsReset.Should().Be(isReset);
        }

        [Fact]
        public void TrackerInfo_ShouldSupportCompleteDataScenario()
        {
            // Arrange
            var calledTime = new DateTime(2024, 12, 16, 14, 30, 45);
            const string keyValue = "tracker-key-12345";
            const bool isReset = true;

            // Act
            var trackerInfo = new TrackerInfo
            {
                CalledTime = calledTime,
                KeyValue = keyValue,
                IsReset = isReset
            };

            // Assert
            trackerInfo.CalledTime.Should().Be(calledTime);
            trackerInfo.KeyValue.Should().Be(keyValue);
            trackerInfo.IsReset.Should().Be(isReset);
        }

        [Fact]
        public void TrackerInfo_ShouldSupportPartialDataScenario()
        {
            // Arrange & Act
            var trackerInfo = new TrackerInfo
            {
                CalledTime = DateTime.UtcNow
                // KeyValue and IsReset left as default
            };

            // Assert
            trackerInfo.CalledTime.Should().NotBe(default(DateTime));
            trackerInfo.KeyValue.Should().BeNull();
            trackerInfo.IsReset.Should().BeFalse();
        }

        [Fact]
        public void TrackerInfo_ShouldHandleResetScenario()
        {
            // Arrange & Act
            var trackerInfo = new TrackerInfo
            {
                CalledTime = DateTime.UtcNow,
                KeyValue = "reset-key",
                IsReset = true
            };

            // Assert
            trackerInfo.IsReset.Should().BeTrue();
            trackerInfo.KeyValue.Should().Be("reset-key");
        }

        [Fact]
        public void TrackerInfo_ShouldHandleNonResetScenario()
        {
            // Arrange & Act
            var trackerInfo = new TrackerInfo
            {
                CalledTime = DateTime.UtcNow,
                KeyValue = "normal-tracking-key",
                IsReset = false
            };

            // Assert
            trackerInfo.IsReset.Should().BeFalse();
            trackerInfo.KeyValue.Should().Be("normal-tracking-key");
        }

        [Fact]
        public void CalledTime_ShouldPreserveDateTimeKind()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var localTime = DateTime.Now;
            var utcTime = DateTime.UtcNow;
            var unspecifiedTime = new DateTime(2024, 12, 16, 14, 30, 0, DateTimeKind.Unspecified);

            // Act & Assert - Local time
            trackerInfo.CalledTime = localTime;
            trackerInfo.CalledTime.Kind.Should().Be(DateTimeKind.Local);

            // Act & Assert - UTC time
            trackerInfo.CalledTime = utcTime;
            trackerInfo.CalledTime.Kind.Should().Be(DateTimeKind.Utc);

            // Act & Assert - Unspecified time
            trackerInfo.CalledTime = unspecifiedTime;
            trackerInfo.CalledTime.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void CalledTime_ShouldHandleMilliseconds()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            var dateTimeWithMilliseconds = new DateTime(2024, 12, 16, 14, 30, 45, 123);

            // Act
            trackerInfo.CalledTime = dateTimeWithMilliseconds;

            // Assert
            trackerInfo.CalledTime.Should().Be(dateTimeWithMilliseconds);
            trackerInfo.CalledTime.Millisecond.Should().Be(123);
        }

        [Fact]
        public void KeyValue_ShouldHandleJsonLikeString()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            const string jsonLikeKey = "{\"trackerId\":12345,\"timestamp\":\"2024-12-16T14:30:00Z\"}";

            // Act
            trackerInfo.KeyValue = jsonLikeKey;

            // Assert
            trackerInfo.KeyValue.Should().Be(jsonLikeKey);
        }

        [Fact]
        public void KeyValue_ShouldHandleXmlLikeString()
        {
            // Arrange
            var trackerInfo = new TrackerInfo();
            const string xmlLikeKey = "<tracker><id>12345</id><timestamp>2024-12-16T14:30:00Z</timestamp></tracker>";

            // Act
            trackerInfo.KeyValue = xmlLikeKey;

            // Assert
            trackerInfo.KeyValue.Should().Be(xmlLikeKey);
        }
    }
}
