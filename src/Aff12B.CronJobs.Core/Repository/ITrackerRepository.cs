using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.CronJobs.Core.Repository
{
    public interface ITrackerRepository
    {
        Task<TrackerInfo> GetLastedTrackerAsync(ITrackerSource timeInfo);

        Task ImportTimeTrackerAsync(IImportTimeTracker timeInfo);
    }
}
