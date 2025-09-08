using Aff12B.CronJobs.Core.Domain.DTOs;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain.DTOs
{
    /// <summary>
    /// Unit tests for CreateTimeTrackerInfo DTO class
    /// Tests property validation, edge cases, and time tracking scenarios
    /// </summary>
    public class CreateTimeTrackerInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(default(DateTime));
            createTimeTrackerInfo.UpdateTime.Should().Be(default(DateTime));
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var expectedCreateTime = new DateTime(2024, 12, 16, 10, 0, 0);
            var expectedUpdateTime = new DateTime(2024, 12, 16, 14, 30, 0);

            // Act
            createTimeTrackerInfo.CreateTime = expectedCreateTime;
            createTimeTrackerInfo.UpdateTime = expectedUpdateTime;

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(expectedCreateTime);
            createTimeTrackerInfo.UpdateTime.Should().Be(expectedUpdateTime);
        }

        [Theory]
        [InlineData(2024, 1, 1, 0, 0, 0)]
        [InlineData(2024, 12, 31, 23, 59, 59)]
        [InlineData(2000, 6, 15, 12, 30, 45)]
        [InlineData(2099, 3, 10, 18, 15, 30)]
        public void CreateTime_ShouldHandleVariousDateTimeValues(int year, int month, int day, int hour, int minute, int second)
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var dateTime = new DateTime(year, month, day, hour, minute, second);

            // Act
            createTimeTrackerInfo.CreateTime = dateTime;

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(dateTime);
            createTimeTrackerInfo.CreateTime.Year.Should().Be(year);
            createTimeTrackerInfo.CreateTime.Month.Should().Be(month);
            createTimeTrackerInfo.CreateTime.Day.Should().Be(day);
            createTimeTrackerInfo.CreateTime.Hour.Should().Be(hour);
            createTimeTrackerInfo.CreateTime.Minute.Should().Be(minute);
            createTimeTrackerInfo.CreateTime.Second.Should().Be(second);
        }

        [Theory]
        [InlineData(2024, 1, 1, 0, 0, 0)]
        [InlineData(2024, 12, 31, 23, 59, 59)]
        [InlineData(2000, 6, 15, 12, 30, 45)]
        [InlineData(2099, 3, 10, 18, 15, 30)]
        public void UpdateTime_ShouldHandleVariousDateTimeValues(int year, int month, int day, int hour, int minute, int second)
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var dateTime = new DateTime(year, month, day, hour, minute, second);

            // Act
            createTimeTrackerInfo.UpdateTime = dateTime;

            // Assert
            createTimeTrackerInfo.UpdateTime.Should().Be(dateTime);
            createTimeTrackerInfo.UpdateTime.Year.Should().Be(year);
            createTimeTrackerInfo.UpdateTime.Month.Should().Be(month);
            createTimeTrackerInfo.UpdateTime.Day.Should().Be(day);
            createTimeTrackerInfo.UpdateTime.Hour.Should().Be(hour);
            createTimeTrackerInfo.UpdateTime.Minute.Should().Be(minute);
            createTimeTrackerInfo.UpdateTime.Second.Should().Be(second);
        }

        [Fact]
        public void CreateTime_ShouldHandleMinMaxValues()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();

            // Act & Assert - Min value
            createTimeTrackerInfo.CreateTime = DateTime.MinValue;
            createTimeTrackerInfo.CreateTime.Should().Be(DateTime.MinValue);

            // Act & Assert - Max value
            createTimeTrackerInfo.CreateTime = DateTime.MaxValue;
            createTimeTrackerInfo.CreateTime.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public void UpdateTime_ShouldHandleMinMaxValues()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();

            // Act & Assert - Min value
            createTimeTrackerInfo.UpdateTime = DateTime.MinValue;
            createTimeTrackerInfo.UpdateTime.Should().Be(DateTime.MinValue);

            // Act & Assert - Max value
            createTimeTrackerInfo.UpdateTime = DateTime.MaxValue;
            createTimeTrackerInfo.UpdateTime.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public void CreateTime_ShouldHandleUtcNow()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var utcNow = DateTime.UtcNow;

            // Act
            createTimeTrackerInfo.CreateTime = utcNow;

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(utcNow);
            createTimeTrackerInfo.CreateTime.Kind.Should().Be(utcNow.Kind);
        }

        [Fact]
        public void UpdateTime_ShouldHandleUtcNow()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var utcNow = DateTime.UtcNow;

            // Act
            createTimeTrackerInfo.UpdateTime = utcNow;

            // Assert
            createTimeTrackerInfo.UpdateTime.Should().Be(utcNow);
            createTimeTrackerInfo.UpdateTime.Kind.Should().Be(utcNow.Kind);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldSupportCompleteDataScenario()
        {
            // Arrange
            var createTime = new DateTime(2024, 12, 16, 10, 0, 0);
            var updateTime = new DateTime(2024, 12, 16, 14, 30, 0);

            // Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = createTime,
                UpdateTime = updateTime
            };

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(createTime);
            createTimeTrackerInfo.UpdateTime.Should().Be(updateTime);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandleChronologicalOrder()
        {
            // Arrange
            var createTime = new DateTime(2024, 12, 16, 10, 0, 0);
            var updateTime = new DateTime(2024, 12, 16, 14, 30, 0);

            // Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = createTime,
                UpdateTime = updateTime
            };

            // Assert
            createTimeTrackerInfo.UpdateTime.Should().BeAfter(createTimeTrackerInfo.CreateTime);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldAllowUpdateTimeBeforeCreateTime()
        {
            // Arrange
            var createTime = new DateTime(2024, 12, 16, 14, 30, 0);
            var updateTime = new DateTime(2024, 12, 16, 10, 0, 0);

            // Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = createTime,
                UpdateTime = updateTime
            };

            // Assert - The DTO doesn't enforce chronological order
            createTimeTrackerInfo.CreateTime.Should().Be(createTime);
            createTimeTrackerInfo.UpdateTime.Should().Be(updateTime);
            createTimeTrackerInfo.UpdateTime.Should().BeBefore(createTimeTrackerInfo.CreateTime);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandleSameTimes()
        {
            // Arrange
            var sameTime = new DateTime(2024, 12, 16, 12, 0, 0);

            // Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = sameTime,
                UpdateTime = sameTime
            };

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(sameTime);
            createTimeTrackerInfo.UpdateTime.Should().Be(sameTime);
            createTimeTrackerInfo.CreateTime.Should().Be(createTimeTrackerInfo.UpdateTime);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldPreserveDateTimeKind()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var localTime = DateTime.Now;
            var utcTime = DateTime.UtcNow;
            var unspecifiedTime = new DateTime(2024, 12, 16, 12, 0, 0, DateTimeKind.Unspecified);

            // Act & Assert - Create Time with different kinds
            createTimeTrackerInfo.CreateTime = localTime;
            createTimeTrackerInfo.CreateTime.Kind.Should().Be(DateTimeKind.Local);

            createTimeTrackerInfo.CreateTime = utcTime;
            createTimeTrackerInfo.CreateTime.Kind.Should().Be(DateTimeKind.Utc);

            createTimeTrackerInfo.CreateTime = unspecifiedTime;
            createTimeTrackerInfo.CreateTime.Kind.Should().Be(DateTimeKind.Unspecified);

            // Act & Assert - Update Time with different kinds
            createTimeTrackerInfo.UpdateTime = localTime;
            createTimeTrackerInfo.UpdateTime.Kind.Should().Be(DateTimeKind.Local);

            createTimeTrackerInfo.UpdateTime = utcTime;
            createTimeTrackerInfo.UpdateTime.Kind.Should().Be(DateTimeKind.Utc);

            createTimeTrackerInfo.UpdateTime = unspecifiedTime;
            createTimeTrackerInfo.UpdateTime.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandleMilliseconds()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var createTimeWithMilliseconds = new DateTime(2024, 12, 16, 10, 0, 0, 123);
            var updateTimeWithMilliseconds = new DateTime(2024, 12, 16, 14, 30, 45, 456);

            // Act
            createTimeTrackerInfo.CreateTime = createTimeWithMilliseconds;
            createTimeTrackerInfo.UpdateTime = updateTimeWithMilliseconds;

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(createTimeWithMilliseconds);
            createTimeTrackerInfo.CreateTime.Millisecond.Should().Be(123);
            createTimeTrackerInfo.UpdateTime.Should().Be(updateTimeWithMilliseconds);
            createTimeTrackerInfo.UpdateTime.Millisecond.Should().Be(456);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandlePartialDataScenario()
        {
            // Arrange & Act
            var createTimeTrackerInfo = new CreateTimeTrackerInfo
            {
                CreateTime = DateTime.UtcNow
                // UpdateTime left as default
            };

            // Assert
            createTimeTrackerInfo.CreateTime.Should().NotBe(default(DateTime));
            createTimeTrackerInfo.UpdateTime.Should().Be(default(DateTime));
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandleTimezoneScenario()
        {
            // Arrange
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();
            var utcCreateTime = new DateTime(2024, 12, 16, 10, 0, 0, DateTimeKind.Utc);
            var localUpdateTime = new DateTime(2024, 12, 16, 14, 30, 0, DateTimeKind.Local);

            // Act
            createTimeTrackerInfo.CreateTime = utcCreateTime;
            createTimeTrackerInfo.UpdateTime = localUpdateTime;

            // Assert
            createTimeTrackerInfo.CreateTime.Kind.Should().Be(DateTimeKind.Utc);
            createTimeTrackerInfo.UpdateTime.Kind.Should().Be(DateTimeKind.Local);
        }

        [Fact]
        public void CreateTimeTrackerInfo_ShouldHandleAuditScenario()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var createTimeTrackerInfo = new CreateTimeTrackerInfo();

            // Act - Simulate audit trail creation
            createTimeTrackerInfo.CreateTime = now;

            // Simulate later update
            var laterTime = now.AddHours(4);
            createTimeTrackerInfo.UpdateTime = laterTime;

            // Assert
            createTimeTrackerInfo.CreateTime.Should().Be(now);
            createTimeTrackerInfo.UpdateTime.Should().Be(laterTime);
            createTimeTrackerInfo.UpdateTime.Should().BeAfter(createTimeTrackerInfo.CreateTime);
            (createTimeTrackerInfo.UpdateTime - createTimeTrackerInfo.CreateTime).Should().Be(TimeSpan.FromHours(4));
        }
    }
}
