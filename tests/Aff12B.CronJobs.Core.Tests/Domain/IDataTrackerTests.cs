using Aff12B.CronJobs.Core.Domain;
using FluentAssertions;
using Xunit;

namespace Aff12B.CronJobs.Core.Tests.Domain
{
    /// <summary>
    /// Unit tests for IDataTracker interface contract
    /// Tests interface contract validation and implementation requirements
    /// </summary>
    public class IDataTrackerTests
    {
        [Fact]
        public void IDataTracker_ShouldHaveCorrectProperties()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act & Assert
            interfaceType.IsInterface.Should().BeTrue();

            var pushDataTypeProperty = interfaceType.GetProperty("PushDataType");
            pushDataTypeProperty.Should().NotBeNull();
            pushDataTypeProperty!.PropertyType.Should().Be<int>();
            pushDataTypeProperty.CanRead.Should().BeTrue();

            var statusIdProperty = interfaceType.GetProperty("StatusId");
            statusIdProperty.Should().NotBeNull();
            statusIdProperty!.PropertyType.Should().Be<int>();
            statusIdProperty.CanRead.Should().BeTrue();

            var pushTrackerIdProperty = interfaceType.GetProperty("PushTrackerId");
            pushTrackerIdProperty.Should().NotBeNull();
            pushTrackerIdProperty!.PropertyType.Should().Be<long>();
            pushTrackerIdProperty.CanRead.Should().BeTrue();
        }

        [Fact]
        public void IDataTracker_ShouldHaveThreeProperties()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act
            var properties = interfaceType.GetProperties();

            // Assert
            properties.Should().HaveCount(3);
            properties.Select(p => p.Name).Should().Contain(new[] { "PushDataType", "StatusId", "PushTrackerId" });
        }

        [Fact]
        public void IDataTracker_Properties_ShouldBeReadOnly()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act
            var properties = interfaceType.GetProperties();

            // Assert
            foreach (var property in properties)
            {
                property.CanRead.Should().BeTrue($"Property {property.Name} should be readable");
                property.GetMethod.Should().NotBeNull($"Property {property.Name} should have a getter");
                property.SetMethod.Should().BeNull($"Property {property.Name} should not have a setter in the interface");
            }
        }

        [Fact]
        public void IDataTracker_PushDataType_ShouldBeIntegerType()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act
            var pushDataTypeProperty = interfaceType.GetProperty("PushDataType");

            // Assert
            pushDataTypeProperty.Should().NotBeNull();
            pushDataTypeProperty!.PropertyType.Should().Be<int>();
        }

        [Fact]
        public void IDataTracker_StatusId_ShouldBeIntegerType()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act
            var statusIdProperty = interfaceType.GetProperty("StatusId");

            // Assert
            statusIdProperty.Should().NotBeNull();
            statusIdProperty!.PropertyType.Should().Be<int>();
        }

        [Fact]
        public void IDataTracker_PushTrackerId_ShouldBeLongType()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act
            var pushTrackerIdProperty = interfaceType.GetProperty("PushTrackerId");

            // Assert
            pushTrackerIdProperty.Should().NotBeNull();
            pushTrackerIdProperty!.PropertyType.Should().Be<long>();
        }

        [Fact]
        public void IDataTracker_ShouldBeImplementableByTestDouble()
        {
            // Arrange & Act
            var testImplementation = new TestDataTrackerImplementation();

            // Assert
            testImplementation.Should().BeAssignableTo<IDataTracker>();
            testImplementation.PushDataType.Should().Be(1);
            testImplementation.StatusId.Should().Be(2);
            testImplementation.PushTrackerId.Should().Be(123456789L);
        }

        [Fact]
        public void IDataTracker_ShouldSupportPolymorphism()
        {
            // Arrange
            IDataTracker dataTracker1 = new TestDataTrackerImplementation();
            IDataTracker dataTracker2 = new AlternativeDataTrackerImplementation();

            // Act & Assert
            dataTracker1.Should().BeAssignableTo<IDataTracker>();
            dataTracker2.Should().BeAssignableTo<IDataTracker>();

            dataTracker1.PushDataType.Should().Be(1);
            dataTracker2.PushDataType.Should().Be(5);

            dataTracker1.StatusId.Should().Be(2);
            dataTracker2.StatusId.Should().Be(10);

            dataTracker1.PushTrackerId.Should().Be(123456789L);
            dataTracker2.PushTrackerId.Should().Be(987654321L);
        }

        [Fact]
        public void IDataTracker_ShouldSupportCollectionOperations()
        {
            // Arrange
            var dataTrackers = new List<IDataTracker>
            {
                new TestDataTrackerImplementation(),
                new AlternativeDataTrackerImplementation()
            };

            // Act
            var pushDataTypes = dataTrackers.Select(dt => dt.PushDataType).ToList();
            var statusIds = dataTrackers.Select(dt => dt.StatusId).ToList();
            var pushTrackerIds = dataTrackers.Select(dt => dt.PushTrackerId).ToList();

            // Assert
            pushDataTypes.Should().HaveCount(2);
            pushDataTypes.Should().Contain(new[] { 1, 5 });

            statusIds.Should().HaveCount(2);
            statusIds.Should().Contain(new[] { 2, 10 });

            pushTrackerIds.Should().HaveCount(2);
            pushTrackerIds.Should().Contain(new[] { 123456789L, 987654321L });
        }

        [Fact]
        public void IDataTracker_ShouldSupportLinqOperations()
        {
            // Arrange
            var dataTrackers = new List<IDataTracker>
            {
                new TestDataTrackerImplementation(),                           // PushTrackerId = 123456789L
                new AlternativeDataTrackerImplementation(),                    // PushTrackerId = 987654321L
                new TestDataTrackerImplementation { StatusId = 20 }           // PushTrackerId = 123456789L
            };

            // Act
            var activeTrackers = dataTrackers.Where(dt => dt.StatusId > 5).ToList();
            var highDataTypeTrackers = dataTrackers.Where(dt => dt.PushDataType >= 5).ToList();
            var largeIdTrackers = dataTrackers.Where(dt => dt.PushTrackerId > 500000000L).ToList();

            // Assert
            activeTrackers.Should().HaveCount(2); // StatusId 10 and 20 (both > 5)
            highDataTypeTrackers.Should().HaveCount(1); // Only AlternativeDataTrackerImplementation has PushDataType = 5
            largeIdTrackers.Should().HaveCount(1); // Only AlternativeDataTrackerImplementation has PushTrackerId = 987654321L > 500000000L
        }

        [Fact]
        public void IDataTracker_ShouldHaveCorrectNamespace()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act & Assert
            interfaceType.Namespace.Should().Be("Aff12B.CronJobs.Core.Domain");
        }

        [Fact]
        public void IDataTracker_ShouldBePublicInterface()
        {
            // Arrange
            var interfaceType = typeof(IDataTracker);

            // Act & Assert
            interfaceType.IsPublic.Should().BeTrue();
            interfaceType.IsInterface.Should().BeTrue();
            interfaceType.IsAbstract.Should().BeTrue(); // Interfaces are abstract by definition
        }

        // Test implementation classes for validation
        private class TestDataTrackerImplementation : IDataTracker
        {
            public int PushDataType { get; init; } = 1;
            public int StatusId { get; init; } = 2;
            public long PushTrackerId { get; init; } = 123456789L;
        }

        private class AlternativeDataTrackerImplementation : IDataTracker
        {
            public int PushDataType => 5;
            public int StatusId => 10;
            public long PushTrackerId => 987654321L;
        }
    }
}
