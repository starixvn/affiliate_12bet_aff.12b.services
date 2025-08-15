using Aff12B.CronJobs.Infrastructure.Helpers;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Helpers
{
    /// <summary>
    /// Unit tests for DateTimeHelper class
    /// Tests time range calculation and utility methods
    /// Target: 100% code coverage
    /// </summary>
    public class DateTimeHelperTests
    {
        [Fact]
        public void GetProportionTimeRange_ZeroTimeSpan_ShouldThrowArgumentException()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1);
            var endTime = new DateTime(2023, 1, 2);
            var zeroSpan = TimeSpan.Zero;

            // Act & Assert
            var act = () => DateTimeHelper.GetProportionTimeRange(startTime, endTime, zeroSpan).ToList();
            act.Should().Throw<ArgumentException>()
                .WithMessage("Zero time span, this make a infinite repeation to get timerange.");
        }

        [Theory]
        [InlineData(1, 24)] // 1 day span over 24 hours
        [InlineData(2, 12)] // 2 day span over 24 hours
        [InlineData(6, 4)]  // 6 hour span over 24 hours
        [InlineData(8, 3)]  // 8 hour span over 24 hours
        public void GetProportionTimeRange_ValidSpan_ShouldReturnCorrectRanges(int spanHours, int expectedRanges)
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 2, 0, 0, 0); // 24 hours later
            var span = TimeSpan.FromHours(spanHours);

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span).ToList();

            // Assert
            result.Should().HaveCount(expectedRanges);

            // Verify first range starts at start time
            result.First().FromDate.Should().Be(startTime);

            // Verify last range ends at or before end time
            result.Last().ToDate.Should().BeOnOrBefore(endTime);

            // Verify each range has the correct span (except possibly the last one)
            for (int i = 0; i < result.Count - 1; i++)
            {
                var range = result[i];
                (range.ToDate - range.FromDate).Should().Be(span);
            }
        }

        [Fact]
        public void GetProportionTimeRange_ExcludeNotFullRange_True_ShouldExcludePartialRanges()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 10, 30, 0); // 10.5 hours
            var span = TimeSpan.FromHours(4); // 4 hour spans
            var isExcludeNotFullRangeSpan = true;

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span, isExcludeNotFullRangeSpan).ToList();

            // Assert
            result.Should().HaveCount(2); // Only 2 full 4-hour ranges fit in 10.5 hours

            // Verify all ranges are full spans
            foreach (var range in result)
            {
                (range.ToDate - range.FromDate).Should().Be(span);
            }
        }

        [Fact]
        public void GetProportionTimeRange_ExcludeNotFullRange_False_ShouldIncludePartialRanges()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 10, 30, 0); // 10.5 hours
            var span = TimeSpan.FromHours(4); // 4 hour spans
            var isExcludeNotFullRangeSpan = false;

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span, isExcludeNotFullRangeSpan).ToList();

            // Assert
            result.Should().HaveCount(3); // 2 full ranges + 1 partial (2.5 hours)

            // Verify first two ranges are full spans
            result[0].ToDate.Subtract(result[0].FromDate).Should().Be(span);
            result[1].ToDate.Subtract(result[1].FromDate).Should().Be(span);

            // Verify last range is partial
            result[2].ToDate.Subtract(result[2].FromDate).Should().Be(TimeSpan.FromHours(2.5));
        }

        [Theory]
        [InlineData("2023-01-01 10:00:00", "2023-01-01 15:00:00", "2023-01-01 10:00:00")] // first is smaller
        [InlineData("2023-01-01 15:00:00", "2023-01-01 10:00:00", "2023-01-01 10:00:00")] // second is smaller
        [InlineData("2023-01-01 12:00:00", "2023-01-01 12:00:00", "2023-01-01 12:00:00")] // equal times
        public void GetSmallerTime_CompareDates_ShouldReturnSmaller(string compareTimeStr, string endTimeStr, string expectedStr)
        {
            // Arrange
            var compareTime = DateTime.Parse(compareTimeStr);
            var endTime = DateTime.Parse(endTimeStr);
            var expected = DateTime.Parse(expectedStr);

            // Act
            var result = DateTimeHelper.GetSmallerTime(compareTime, endTime);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetApiServerDateTime_ShouldReturnUtcDateTime()
        {
            // Arrange
            var beforeCall = DateTimeOffset.UtcNow.DateTime;

            // Act
            var result = DateTimeHelper.GetApiServerDateTime();

            // Assert
            var afterCall = DateTimeOffset.UtcNow.DateTime;

            result.Should().BeOnOrAfter(beforeCall);
            result.Should().BeOnOrBefore(afterCall);
            // DateTimeOffset.UtcNow.DateTime returns DateTimeKind.Unspecified, not Utc
            result.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_StartEqualsEnd_ShouldReturnEmpty()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 12, 0, 0); // Same as start
            var span = TimeSpan.FromHours(1);

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span).ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_SpanLargerThanRange_ShouldReturnSinglePartialRange()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 2, 0, 0); // 2 hours
            var span = TimeSpan.FromHours(5); // Larger than total range
            var isExcludeNotFullRangeSpan = false;

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span, isExcludeNotFullRangeSpan).ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].FromDate.Should().Be(startTime);
            result[0].ToDate.Should().Be(endTime);
            (result[0].ToDate - result[0].FromDate).Should().Be(TimeSpan.FromHours(2));
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_SpanLargerThanRange_ExcludePartial_ShouldReturnEmpty()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 2, 0, 0); // 2 hours
            var span = TimeSpan.FromHours(5); // Larger than total range
            var isExcludeNotFullRangeSpan = true;

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span, isExcludeNotFullRangeSpan).ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_VerySmallSpan_ShouldHandleCorrectly()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 0, 0, 0);
            var endTime = new DateTime(2023, 1, 1, 0, 0, 10); // 10 seconds
            var span = TimeSpan.FromSeconds(1); // 1 second spans

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span).ToList();

            // Assert
            result.Should().HaveCount(10);

            // Verify each range is exactly 1 second
            foreach (var range in result)
            {
                (range.ToDate - range.FromDate).Should().Be(TimeSpan.FromSeconds(1));
            }
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_CrossingMidnight_ShouldHandleCorrectly()
        {
            // Arrange
            var startTime = new DateTime(2023, 1, 1, 23, 0, 0); // 11 PM
            var endTime = new DateTime(2023, 1, 2, 2, 0, 0);   // 2 AM next day (3 hours later)
            var span = TimeSpan.FromHours(1);

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span).ToList();

            // Assert
            result.Should().HaveCount(3);

            // Verify ranges cross midnight correctly
            result[0].FromDate.Should().Be(new DateTime(2023, 1, 1, 23, 0, 0));
            result[0].ToDate.Should().Be(new DateTime(2023, 1, 2, 0, 0, 0));

            result[1].FromDate.Should().Be(new DateTime(2023, 1, 2, 0, 0, 0));
            result[1].ToDate.Should().Be(new DateTime(2023, 1, 2, 1, 0, 0));

            result[2].FromDate.Should().Be(new DateTime(2023, 1, 2, 1, 0, 0));
            result[2].ToDate.Should().Be(new DateTime(2023, 1, 2, 2, 0, 0));
        }

        [Fact]
        public void GetProportionTimeRange_EdgeCase_LeapYear_ShouldHandleCorrectly()
        {
            // Arrange - February 28 to March 1 in leap year 2024
            var startTime = new DateTime(2024, 2, 28, 0, 0, 0);
            var endTime = new DateTime(2024, 3, 1, 0, 0, 0);   // Includes Feb 29
            var span = TimeSpan.FromDays(1);

            // Act
            var result = DateTimeHelper.GetProportionTimeRange(startTime, endTime, span).ToList();

            // Assert
            result.Should().HaveCount(2); // Feb 28-29 and Feb 29-Mar 1

            result[0].FromDate.Should().Be(new DateTime(2024, 2, 28, 0, 0, 0));
            result[0].ToDate.Should().Be(new DateTime(2024, 2, 29, 0, 0, 0));

            result[1].FromDate.Should().Be(new DateTime(2024, 2, 29, 0, 0, 0));
            result[1].ToDate.Should().Be(new DateTime(2024, 3, 1, 0, 0, 0));
        }

        [Fact]
        public void GetSmallerTime_EdgeCase_MaxValue_ShouldReturnOther()
        {
            // Arrange
            var maxTime = DateTime.MaxValue;
            var normalTime = new DateTime(2023, 1, 1);

            // Act
            var result1 = DateTimeHelper.GetSmallerTime(maxTime, normalTime);
            var result2 = DateTimeHelper.GetSmallerTime(normalTime, maxTime);

            // Assert
            result1.Should().Be(normalTime);
            result2.Should().Be(normalTime);
        }

        [Fact]
        public void GetSmallerTime_EdgeCase_MinValue_ShouldReturnMinValue()
        {
            // Arrange
            var minTime = DateTime.MinValue;
            var normalTime = new DateTime(2023, 1, 1);

            // Act
            var result1 = DateTimeHelper.GetSmallerTime(minTime, normalTime);
            var result2 = DateTimeHelper.GetSmallerTime(normalTime, minTime);

            // Assert
            result1.Should().Be(minTime);
            result2.Should().Be(minTime);
        }

        [Fact]
        public async Task GetApiServerDateTime_MultipleCallsInQuickSuccession_ShouldReturnIncreasingTimes()
        {
            // Arrange & Act
            var time1 = DateTimeHelper.GetApiServerDateTime();
            await Task.Delay(1); // Ensure some time passes
            var time2 = DateTimeHelper.GetApiServerDateTime();
            await Task.Delay(1);
            var time3 = DateTimeHelper.GetApiServerDateTime();

            // Assert
            time2.Should().BeOnOrAfter(time1);
            time3.Should().BeOnOrAfter(time2);
        }
    }
}
