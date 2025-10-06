namespace Aff12B.CronJobs.Core.Services
{
    public interface IRedisService
    {
        Task<bool> RemoveAsync(string key);

        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expireTime = null);

        Task<T> GetAsync<T>(string key);
    }
}
