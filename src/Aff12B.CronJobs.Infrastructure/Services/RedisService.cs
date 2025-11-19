using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.Converters;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace Aff12B.CronJobs.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase datatabse;
        private readonly AsyncRetryPolicy retryPolicy;
        private const int RetryCount = 3;

        public RedisService(IConnectionMultiplexer redis)
        {
            datatabse = redis.GetDatabase();
            retryPolicy = Policy
                .Handle<RedisConnectionException>()
                .Or<RedisTimeoutException>()
                .Or<RedisServerException>(ex =>
                    ex.Message.Contains("READONLY"))
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(1),
                    (_, _, _, _) =>
                    {
                        // Retry logic handled by Polly
                    });
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await retryPolicy.ExecuteAsync(async () =>
                await datatabse.KeyDeleteAsync(key)
              );
        }

        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expireTime = null)
        {
            var redisValue = RedisTypeConverter.ToRedisValue(value);
            return await retryPolicy.ExecuteAsync(async () =>
                await datatabse.StringSetAsync(key, redisValue, expireTime));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await retryPolicy.ExecuteAsync(async () =>
            {
                var redisValue = await datatabse.StringGetAsync(key);
                T value = default;

                if (!redisValue.IsNull)
                {
                    value = RedisTypeConverter.ToObject<T>(redisValue);
                }

                return value;
            });
        }
    }
}
