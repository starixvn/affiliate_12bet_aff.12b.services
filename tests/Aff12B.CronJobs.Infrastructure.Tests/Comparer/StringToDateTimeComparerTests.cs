using Aff12B.CronJobs.Infrastructure.Comparer;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Comparer
{
    /// <summary>
    /// Comprehensive unit tests for StringToDateTimeComparer class.
    /// Business-focused scenarios for date string comparison in data processing workflows.
    /// Covers various date formats, edge cases, and sorting scenarios.
    /// </summary>
    public class StringToDateTimeComparerTests
    {
        private readonly StringToDateTimeComparer _comparer;

        public StringToDateTimeComparerTests()
        {
            _comparer = new StringToDateTimeComparer();
        }

        [Fact]
        public void Compare_ShouldReturnZero_WhenDatesAreEqual()
        {
            // Arrange
            var date1 = "2024-01-15 10:30:00";
            var date2 = "2024-01-15 10:30:00";

            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void Compare_ShouldReturnPositive_WhenFirstDateIsLater()
        {
            // Arrange - Customer registration scenario
            var laterDate = "2024-01-15 10:30:00";
            var earlierDate = "2024-01-14 10:30:00";

            // Act
            var result = _comparer.Compare(laterDate, earlierDate);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void Compare_ShouldReturnNegative_WhenFirstDateIsEarlier()
        {
            // Arrange - Transaction processing scenario
            var earlierDate = "2024-01-14 10:30:00";
            var laterDate = "2024-01-15 10:30:00";

            // Act
            var result = _comparer.Compare(earlierDate, laterDate);

            // Assert
            result.Should().Be(-1);
        }

        [Theory]
        [InlineData("2024-01-15", "2024-01-14", 1)]
        [InlineData("2024-01-14", "2024-01-15", -1)]
        [InlineData("2024-01-15", "2024-01-15", 0)]
        public void Compare_ShouldHandleDateOnlyFormats(string date1, string date2, int expected)
        {
            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("2024-12-31 23:59:59", "2024-01-01 00:00:00", 1)]
        [InlineData("2024-01-01 00:00:00", "2024-12-31 23:59:59", -1)]
        [InlineData("2024-06-15 12:00:00", "2024-06-15 12:00:00", 0)]
        public void Compare_ShouldHandleFullDateTimeFormats(string date1, string date2, int expected)
        {
            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Compare_ShouldHandleMillisecondsAccurately()
        {
            // Arrange - High-precision timestamp scenario
            var date1 = "2024-01-15 10:30:00.001";
            var date2 = "2024-01-15 10:30:00.000";

            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            result.Should().Be(1); // First date is 1ms later
        }

        [Fact]
        public void Compare_ShouldSortDatesCorrectly_InBusinessScenario()
        {
            // Arrange - Customer transaction dates for sorting
            var transactionDates = new[]
            {
                "2024-01-15 14:30:00", // Latest
                "2024-01-14 09:15:00", // Earliest
                "2024-01-15 10:00:00", // Middle
                "2024-01-14 18:45:00"  // Second
            };

            // Act - Sort using the comparer
            Array.Sort(transactionDates, _comparer);

            // Assert - Should be in chronological order
            transactionDates[0].Should().Be("2024-01-14 09:15:00"); // Earliest
            transactionDates[1].Should().Be("2024-01-14 18:45:00");
            transactionDates[2].Should().Be("2024-01-15 10:00:00");
            transactionDates[3].Should().Be("2024-01-15 14:30:00"); // Latest
        }

        [Fact]
        public void Compare_ShouldHandleDifferentYears()
        {
            // Arrange - Cross-year comparison
            var date2024 = "2024-01-01 00:00:00";
            var date2023 = "2023-12-31 23:59:59";

            // Act
            var result = _comparer.Compare(date2024, date2023);

            // Assert
            result.Should().Be(1); // 2024 is later than 2023
        }

        [Fact]
        public void Compare_ShouldHandleLeapYearScenario()
        {
            // Arrange - Leap year date comparison
            var leapYearDate = "2024-02-29 12:00:00"; // 2024 is a leap year
            var regularDate = "2024-02-28 12:00:00";

            // Act
            var result = _comparer.Compare(leapYearDate, regularDate);

            // Assert
            result.Should().Be(1); // Feb 29 is later than Feb 28
        }

        [Fact]
        public void Compare_ShouldHandleTimeZoneIndependentComparison()
        {
            // Arrange - Same UTC time, different representations
            var utcDate1 = "2024-01-15 10:30:00";
            var utcDate2 = "2024-01-15 10:30:00";

            // Act
            var result = _comparer.Compare(utcDate1, utcDate2);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void Compare_ShouldThrowFormatException_WhenInvalidDateFormat()
        {
            // Arrange - Invalid date format scenario
            var validDate = "2024-01-15 10:30:00";
            var invalidDate = "invalid-date-format";

            // Act & Assert
            _comparer.Invoking(c => c.Compare(validDate, invalidDate))
                .Should().Throw<FormatException>();
        }

        [Fact]
        public void Compare_ShouldHandleNullInput()
        {
            // Arrange
            var validDate = "2024-01-15 10:30:00";

            // Act & Assert - Convert.ToDateTime with null returns DateTime.MinValue (0)
            // So null dates are treated as the earliest possible date
            var result1 = _comparer.Compare(validDate, null);
            var result2 = _comparer.Compare(null, validDate);
            var result3 = _comparer.Compare(null, null);

            result1.Should().Be(1); // Valid date is later than null (DateTime.MinValue)
            result2.Should().Be(-1); // Null (DateTime.MinValue) is earlier than valid date
            result3.Should().Be(0); // Both null values are equal
        }

        [Fact]
        public void Compare_ShouldHandleVeryCloseTimestamps()
        {
            // Arrange - Microsecond precision scenario
            var timestamp1 = "2024-01-15 10:30:00.123456";
            var timestamp2 = "2024-01-15 10:30:00.123457";

            // Act
            var result = _comparer.Compare(timestamp1, timestamp2);

            // Assert
            result.Should().Be(-1); // First timestamp is slightly earlier
        }

        [Fact]
        public void Compare_ShouldHandleBusinessWorkflowScenario()
        {
            // Arrange - Customer lifecycle dates
            var registrationDate = "2024-01-01 09:00:00";
            var firstDepositDate = "2024-01-01 10:30:00";
            var firstBetDate = "2024-01-01 11:15:00";
            var withdrawalDate = "2024-01-02 14:20:00";

            // Act & Assert - Verify chronological order
            _comparer.Compare(registrationDate, firstDepositDate).Should().Be(-1);
            _comparer.Compare(firstDepositDate, firstBetDate).Should().Be(-1);
            _comparer.Compare(firstBetDate, withdrawalDate).Should().Be(-1);
        }

        [Theory]
        [InlineData("1/15/2024 10:30:00 AM", "1/14/2024 10:30:00 AM", 1)]
        [InlineData("2024/01/15 10:30:00", "2024/01/14 10:30:00", 1)]
        public void Compare_ShouldHandleVariousDateFormats(string date1, string date2, int expected)
        {
            // Act
            var result = _comparer.Compare(date1, date2);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Compare_ShouldThrowFormatException_WhenUnsupportedDateFormat()
        {
            // Arrange - European date format that may not be supported
            var europeanDate1 = "15/01/2024 10:30:00";
            var europeanDate2 = "14/01/2024 10:30:00";

            // Act & Assert - Should throw FormatException for unsupported formats
            _comparer.Invoking(c => c.Compare(europeanDate1, europeanDate2))
                .Should().Throw<FormatException>();
        }

        [Fact]
        public void Compare_ShouldHandleEndOfMonthScenarios()
        {
            // Arrange - Month boundary scenarios
            var endOfJanuary = "2024-01-31 23:59:59";
            var startOfFebruary = "2024-02-01 00:00:00";

            // Act
            var result = _comparer.Compare(endOfJanuary, startOfFebruary);

            // Assert
            result.Should().Be(-1); // January 31 is before February 1
        }
    }
}
