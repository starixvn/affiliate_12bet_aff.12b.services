using Aff12B.Common.DataDomain.Models;
using FluentAssertions;
using Xunit;

namespace Aff12B.Common.DataDomain.Tests.Models
{
    /// <summary>
    /// Unit tests for IHasUpdateTime interface contract
    /// Tests interface contract validation, implementation requirements, and usage scenarios
    /// </summary>
    public class IHasUpdateTimeTests
    {
        [Fact]
        public void IHasUpdateTime_ShouldHaveCorrectInterface()
        {
            // Arrange
            var interfaceType = typeof(IHasUpdateTime);

            // Act & Assert
            interfaceType.IsInterface.Should().BeTrue();
            interfaceType.IsPublic.Should().BeTrue();
            interfaceType.IsAbstract.Should().BeTrue(); // Interfaces are abstract by definition
        }

        [Fact]
        public void IHasUpdateTime_ShouldHaveUpdateTimeProperty()
        {
            // Arrange
            var interfaceType = typeof(IHasUpdateTime);

            // Act
            var property = interfaceType.GetProperty("UpdateTime");

            // Assert
            property.Should().NotBeNull();
            property!.Name.Should().Be("UpdateTime");
            property.PropertyType.Should().Be<DateTime?>();
            property.CanRead.Should().BeTrue();
            property.CanWrite.Should().BeFalse(); // Getter only
        }

        [Fact]
        public void IHasUpdateTime_ShouldHaveOnlyOneProperty()
        {
            // Arrange
            var interfaceType = typeof(IHasUpdateTime);

            // Act
            var properties = interfaceType.GetProperties();

            // Assert
            properties.Should().HaveCount(1);
            properties[0].Name.Should().Be("UpdateTime");
        }

        [Fact]
        public void IHasUpdateTime_ShouldHaveCorrectNamespace()
        {
            // Arrange
            var interfaceType = typeof(IHasUpdateTime);

            // Act & Assert
            interfaceType.Namespace.Should().Be("Aff12B.Common.DataDomain.Models");
        }

