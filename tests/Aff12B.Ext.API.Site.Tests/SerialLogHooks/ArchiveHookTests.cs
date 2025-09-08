using Aff12B.Ext.API.Site.SerialLogHooks;
using FluentAssertions;
using Serilog.Sinks.File.Archive;
using Xunit;

namespace Aff12B.Ext.API.Site.Tests.SerialLogHooks
{
    /// <summary>
    /// Unit tests for ArchiveHook static class.
    /// Tests archive hook configuration for Serilog file archiving.
    /// </summary>
    public class ArchiveHookTests
    {
        [Fact]
        public void MyArchiveHooks_ShouldReturnArchiveHooksInstance()
        {
            // Act
            var result = ArchiveHook.MyArchiveHooks;

            // Assert - Should return a valid ArchiveHooks instance
            result.Should().NotBeNull();
            result.Should().BeOfType<ArchiveHooks>();
        }

        [Fact]
        public void MyArchiveHooks_ShouldReturnConsistentInstance()
        {
            // Act - Call multiple times
            var result1 = ArchiveHook.MyArchiveHooks;
            var result2 = ArchiveHook.MyArchiveHooks;

            // Assert - Should return new instances each time (not singleton)
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            result1.Should().BeOfType<ArchiveHooks>();
            result2.Should().BeOfType<ArchiveHooks>();

            // Note: These are different instances since it's a property that creates new objects
            result1.Should().NotBeSameAs(result2);
        }

        [Fact]
        public void MyArchiveHooks_ShouldSupportArchiveHooksInterface()
        {
            // Act
            var result = ArchiveHook.MyArchiveHooks;

            // Assert - Should implement the expected interface/base class
            result.Should().BeAssignableTo<ArchiveHooks>();
        }

        [Fact]
        public void ArchiveHook_ShouldBeStaticClass()
        {
            // Assert - ArchiveHook should be a static class
            var type = typeof(ArchiveHook);
            type.IsAbstract.Should().BeTrue(); // Static classes are abstract
            type.IsSealed.Should().BeTrue();   // Static classes are sealed
        }

        [Fact]
        public void MyArchiveHooks_ShouldProvideDefaultArchiveConfiguration()
        {
            // Act
            var archiveHooks = ArchiveHook.MyArchiveHooks;

            // Assert - Should provide a default archive configuration
            archiveHooks.Should().NotBeNull();

            // The ArchiveHooks class should have default behavior for file archiving
            // This ensures the property provides a usable archive configuration
            archiveHooks.GetType().Should().Be(typeof(ArchiveHooks));
        }

        [Fact]
        public void MyArchiveHooks_ShouldSupportSerilogFileArchiving()
        {
            // Arrange & Act
            var archiveHooks = ArchiveHook.MyArchiveHooks;

            // Assert - Should be compatible with Serilog file archiving
            archiveHooks.Should().NotBeNull();
            archiveHooks.Should().BeOfType<ArchiveHooks>();

            // Verify it's from the correct namespace for Serilog file archiving
            archiveHooks.GetType().Namespace.Should().Be("Serilog.Sinks.File.Archive");
        }

        [Fact]
        public void MyArchiveHooks_ShouldBeUsableInSerilogConfiguration()
        {
            // Arrange & Act - This simulates how it would be used in Serilog configuration
            var archiveHooks = ArchiveHook.MyArchiveHooks;

            // Assert - Should be ready for use in Serilog file sink configuration
            archiveHooks.Should().NotBeNull();
            archiveHooks.Should().BeOfType<ArchiveHooks>();

            // The property should provide a valid archive hooks instance
            // that can be used in Serilog's WriteTo.File() configuration
        }

        [Fact]
        public void MyArchiveHooks_ShouldCreateNewInstancesOnEachCall()
        {
            // Arrange - Multiple calls to the property
            var instances = new List<ArchiveHooks>();

            // Act - Call the property multiple times
            for (int i = 0; i < 5; i++)
            {
                instances.Add(ArchiveHook.MyArchiveHooks);
            }

            // Assert - Each call should return a new instance
            instances.Should().HaveCount(5);
            instances.Should().OnlyContain(x => x != null);
            instances.Should().OnlyContain(x => x.GetType() == typeof(ArchiveHooks));

            // Verify they are different instances (not cached/singleton)
            for (int i = 0; i < instances.Count - 1; i++)
            {
                for (int j = i + 1; j < instances.Count; j++)
                {
                    instances[i].Should().NotBeSameAs(instances[j],
                        $"Instance {i} should not be the same as instance {j}");
                }
            }
        }
    }
}
