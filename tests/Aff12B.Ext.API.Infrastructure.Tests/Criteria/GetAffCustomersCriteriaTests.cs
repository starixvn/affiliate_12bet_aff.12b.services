using Aff12B.Ext.API.Infastructure.Criteria;
using FluentAssertions;
using Xunit;

namespace Aff12B.Ext.API.Infrastructure.Tests.Criteria
{
    public class GetAffCustomersCriteriaTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ValidParameters_ShouldSetProperties()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            const int pageIndex = 1;
            const int pageSize = 20;

            // Act
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize);

            // Assert
            criteria.FromDate.Should().Be(fromDate);
            criteria.ToDate.Should().Be(toDate);
            criteria.PageIndex.Should().Be(pageIndex);
            criteria.PageSize.Should().Be(pageSize);
        }

        [Theory]
        [InlineData("2024-01-01", "2024-01-31", 0, 10)]
        [InlineData("2023-12-01", "2023-12-31", 1, 20)]
        [InlineData("2024-06-15", "2024-06-30", 5, 50)]
        [InlineData("2020-01-01", "2020-12-31", 10, 100)]
        public void Constructor_VariousDateRanges_ShouldHandleCorrectly(string fromDateStr, string toDateStr, int pageIndex, int pageSize)
        {
            // Arrange
            var fromDate = DateTime.Parse(fromDateStr);
            var toDate = DateTime.Parse(toDateStr);

            // Act
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize);

            // Assert
            criteria.FromDate.Should().Be(fromDate);
            criteria.ToDate.Should().Be(toDate);
            criteria.PageIndex.Should().Be(pageIndex);
            criteria.PageSize.Should().Be(pageSize);
            criteria.FromDate.Should().BeBefore(criteria.ToDate);
        }

        [Fact]
        public void Constructor_SameDateRange_ShouldWork()
        {
            // Arrange
            var sameDate = new DateTime(2024, 1, 15);

            // Act
            var criteria = new GetAffCustomersCriteria(sameDate, sameDate, 1, 10);

            // Assert
            criteria.FromDate.Should().Be(sameDate);
            criteria.ToDate.Should().Be(sameDate);
        }

        [Fact]
        public void Constructor_ReverseDateRange_ShouldAcceptValues()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 31);
            var toDate = new DateTime(2024, 1, 1);

            // Act
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, 1, 10);

            // Assert
            criteria.FromDate.Should().Be(fromDate);
            criteria.ToDate.Should().Be(toDate);
            criteria.FromDate.Should().BeAfter(criteria.ToDate);
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(0, 0)]
        [InlineData(1, -5)]
        [InlineData(int.MaxValue, int.MinValue)]
        public void Constructor_EdgeCasePageValues_ShouldAcceptValues(int pageIndex, int pageSize)
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);

            // Act
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize);

            // Assert
            criteria.PageIndex.Should().Be(pageIndex);
            criteria.PageSize.Should().Be(pageSize);
        }

        #endregion

        #region Method Tests

        [Fact]
        public void GetSettingKey_ShouldReturnCorrectValue()
        {
            // Arrange
            var criteria = new GetAffCustomersCriteria(DateTime.Now, DateTime.Now, 1, 10);

            // Act
            var settingKey = criteria.GetSettingKey();

            // Assert
            settingKey.Should().Be("Aff_CustomerSel");
        }

        [Fact]
        public void IsValid_ShouldAlwaysReturnTrue()
        {
            // Arrange
            var criteria = new GetAffCustomersCriteria(DateTime.Now, DateTime.Now, 1, 10);

            // Act
            var isValid = criteria.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("2020-01-01", "2020-01-01", -1, -1)]
        [InlineData("2024-12-31", "2024-01-01", 0, 0)]
        [InlineData("1900-01-01", "2100-12-31", int.MaxValue, int.MaxValue)]
        public void IsValid_WithVariousParameters_ShouldAlwaysReturnTrue(string fromDateStr, string toDateStr, int pageIndex, int pageSize)
        {
            // Arrange
            var fromDate = DateTime.Parse(fromDateStr);
            var toDate = DateTime.Parse(toDateStr);
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize);

            // Act
            var isValid = criteria.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void GetAffCustomersCriteria_ShouldInheritFromPagingCriteria()
        {
            // Arrange
            var criteria = new GetAffCustomersCriteria(DateTime.Now, DateTime.Now, 1, 10);

            // Act & Assert
            criteria.Should().BeAssignableTo<PagingCriteria>();
        }

        [Fact]
        public void GetAffCustomersCriteria_ShouldHaveInheritedProperties()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, 1, 10);

            // Act & Assert
            // These properties come from PagingCriteria base class
            criteria.FromDate.Should().Be(fromDate);
            criteria.ToDate.Should().Be(toDate);
            criteria.PageIndex.Should().Be(1);
            criteria.PageSize.Should().Be(10);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void GetAffCustomersCriteria_RealWorldScenario_ShouldWork()
        {
            // Arrange
            var startOfMonth = new DateTime(2024, 1, 1);
            var endOfMonth = new DateTime(2024, 1, 31, 23, 59, 59);

            // Act
            var criteria = new GetAffCustomersCriteria(startOfMonth, endOfMonth, 1, 50);

            // Assert
            criteria.FromDate.Should().Be(startOfMonth);
            criteria.ToDate.Should().Be(endOfMonth);
            criteria.PageIndex.Should().Be(1);
            criteria.PageSize.Should().Be(50);
            criteria.GetSettingKey().Should().Be("Aff_CustomerSel");
            criteria.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GetAffCustomersCriteria_LargePagingValues_ShouldWork()
        {
            // Arrange
            var fromDate = new DateTime(2020, 1, 1);
            var toDate = new DateTime(2024, 12, 31);
            const int pageIndex = 1000;
            const int pageSize = 10000;

            // Act
            var criteria = new GetAffCustomersCriteria(fromDate, toDate, pageIndex, pageSize);

            // Assert
            criteria.FromDate.Should().Be(fromDate);
            criteria.ToDate.Should().Be(toDate);
            criteria.PageIndex.Should().Be(pageIndex);
            criteria.PageSize.Should().Be(pageSize);
            criteria.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GetAffCustomersCriteria_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange
            var criteria1 = new GetAffCustomersCriteria(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), 1, 10);
            var criteria2 = new GetAffCustomersCriteria(new DateTime(2024, 2, 1), new DateTime(2024, 2, 28), 2, 20);

            // Act
            criteria1.PageIndex = 5;

            // Assert
            criteria1.PageIndex.Should().Be(5);
            criteria1.PageSize.Should().Be(10);
            criteria2.PageIndex.Should().Be(2);
            criteria2.PageSize.Should().Be(20);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GetAffCustomersCriteria_MinMaxDateTimeValues_ShouldWork()
        {
            // Arrange & Act
            var criteria = new GetAffCustomersCriteria(DateTime.MinValue, DateTime.MaxValue, 0, 1);

            // Assert
            criteria.FromDate.Should().Be(DateTime.MinValue);
            criteria.ToDate.Should().Be(DateTime.MaxValue);
            criteria.PageIndex.Should().Be(0);
            criteria.PageSize.Should().Be(1);
            criteria.IsValid().Should().BeTrue();
        }

        [Fact]
        public void GetAffCustomersCriteria_PropertyModification_ShouldWork()
        {
            // Arrange
            var criteria = new GetAffCustomersCriteria(new DateTime(2024, 1, 1), new DateTime(2024, 1, 31), 1, 10);

            // Act
            criteria.PageIndex = 3;
            criteria.PageSize = 25;

            // Assert
            criteria.PageIndex.Should().Be(3);
            criteria.PageSize.Should().Be(25);
            criteria.FromDate.Should().Be(new DateTime(2024, 1, 1));
            criteria.ToDate.Should().Be(new DateTime(2024, 1, 31));
        }

        [Fact]
        public void GetAffCustomersCriteria_MethodCallsMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var criteria = new GetAffCustomersCriteria(DateTime.Now, DateTime.Now, 1, 10);

            // Act
            var settingKey1 = criteria.GetSettingKey();
            var settingKey2 = criteria.GetSettingKey();
            var isValid1 = criteria.IsValid();
            var isValid2 = criteria.IsValid();

            // Assert
            settingKey1.Should().Be(settingKey2);
            settingKey1.Should().Be("Aff_CustomerSel");
            isValid1.Should().Be(isValid2);
            isValid1.Should().BeTrue();
        }

        #endregion
    }
}
