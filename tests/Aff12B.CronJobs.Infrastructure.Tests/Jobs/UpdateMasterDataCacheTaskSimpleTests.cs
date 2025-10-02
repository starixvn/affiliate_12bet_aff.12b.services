using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using Aff12B.CronJobs.Infrastructure.Configurations;
using Aff12B.CronJobs.Infrastructure.Jobs;
using Aff12B.CronJobs.Infrastructure.Jobs.Interfaces;
using Aff12B.CronJobs.Infrastructure.Services;
using Aff12B.CronJobs.Infrastructure.Tests.Helpers;
using Fanex.Data.Repository;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Jobs
{
    /// <summary>
    /// Simplified unit tests for UpdateMasterDataCacheTask class.
    /// Focuses on core functionality without complex external library mocking.
    /// </summary>
    public class UpdateMasterDataCacheTaskSimpleTests
    {
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly Mock<IDynamicRepository> _mockDynamicRepository;
        private readonly Mock<IOptions<LicenseeSiteSetting>> _mockOptions;
        private readonly Mock<ILogger<UpdateMasterDataCacheTask>> _mockLogger;
        private readonly LicenseeSiteSetting _licenseeSiteSetting;
        private readonly UpdateMasterDataCacheTask _task;

        public UpdateMasterDataCacheTaskSimpleTests()
        {
            _mockRedisService = new Mock<IRedisService>();
            _mockDynamicRepository = new Mock<IDynamicRepository>();
            _mockOptions = new Mock<IOptions<LicenseeSiteSetting>>();
            _mockLogger = new Mock<ILogger<UpdateMasterDataCacheTask>>();

            _licenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new[] { "Site1", "Site2" },
                LicenseeSiteMapping = new[]
                {
                    new LicenseeSiteMapping { SiteName = "Site1", SettingCodePrefix = "S1" },
                    new LicenseeSiteMapping { SiteName = "Site2", SettingCodePrefix = "S2" }
                }
            };

            _mockOptions.Setup(x => x.Value).Returns(_licenseeSiteSetting);

            _task = new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                _mockDynamicRepository.Object,
                _mockOptions.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDependencies()
        {
            // Act & Assert - Constructor should not throw
            var task = new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                _mockDynamicRepository.Object,
                _mockOptions.Object,
                _mockLogger.Object);

            task.Should().NotBeNull();
            task.Should().BeAssignableTo<ICacheUpdateTask>();
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenRedisServiceIsNull()
        {
            // Act & Assert - Should throw ArgumentNullException for null cache service
            Action act = () => new UpdateMasterDataCacheTask(
                null,
                _mockDynamicRepository.Object,
                _mockOptions.Object,
                _mockLogger.Object);

            act.Should().Throw<ArgumentNullException>().WithParameterName("redisService");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenDynamicRepositoryIsNull()
        {
            // Act & Assert - Should throw ArgumentNullException for null repository
            Action act = () => new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                null,
                _mockOptions.Object,
                _mockLogger.Object);

            act.Should().Throw<ArgumentNullException>().WithParameterName("dynamicRepository");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert - Should throw ArgumentNullException for null logger
            Action act = () => new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                _mockDynamicRepository.Object,
                _mockOptions.Object,
                null);

            act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
        }

        [Fact]
        public void MASTER_DATA_CACHE_KEY_TEMPLATE_ShouldHaveCorrectValue()
        {
            // Act - Access the constant through cached reflection
            var field = ReflectionTestHelper.GetCachedProtectedField(typeof(UpdateMasterDataCacheTask), "MASTER_DATA_CACHE_KEY_TEMPLATE");
            var template = (string)field.GetValue(null);

            // Assert - Should have the expected template format
            template.Should().Be("Aff.{0}.Go.MasterData");
        }

        [Fact]
        public void CacheSlidingExpirationInHours_ShouldHaveCorrectValue()
        {
            // Act - Access the constant through cached reflection
            var field = ReflectionTestHelper.GetCachedProtectedField(typeof(UpdateMasterDataCacheTask), "CacheSlidingExpirationInHours");
            var expiration = (int)field.GetValue(null);

            // Assert - Should have 24 hours expiration
            expiration.Should().Be(24);
        }

        [Fact]
        public void SupportedSites_Property_ShouldReturnLicenseeSiteSettingSupportedSites()
        {
            // Act - Access the SupportedSites property through cached reflection
            var property = ReflectionTestHelper.GetCachedProtectedProperty(typeof(UpdateMasterDataCacheTask), "SupportedSites");
            var supportedSites = (IEnumerable<string>)property.GetValue(_task);

            // Assert - Should return the supported sites from the license setting
            supportedSites.Should().BeEquivalentTo(new[] { "Site1", "Site2" });
        }

        [Fact]
        public async Task UpdateMasterDataCacheAsync_ShouldReturn_WhenNoSupportedSites()
        {
            // Arrange
            var emptyLicenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = new string[0]
            };

            var mockEmptyOptions = new Mock<IOptions<LicenseeSiteSetting>>();
            mockEmptyOptions.Setup(x => x.Value).Returns(emptyLicenseeSiteSetting);

            var taskWithEmptySites = new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                _mockDynamicRepository.Object,
                mockEmptyOptions.Object,
                _mockLogger.Object);

            // Act
            await taskWithEmptySites.UpdateMasterDataCacheAsync();

            // Assert - Should not call any cache methods
            _mockRedisService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task UpdateMasterDataCacheAsync_ShouldReturn_WhenSupportedSitesIsNull()
        {
            // Arrange
            var nullLicenseeSiteSetting = new LicenseeSiteSetting
            {
                SupportedSites = null
            };

            var mockNullOptions = new Mock<IOptions<LicenseeSiteSetting>>();
            mockNullOptions.Setup(x => x.Value).Returns(nullLicenseeSiteSetting);

            var taskWithNullSites = new UpdateMasterDataCacheTask(
                _mockRedisService.Object,
                _mockDynamicRepository.Object,
                mockNullOptions.Object,
                _mockLogger.Object);

            // Act
            await taskWithNullSites.UpdateMasterDataCacheAsync();

            // Assert - Should not call any cache methods
            _mockRedisService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldHaveCorrectHangfireAttributes()
        {
            // Act - Get the UpdateMasterDataCacheAsync method
            var updateMethod = typeof(UpdateMasterDataCacheTask).GetMethod("UpdateMasterDataCacheAsync");

            // Assert - Should have the correct Hangfire attributes
            var disableConcurrentExecutionAttr = updateMethod.GetCustomAttributes(typeof(DisableConcurrentExecutionAttribute), false);
            var automaticRetryAttr = updateMethod.GetCustomAttributes(typeof(AutomaticRetryAttribute), false);

            disableConcurrentExecutionAttr.Should().HaveCount(1);
            automaticRetryAttr.Should().HaveCount(1);

            var retryAttr = (AutomaticRetryAttribute)automaticRetryAttr[0];
            retryAttr.Attempts.Should().Be(1);
            retryAttr.OnAttemptsExceeded.Should().Be(AttemptsExceededAction.Fail);
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldImplementICacheUpdateTask()
        {
            // Act & Assert - Should implement the correct interface
            _task.Should().BeAssignableTo<ICacheUpdateTask>();
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldBeInCorrectNamespace()
        {
            // Act & Assert - Should be in the correct namespace
            _task.GetType().Namespace.Should().Be("Aff12B.CronJobs.Infrastructure.Jobs");
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldHaveCorrectConstructorParameters()
        {
            // Act - Check constructor parameters
            var constructors = typeof(UpdateMasterDataCacheTask).GetConstructors();
            var publicConstructor = constructors.FirstOrDefault(c => c.IsPublic);

            // Assert - Should have a public constructor with the expected parameters
            publicConstructor.Should().NotBeNull();
            var parameters = publicConstructor.GetParameters();
            parameters.Should().HaveCount(4);
            parameters[0].ParameterType.Should().Be<IRedisService>();
            parameters[1].ParameterType.Should().Be<IDynamicRepository>();
            parameters[2].ParameterType.Should().Be<IOptions<LicenseeSiteSetting>>();
            parameters[3].ParameterType.Should().Be<ILogger<UpdateMasterDataCacheTask>>();
        }

        [Fact]
        public void UpdateMasterDataCacheAsync_ShouldHaveCorrectMethodSignature()
        {
            // Act - Get the UpdateMasterDataCacheAsync method
            var updateMethod = typeof(UpdateMasterDataCacheTask).GetMethod("UpdateMasterDataCacheAsync");

            // Assert - Should have the correct method signature
            updateMethod.Should().NotBeNull();
            updateMethod.ReturnType.Should().Be<Task>();
            updateMethod.GetParameters().Should().BeEmpty();
            updateMethod.IsPublic.Should().BeTrue();
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldHavePrivateHelperMethods()
        {
            // Act - Check for private helper methods
            var updateCacheForSiteAsyncMethod = typeof(UpdateMasterDataCacheTask)
                .GetMethod("UpdateCacheForSiteAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var getMasterDataAsyncMethod = typeof(UpdateMasterDataCacheTask)
                .GetMethod("GetMasterDataAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert - Should have the expected private methods
            updateCacheForSiteAsyncMethod.Should().NotBeNull();
            getMasterDataAsyncMethod.Should().NotBeNull();

            updateCacheForSiteAsyncMethod.ReturnType.Should().Be<Task>();
            getMasterDataAsyncMethod.ReturnType.Should().Be<Task<MasterInfoValidation>>();
        }

        [Fact]
        public void UpdateMasterDataCacheTask_ShouldHaveCorrectFieldTypes()
        {
            // Act - Check private fields
            var RedisServiceField = typeof(UpdateMasterDataCacheTask)
                .GetField("_redisService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dynamicRepositoryField = typeof(UpdateMasterDataCacheTask)
                .GetField("_dynamicRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var loggerField = typeof(UpdateMasterDataCacheTask)
                .GetField("_logger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var licenseeSiteSettingField = typeof(UpdateMasterDataCacheTask)
                .GetField("_licenseeSiteSetting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert - Should have the expected field types
            RedisServiceField.Should().NotBeNull();
            RedisServiceField.FieldType.Should().Be<IRedisService>();

            dynamicRepositoryField.Should().NotBeNull();
            dynamicRepositoryField.FieldType.Should().Be<IDynamicRepository>();

            loggerField.Should().NotBeNull();
            loggerField.FieldType.Should().Be<ILogger<UpdateMasterDataCacheTask>>();

            licenseeSiteSettingField.Should().NotBeNull();
            licenseeSiteSettingField.FieldType.Should().Be<LicenseeSiteSetting>();
        }
    }
}
