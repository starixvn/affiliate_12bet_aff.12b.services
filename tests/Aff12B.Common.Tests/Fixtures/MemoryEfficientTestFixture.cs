using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aff12B.Common.Tests.Fixtures
{
    /// <summary>
    /// Memory-efficient test fixture that creates minimal resources and properly disposes them
    /// Use this for tests that don't require heavy service providers
    /// </summary>
    public class MemoryEfficientTestFixture : IDisposable
    {
        private static readonly Lazy<IFixture> _lazyFixture = new(() => TestUtilities.CreateFixture());

        /// <summary>
        /// Shared fixture instance to reduce memory usage
        /// </summary>
        public IFixture Fixture => _lazyFixture.Value;

        private bool _disposed = false;

        /// <summary>
        /// Creates a minimal mock logger without service provider overhead
        /// </summary>
        /// <typeparam name="T">Type for which to create the logger</typeparam>
        /// <returns>Mock logger instance</returns>
        public Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return TestUtilities.CreateMockLogger<T>();
        }

        /// <summary>
        /// Creates a minimal service collection for lightweight testing
        /// </summary>
        /// <returns>Service collection with minimal services</returns>
        public ServiceCollection CreateMinimalServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.None));
            return services;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // No heavy resources to dispose in this lightweight fixture
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
