using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Aff12B.CronJobs.Infrastructure.Tests.Services
{
    /// <summary>
    /// Unit tests for MasterDataCacheService.
    /// Tests Redis cache operations and error handling.
    /// </summary>
    public class DataCacheServiceTests
    {
        private readonly Mock<IRedisService> _mockRedisService;
        private readonly Mock<ILogger<DataCacheService>> _mockLogger;
        private readonly DataCacheService _service;

        public DataCacheServiceTests()
        {
            _mockRedisService = new Mock<IRedisService>();
            _mockLogger = new Mock<ILogger<DataCacheService>>();
            _service = new DataCacheService(_mockRedisService.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldCreateInstance_WhenValidDependencies()
        {
            // Arrange & Act
            var service = new DataCacheService(_mockRedisService.Object, _mockLogger.Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IDataCacheService>();
        }

        [Theory]
        [InlineData("redisService")]
        [InlineData("logger")]
        public void Constructor_ShouldThrowArgumentNullException_WhenDependencyIsNull(string parameterName)
        {
            // Arrange & Act
            var act = () => parameterName switch
            {
                "redisService" => new DataCacheService(null, _mockLogger.Object),
                "logger" => new DataCacheService(_mockRedisService.Object, null),
                _ => throw new ArgumentException("Invalid parameter name")
            };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName(parameterName);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenRedisUpdateSucceeds()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, masterData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenRedisUpdateFails()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(false);

            // Act
            var result = await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task UpdateAsync_ShouldThrowArgumentException_WhenCacheKeyIsInvalid(string invalidKey)
        {
            // Arrange
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(invalidKey, masterData, expiration);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Cache key cannot be null or empty*");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenMasterDataIsNull()
        {
            // Arrange
            var cacheKey = "test-key";
            var expiration = TimeSpan.FromHours(24);

            // Act & Assert
            var act = async () => await _service.UpdateAsync<MasterInfoValidation>(cacheKey, null, expiration);
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("data");
        }

        [Fact]
        public async Task UpdateAsync_ShouldPropagateException_WhenRedisThrows()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);
            var expectedException = new InvalidOperationException("Redis connection failed");

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, expiration);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Redis connection failed");
        }

        [Theory]
        [InlineData(0)]      // Zero timespan
        [InlineData(-1)]     // Negative seconds
        [InlineData(-60)]    // Negative minutes
        [InlineData(-3600)]  // Negative hours
        public async Task UpdateAsync_ShouldThrowArgumentOutOfRangeException_WhenExpireTimeIsZeroOrNegative(int seconds)
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var invalidExpiration = TimeSpan.FromSeconds(seconds);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, invalidExpiration);
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*Expire time must be positive*")
                .WithParameterName("expireTime");
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotCallRedis_WhenValidationFails()
        {
            // Arrange
            var cacheKey = "test-key";
            var invalidExpiration = TimeSpan.Zero;

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, CreateMockMasterData(), invalidExpiration);
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();

            // Verify Redis was never called due to early validation
            _mockRedisService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldLogWarning_WhenDataIsNull()
        {
            // Arrange
            var cacheKey = "test-key";
            var expiration = TimeSpan.FromHours(24);

            // Act & Assert
            var act = async () => await _service.UpdateAsync<MasterInfoValidation>(cacheKey, null, expiration);
            await act.Should().ThrowAsync<ArgumentNullException>();

            // Verify warning log was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Master data cannot be null for key: {cacheKey}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldLogWarning_WhenExpireTimeIsInvalid()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var invalidExpiration = TimeSpan.FromSeconds(-10);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, invalidExpiration);
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>();

            // Verify warning log was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Invalid expiration time") && v.ToString().Contains(cacheKey)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldLogInformation_WhenUpdateSucceeds()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(true);

            // Act
            await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert - Verify information log was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Updated cache successfully for key: {cacheKey}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldLogWarning_WhenUpdateReturnsFalse()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(false);

            // Act
            await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert - Verify warning log was called
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Cache update returned false for key: {cacheKey}")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);
            var expectedException = new InvalidOperationException("Redis connection failed");

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, expiration);
            await act.Should().ThrowAsync<InvalidOperationException>();

            // Verify error log was called with exception
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Failed to update cache for key: {cacheKey}")),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldWorkWithDifferentDataTypes_StringType()
        {
            // Arrange - Test with different generic type
            var cacheKey = "string-data-key";
            var stringData = "Test string data with unicode: 测试数据";
            var expiration = TimeSpan.FromMinutes(30);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, stringData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, stringData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, stringData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldWorkWithDifferentDataTypes_ListType()
        {
            // Arrange - Test with List type
            var cacheKey = "list-data-key";
            var listData = new List<string> { "Item1", "Item2", "Item3" };
            var expiration = TimeSpan.FromHours(1);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, listData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, listData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, listData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldWorkWithDifferentDataTypes_DictionaryType()
        {
            // Arrange - Test with Dictionary type
            var cacheKey = "dict-data-key";
            var dictData = new Dictionary<string, int> { { "key1", 100 }, { "key2", 200 } };
            var expiration = TimeSpan.FromMinutes(15);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, dictData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, dictData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, dictData, expiration), Times.Once);
        }

        [Theory]
        [InlineData(1)]        // 1 second
        [InlineData(60)]       // 1 minute
        [InlineData(3600)]     // 1 hour
        [InlineData(86400)]    // 1 day
        [InlineData(604800)]   // 1 week
        public async Task UpdateAsync_ShouldAcceptValidExpireTimes(int seconds)
        {
            // Arrange - Test with various valid expiration times
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromSeconds(seconds);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, masterData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleDifferentExceptionTypes_TimeoutException()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);
            var expectedException = new TimeoutException("Redis operation timed out");

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, expiration);
            await act.Should().ThrowAsync<TimeoutException>()
                .WithMessage("Redis operation timed out");
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleDifferentExceptionTypes_UnauthorizedAccessException()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);
            var expectedException = new UnauthorizedAccessException("Redis access denied");

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, expiration);
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Redis access denied");
        }

        [Fact]
        public async Task UpdateAsync_ShouldHandleUnicodeInCacheKey()
        {
            // Arrange - Test internationalization support
            var cacheKey = "master-data:用户数据:VIP会員";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, masterData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldWorkWithComplexCacheKeyPattern()
        {
            // Arrange - Test realistic cache key pattern
            var cacheKey = "aff12b:master-data:landing-pages:v1:2024";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UpdateAsync(cacheKey, masterData, expiration);

            // Assert
            result.Should().BeTrue();
            _mockRedisService.Verify(x => x.SetAsync(cacheKey, masterData, expiration), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowOriginalException_WhenRedisThrowsAggregateException()
        {
            // Arrange
            var cacheKey = "test-key";
            var masterData = CreateMockMasterData();
            var expiration = TimeSpan.FromHours(24);
            var innerException = new InvalidOperationException("Inner exception");
            var expectedException = new AggregateException("Aggregate exception", innerException);

            _mockRedisService.Setup(x => x.SetAsync(cacheKey, masterData, expiration))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.UpdateAsync(cacheKey, masterData, expiration);
            await act.Should().ThrowAsync<AggregateException>();

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to update cache")),
                    expectedException,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Helper Methods

        private MasterInfoValidation CreateMockMasterData()
        {
            return new MasterInfoValidation
            {
                SpecialDomainMappings = new List<SpecialDomainMapping>(),
                DomainMappings = new List<DomainMapping>(),
                LanguageReferences = new List<LanguageReference>(),
                AffCommGroups = new List<AffiliateDeal>(),
                LandingPages = new List<LandingPageSetting>()
            };
        }

        #endregion
    }
}