        [Fact]
        public void IHasUpdateTime_ShouldBeImplementableByTestDouble()
        {
            // Arrange & Act
            var testImplementation = new TestHasUpdateTimeImplementation();

            // Assert
            testImplementation.Should().BeAssignableTo<IHasUpdateTime>();
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportPolymorphism()
        {
            // Arrange
            IHasUpdateTime hasUpdateTime1 = new TestHasUpdateTimeImplementation();
            IHasUpdateTime hasUpdateTime2 = new AlternativeHasUpdateTimeImplementation();

            // Act
            var updateTime1 = hasUpdateTime1.UpdateTime;
            var updateTime2 = hasUpdateTime2.UpdateTime;

            // Assert
            hasUpdateTime1.Should().BeAssignableTo<IHasUpdateTime>();
            hasUpdateTime2.Should().BeAssignableTo<IHasUpdateTime>();

            updateTime1.Should().NotBeNull(); // TestHasUpdateTimeImplementation returns current time
            updateTime2.Should().BeNull(); // AlternativeHasUpdateTimeImplementation returns null
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportCollectionOperations()
        {
            // Arrange
            var hasUpdateTimeObjects = new List<IHasUpdateTime>
            {
                new TestHasUpdateTimeImplementation(),
                new AlternativeHasUpdateTimeImplementation()
            };

            // Act
            var updateTimes = hasUpdateTimeObjects.Select(obj => obj.UpdateTime).ToList();
            var nonNullUpdateTimes = hasUpdateTimeObjects.Where(obj => obj.UpdateTime.HasValue).ToList();

            // Assert
            updateTimes.Should().HaveCount(2);
            nonNullUpdateTimes.Should().HaveCount(1); // Only TestHasUpdateTimeImplementation has non-null UpdateTime
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportLinqOperations()
        {
            // Arrange
            var hasUpdateTimeObjects = new List<IHasUpdateTime>
            {
                new TestHasUpdateTimeImplementation(),
                new AlternativeHasUpdateTimeImplementation(),
                new TestHasUpdateTimeImplementation()
            };

            // Act
            var recentlyUpdated = hasUpdateTimeObjects
                .Where(obj => obj.UpdateTime.HasValue && obj.UpdateTime > DateTime.UtcNow.AddHours(-1))
                .ToList();

            var oldestUpdate = hasUpdateTimeObjects
                .Where(obj => obj.UpdateTime.HasValue)
                .Min(obj => obj.UpdateTime);

            // Assert
            recentlyUpdated.Should().HaveCount(2); // Two TestHasUpdateTimeImplementation instances
            oldestUpdate.Should().NotBeNull();
        }

        [Fact]
        public void IHasUpdateTime_UpdateTimeProperty_ShouldReturnNullableDateTime()
        {
            // Arrange
            var interfaceType = typeof(IHasUpdateTime);

            // Act
            var property = interfaceType.GetProperty("UpdateTime");

            // Assert
            property.Should().NotBeNull();
            property!.PropertyType.Should().Be<DateTime?>();
            property.PropertyType.IsGenericType.Should().BeTrue();
            property.PropertyType.GetGenericTypeDefinition().Should().Be(typeof(Nullable<>));
            property.PropertyType.GetGenericArguments()[0].Should().Be<DateTime>();
        }

        [Fact]
        public void IHasUpdateTime_ShouldBeImplementedByWithdrawalInfoModel()
        {
            // Arrange & Act
            var withdrawal = new WithdrawalInfoModel();

            // Assert
            withdrawal.Should().BeAssignableTo<IHasUpdateTime>();
        }

        [Fact]
        public void IHasUpdateTime_WithdrawalInfoModel_ShouldProvideUpdateTimeAccess()
        {
            // Arrange
            var withdrawal = new WithdrawalInfoModel();
            var updateTime = DateTime.UtcNow;
            withdrawal.UpdateTime = updateTime;

            // Act
            IHasUpdateTime hasUpdateTime = withdrawal;

            // Assert
            hasUpdateTime.UpdateTime.Should().Be(updateTime);
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportGenericConstraintScenario()
        {
            // Arrange
            var withdrawal = new WithdrawalInfoModel { UpdateTime = DateTime.UtcNow };
            var testImplementation = new TestHasUpdateTimeImplementation();

            // Act
            var withdrawalUpdateTime = GetUpdateTimeGeneric(withdrawal);
            var testUpdateTime = GetUpdateTimeGeneric(testImplementation);

            // Assert
            withdrawalUpdateTime.Should().NotBeNull();
            testUpdateTime.Should().NotBeNull();
        }

        [Fact]
        public void IHasUpdateTime_ShouldHandleNullUpdateTimeScenario()
        {
            // Arrange & Act
            var hasUpdateTime = new AlternativeHasUpdateTimeImplementation();

            // Assert
            hasUpdateTime.UpdateTime.Should().BeNull();
        }

        [Fact]
        public void IHasUpdateTime_ShouldHandleValidUpdateTimeScenario()
        {
            // Arrange & Act
            var hasUpdateTime = new TestHasUpdateTimeImplementation();

            // Assert
            hasUpdateTime.UpdateTime.Should().NotBeNull();
            hasUpdateTime.UpdateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportTimeComparisonScenario()
        {
            // Arrange
            var obj1 = new TimestampedHasUpdateTimeImplementation(DateTime.UtcNow.AddHours(-2));
            var obj2 = new TimestampedHasUpdateTimeImplementation(DateTime.UtcNow.AddHours(-1));

            // Act
            var isObj2MoreRecent = obj2.UpdateTime > obj1.UpdateTime;

            // Assert
            obj1.UpdateTime.Should().BeBefore(obj2.UpdateTime.Value);
            isObj2MoreRecent.Should().BeTrue();
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportFilteringByUpdateTime()
        {
            // Arrange
            var cutoffTime = DateTime.UtcNow.AddHours(-1);
            var objects = new List<IHasUpdateTime>
            {
                new TimestampedHasUpdateTimeImplementation(DateTime.UtcNow.AddHours(-2)), // Old
                new TimestampedHasUpdateTimeImplementation(DateTime.UtcNow.AddMinutes(-30)), // Recent
                new AlternativeHasUpdateTimeImplementation(), // Null
                new TimestampedHasUpdateTimeImplementation(DateTime.UtcNow.AddMinutes(-10)) // Recent
            };

            // Act
            var recentlyUpdated = objects
                .Where(obj => obj.UpdateTime.HasValue && obj.UpdateTime > cutoffTime)
                .ToList();

            // Assert
            recentlyUpdated.Should().HaveCount(2);
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportOrderingByUpdateTime()
        {
            // Arrange
            var objects = new List<IHasUpdateTime>
            {
                new TimestampedHasUpdateTimeImplementation(new DateTime(2024, 12, 16, 14, 0, 0)),
                new TimestampedHasUpdateTimeImplementation(new DateTime(2024, 12, 16, 10, 0, 0)),
                new TimestampedHasUpdateTimeImplementation(new DateTime(2024, 12, 16, 18, 0, 0))
            };

            // Act
            var orderedByUpdateTime = objects
                .Where(obj => obj.UpdateTime.HasValue)
                .OrderBy(obj => obj.UpdateTime)
                .ToList();

            // Assert
            orderedByUpdateTime.Should().HaveCount(3);
            orderedByUpdateTime[0].UpdateTime.Should().Be(new DateTime(2024, 12, 16, 10, 0, 0));
            orderedByUpdateTime[1].UpdateTime.Should().Be(new DateTime(2024, 12, 16, 14, 0, 0));
            orderedByUpdateTime[2].UpdateTime.Should().Be(new DateTime(2024, 12, 16, 18, 0, 0));
        }

        [Fact]
        public void IHasUpdateTime_ShouldSupportGroupingByUpdateTimeRange()
        {
            // Arrange
            var baseTime = new DateTime(2024, 12, 16, 12, 0, 0);
            var objects = new List<IHasUpdateTime>
            {
                new TimestampedHasUpdateTimeImplementation(baseTime.AddHours(-3)), // Morning
                new TimestampedHasUpdateTimeImplementation(baseTime.AddHours(-1)), // Midday
                new TimestampedHasUpdateTimeImplementation(baseTime.AddHours(2)),  // Afternoon
                new TimestampedHasUpdateTimeImplementation(baseTime.AddHours(8))   // Evening
            };

            // Act
            var groupedByTimeRange = objects
                .Where(obj => obj.UpdateTime.HasValue)
                .GroupBy(obj => obj.UpdateTime!.Value.Hour < 12 ? "Morning" :
                               obj.UpdateTime!.Value.Hour < 18 ? "Afternoon" : "Evening")
                .ToList();

            // Assert
            groupedByTimeRange.Should().HaveCount(3);
            groupedByTimeRange.Should().Contain(g => g.Key == "Morning");
            groupedByTimeRange.Should().Contain(g => g.Key == "Afternoon");
            groupedByTimeRange.Should().Contain(g => g.Key == "Evening");
        }

        /// <summary>
        /// Generic method to demonstrate IHasUpdateTime usage with generic constraints
        /// </summary>
        private static DateTime? GetUpdateTimeGeneric<T>(T obj) where T : IHasUpdateTime
        {
            return obj.UpdateTime;
        }

        // Test implementation classes for validation
        private class TestHasUpdateTimeImplementation : IHasUpdateTime
        {
            public DateTime? UpdateTime => DateTime.UtcNow;
        }

        private class AlternativeHasUpdateTimeImplementation : IHasUpdateTime
        {
            public DateTime? UpdateTime => null;
        }

        private class TimestampedHasUpdateTimeImplementation : IHasUpdateTime
        {
            public TimestampedHasUpdateTimeImplementation(DateTime updateTime)
            {
                UpdateTime = updateTime;
            }

            public DateTime? UpdateTime { get; }
        }
    }
}
