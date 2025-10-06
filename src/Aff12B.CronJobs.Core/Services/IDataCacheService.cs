namespace Aff12B.CronJobs.Core.Services
{
    public interface IDataCacheService
    {
        Task<bool> UpdateAsync<T>(string cacheKey, T data, TimeSpan expireTime)
            where T : class;
    }
}
