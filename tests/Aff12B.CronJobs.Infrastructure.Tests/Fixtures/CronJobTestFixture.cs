using Aff12B.Common.Tests;
using Aff12B.CronJobs.Infrastructure.Configurations;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Aff12B.CronJobs.Infrastructure.Tests.Fixtures
{
    /// <summary>
    /// Test fixture for CronJob infrastructure testing providing common setup and services
    /// </summary>
    public class CronJobTestFixture : IDisposable
    {
        public IFixture Fixture { get; }
        public IServiceProvider ServiceProvider { get; }
        private readonly ServiceCollection _services;
        private bool _disposed = false;

        public CronJobTestFixture()
        {
            Fixture = TestUtilities.CreateFixture();
            _services = new ServiceCollection();
            SetupServices();
            ServiceProvider = _services.BuildServiceProvider();
        }

        /// <summary>
        /// Sets up common services for CronJob testing
        /// </summary>
        private void SetupServices()
        {
            // Add logging - disabled for performance
            _services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.None));

            // Add common test services
            _services.AddSingleton(Fixture);

            // Add test configurations
            _services.AddSingleton(CreateTestFetchServiceSetting());
            _services.AddSingleton(CreateTestLicenseeSiteSetting());
            _services.AddSingleton(CreateTestTimeTrackerSetting());
            _services.AddSingleton(CreateTestProxySetting());
        }

        /// <summary>
        /// Creates test FetchServiceSetting
        /// </summary>
        /// <returns>Test FetchServiceSetting</returns>
        private FetchServiceSetting CreateTestFetchServiceSetting()
        {
            return new FetchServiceSetting
            {
                // Add test configuration values as needed
            };
        }

        /// <summary>
        /// Creates test LicenseeSiteSetting
        /// </summary>
        /// <returns>Test LicenseeSiteSetting</returns>
        private LicenseeSiteSetting CreateTestLicenseeSiteSetting()
        {
            return new LicenseeSiteSetting
            {
                // Add test configuration values as needed
            };
        }

        /// <summary>
        /// Creates test TimeTrackerSetting
        /// </summary>
        /// <returns>Test TimeTrackerSetting</returns>
        private TimeTrackerSetting CreateTestTimeTrackerSetting()
        {
            return new TimeTrackerSetting
            {
                // Add test configuration values as needed
            };
        }

        /// <summary>
        /// Creates test ProxySetting
        /// </summary>
        /// <returns>Test ProxySetting</returns>
        private ProxySetting CreateTestProxySetting()
        {
            return new ProxySetting
            {
                // Add test configuration values as needed
            };
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

        /// <summary>
        /// Creates a mock IOptions wrapper for configuration
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="value">Configuration value</param>
        /// <returns>Mock IOptions instance</returns>
        public Mock<IOptions<T>> CreateMockOptions<T>(T value) where T : class
        {
            var mockOptions = new Mock<IOptions<T>>();
            mockOptions.Setup(x => x.Value).Returns(value);
            return mockOptions;
        }

        /// <summary>
        /// Creates a mock IOptionsSnapshot wrapper for configuration
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="value">Configuration value</param>
        /// <returns>Mock IOptionsSnapshot instance</returns>
        public Mock<IOptionsSnapshot<T>> CreateMockOptionsSnapshot<T>(T value) where T : class
        {
            var mockOptionsSnapshot = new Mock<IOptionsSnapshot<T>>();
            mockOptionsSnapshot.Setup(x => x.Value).Returns(value);
            return mockOptionsSnapshot;
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
