namespace Aff12B.CronJobs.Infrastructure.Services
{
    public interface IRedisService
    {
        Task<bool> RemoveAsync(string key);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expireTime = null);

        Task<T> GetAsync<T>(string key);
    }
}
