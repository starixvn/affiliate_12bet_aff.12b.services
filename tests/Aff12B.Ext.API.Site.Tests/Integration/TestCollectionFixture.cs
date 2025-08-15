using Xunit;

namespace Aff12B.Ext.API.Site.Tests.Integration
{
    /// <summary>
    /// Collection definition for integration tests to share TestWebApplicationFactory instance
    /// This improves performance by reusing the same application factory across all integration tests
    /// </summary>
    [CollectionDefinition("IntegrationTestCollection")]
    public class TestCollectionFixture : ICollectionFixture<TestWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
