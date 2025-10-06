using Aff12B.CronJobs.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aff12B.CronJobs.Infrastructure.Services
{
    public class DataCacheService : IDataCacheService
    {
        private readonly IRedisService _redisService;
        private readonly ILogger<DataCacheService> _logger;

        public DataCacheService(IRedisService redisService, ILogger<DataCacheService> logger)
        {
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> UpdateAsync<T>(string cacheKey, T data, TimeSpan expireTime) where T : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentException("Cache key cannot be null or empty", nameof(cacheKey));
            }

            if (data == null)
            {
                _logger.LogWarning("Master data cannot be null for key: {CacheKey}", cacheKey);

                throw new ArgumentNullException(nameof(data));
            }

            if (expireTime <= TimeSpan.Zero)
            {
                _logger.LogWarning("Invalid expiration time {ExpireTime} for key: {CacheKey}", expireTime, cacheKey);
                throw new ArgumentOutOfRangeException(nameof(expireTime), "Expire time must be positive.");
            }

            try
            {
                var success = await _redisService.SetAsync(cacheKey, data, expireTime);

                if (success)
                {
                    _logger.LogInformation("Updated cache successfully for key: {CacheKey}", cacheKey);
                }
                else
                {
                    _logger.LogWarning("Cache update returned false for key: {CacheKey}", cacheKey);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cache for key: {CacheKey}", cacheKey);
                throw;
            }
        }
    }
}
