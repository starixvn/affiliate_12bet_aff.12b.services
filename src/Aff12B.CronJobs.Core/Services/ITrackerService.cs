using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.CronJobs.Core.Services
{
    public interface ITrackerService
    {
        Task<TrackerInfo> GetTrackerAsync<TKey>(string source, Func<TrackerInfo, TKey> minKeySelector);

        Task<TrackerInfo> GetTrackerAsync<TKey>(string source, Func<TrackerInfo, TKey> minKeySelector, IComparer<TKey> comparer);

        Task SaveTrackerAsync(string source, TrackerInfo tracker);
    }
}