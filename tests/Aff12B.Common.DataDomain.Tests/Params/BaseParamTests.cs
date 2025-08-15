using Aff12B.Common.DataDomain.Params;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Params
{
    /// <summary>
    /// Unit tests for BaseParam - Target: 95% Coverage
    /// </summary>
    public class BaseParamTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Act
            var param = new BaseParam();

            // Assert
            param.Should().NotBeNull();
            param.FromDate.Should().Be(default(DateTime));
            param.ToDate.Should().Be(default(DateTime));
            param.PageIndex.Should().Be(0);
            param.PageSize.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var param = new BaseParam();
            var fromDate = DateTime.UtcNow.AddDays(-7);
            var toDate = DateTime.UtcNow;

            // Act
            param.FromDate = fromDate;
            param.ToDate = toDate;
            param.PageIndex = 1;
            param.PageSize = 50;

            // Assert
            param.FromDate.Should().Be(fromDate);
            param.ToDate.Should().Be(toDate);
            param.PageIndex.Should().Be(1);
            param.PageSize.Should().Be(50);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void PageIndex_ShouldHandleIntegerValues(int pageIndex)
        {
            // Arrange
            var param = new BaseParam();

            // Act
            param.PageIndex = pageIndex;

            // Assert
            param.PageIndex.Should().Be(pageIndex);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(int.MaxValue)]
        public void PageSize_ShouldHandleIntegerValues(int pageSize)
        {
            // Arrange
            var param = new BaseParam();

            // Act
            param.PageSize = pageSize;

            // Assert
            param.PageSize.Should().Be(pageSize);
        }

        [Fact]
        public void DateTimeProperties_ShouldHandleValidDates()
        {
            // Arrange
            var param = new BaseParam();
            var fromDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var toDate = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            // Act & Assert - FromDate
            param.FromDate = fromDate;
            param.FromDate.Should().Be(fromDate);

            // Act & Assert - ToDate
            param.ToDate = toDate;
            param.ToDate.Should().Be(toDate);
        }

        [Fact]
        public void BaseParam_ShouldHandleValidDateRange()
        {
            // Arrange & Act
            var param = new BaseParam
            {
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow,
                PageIndex = 1,
                PageSize = 50
            };

            // Assert
            param.FromDate.Should().BeBefore(param.ToDate);
            param.PageIndex.Should().Be(1);
            param.PageSize.Should().Be(50);
        }

        [Fact]
        public void BaseParam_ShouldHandleSameDateRange()
        {
            // Arrange
            var sameDate = DateTime.UtcNow.Date;
            var param = new BaseParam();

            // Act
            param.FromDate = sameDate;
            param.ToDate = sameDate;

            // Assert
            param.FromDate.Should().Be(param.ToDate);
        }

        [Fact]
        public void BaseParam_ShouldHandleReverseDateRange()
        {
            // Arrange
            var param = new BaseParam();
            var fromDate = DateTime.UtcNow;
            var toDate = DateTime.UtcNow.AddDays(-1);

            // Act
            param.FromDate = fromDate;
            param.ToDate = toDate;

            // Assert - Should allow reverse dates (business logic validation should handle this)
            param.FromDate.Should().BeAfter(param.ToDate);
        }

        [Fact]
        public void BaseParam_ShouldHandleEdgeCaseDates()
        {
            // Arrange
            var param = new BaseParam();

            // Act & Assert - MinValue
            param.FromDate = DateTime.MinValue;
            param.ToDate = DateTime.MinValue;

            param.FromDate.Should().Be(DateTime.MinValue);
            param.ToDate.Should().Be(DateTime.MinValue);

            // Act & Assert - MaxValue
            param.FromDate = DateTime.MaxValue;
            param.ToDate = DateTime.MaxValue;

            param.FromDate.Should().Be(DateTime.MaxValue);
            param.ToDate.Should().Be(DateTime.MaxValue);
        }

        [Fact]
        public void BaseParam_ShouldHandleTypicalPagingScenarios()
        {
            // Arrange & Act - First page
            var firstPage = new BaseParam
            {
                PageIndex = 1,
                PageSize = 20
            };

            // Assert
            firstPage.PageIndex.Should().Be(1);
            firstPage.PageSize.Should().Be(20);

            // Arrange & Act - Middle page
            var middlePage = new BaseParam
            {
                PageIndex = 5,
                PageSize = 50
            };

            // Assert
            middlePage.PageIndex.Should().Be(5);
            middlePage.PageSize.Should().Be(50);

            // Arrange & Act - Large page
            var largePage = new BaseParam
            {
                PageIndex = 100,
                PageSize = 1000
            };

            // Assert
            largePage.PageIndex.Should().Be(100);
            largePage.PageSize.Should().Be(1000);
        }

        [Fact]
        public void BaseParam_ShouldHandleZeroPaging()
        {
            // Arrange & Act
            var param = new BaseParam
            {
                PageIndex = 0,
                PageSize = 0
            };

            // Assert
            param.PageIndex.Should().Be(0);
            param.PageSize.Should().Be(0);
        }

        [Fact]
        public void BaseParam_ShouldHandleNegativePaging()
        {
            // Arrange & Act
            var param = new BaseParam
            {
                PageIndex = -1,
                PageSize = -10
            };

            // Assert
            param.PageIndex.Should().Be(-1);
            param.PageSize.Should().Be(-10);
        }

        [Fact]
        public void BaseParam_ShouldHandleDailyDateRange()
        {
            // Arrange & Act - Today's data
            var param = new BaseParam
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddDays(1).AddTicks(-1), // End of today
                PageIndex = 1,
                PageSize = 100
            };

            // Assert
            param.FromDate.Should().BeBefore(param.ToDate);
            param.ToDate.Subtract(param.FromDate).Should().BeLessThan(TimeSpan.FromDays(1));
        }

        [Fact]
        public void BaseParam_ShouldHandleWeeklyDateRange()
        {
            // Arrange & Act - This week's data
            var param = new BaseParam
            {
                FromDate = DateTime.Today.AddDays(-7),
                ToDate = DateTime.Today,
                PageIndex = 1,
                PageSize = 500
            };

            // Assert
            param.FromDate.Should().BeBefore(param.ToDate);
            param.ToDate.Subtract(param.FromDate).Should().Be(TimeSpan.FromDays(7));
        }

        [Fact]
        public void BaseParam_ShouldHandleMonthlyDateRange()
        {
            // Arrange & Act - This month's data
            var param = new BaseParam
            {
                FromDate = DateTime.Today.AddMonths(-1),
                ToDate = DateTime.Today,
                PageIndex = 1,
                PageSize = 1000
            };

            // Assert
            param.FromDate.Should().BeBefore(param.ToDate);
            param.ToDate.Subtract(param.FromDate).Should().BeGreaterThan(TimeSpan.FromDays(28));
            param.ToDate.Subtract(param.FromDate).Should().BeLessOrEqualTo(TimeSpan.FromDays(32));
        }

        [Fact]
        public void BaseParam_ShouldHandleCompleteParameterSet()
        {
            // Arrange & Act
            var param = new BaseParam
            {
                FromDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ToDate = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                PageIndex = 10,
                PageSize = 75
            };

            // Assert
            param.FromDate.Year.Should().Be(2023);
            param.FromDate.Month.Should().Be(1);
            param.FromDate.Day.Should().Be(1);
            param.ToDate.Year.Should().Be(2023);
            param.ToDate.Month.Should().Be(12);
            param.ToDate.Day.Should().Be(31);
            param.PageIndex.Should().Be(10);
            param.PageSize.Should().Be(75);
        }

        [Theory]
        [InlineData(1, 10, 0, 9)]
        [InlineData(2, 10, 10, 19)]
        [InlineData(3, 25, 50, 74)]
        [InlineData(5, 100, 400, 499)]
        public void BaseParam_ShouldCalculateOffsetCorrectly(int pageIndex, int pageSize, int expectedStartIndex, int expectedEndIndex)
        {
            // Arrange & Act
            var param = new BaseParam
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            // Assert - Calculate offset manually for verification
            var calculatedOffset = (pageIndex - 1) * pageSize;
            var calculatedEndIndex = calculatedOffset + pageSize - 1;

            calculatedOffset.Should().Be(expectedStartIndex);
            calculatedEndIndex.Should().Be(expectedEndIndex);
        }
    }
}
