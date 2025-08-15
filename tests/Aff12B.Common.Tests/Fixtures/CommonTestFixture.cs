using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aff12B.Common.Tests.Fixtures
{
    /// <summary>
    /// Common test fixture providing shared test infrastructure
    /// </summary>
    public class CommonTestFixture : IDisposable
    {
        public IFixture Fixture { get; }
        public IServiceProvider ServiceProvider { get; }
        private readonly ServiceCollection _services;
        private bool _disposed = false;

        public CommonTestFixture()
        {
            Fixture = TestUtilities.CreateFixture();
            _services = new ServiceCollection();
            SetupServices();
            ServiceProvider = _services.BuildServiceProvider();
        }

        /// <summary>
        /// Sets up common services for testing
        /// </summary>
        private void SetupServices()
        {
            // Add logging - disabled for performance
            _services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.None));

            // Add common test services
            _services.AddSingleton(Fixture);
            _services.AddSingleton(TestUtilities.CreateTestTokenManagement());
            _services.AddSingleton(TestUtilities.CreateTestUsers());
        }

        /// <summary>
        /// Gets a service from the test service provider
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Gets a required service from the test service provider
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        public T GetRequiredService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Creates a mock of the specified type
        /// </summary>
        /// <typeparam name="T">Type to mock</typeparam>
        /// <returns>Mock instance</returns>
        public Mock<T> CreateMock<T>() where T : class
        {
            return new Mock<T>();
        }

        /// <summary>
        /// Creates a mock logger for the specified type
        /// </summary>
        /// <typeparam name="T">Type for which to create the logger</typeparam>
        /// <returns>Mock logger instance</returns>
        public Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return TestUtilities.CreateMockLogger<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (ServiceProvider is IDisposable disposableServiceProvider)
                    {
                        disposableServiceProvider.Dispose();
                    }
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
