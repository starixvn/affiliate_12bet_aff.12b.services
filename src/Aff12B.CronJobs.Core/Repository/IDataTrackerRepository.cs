using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.CronJobs.Core.Repository
{
    public interface IDataTrackerRepository
    {
        Task UpdateDataTrackerAsync(IDataTracker command);

        Task<IEnumerable<DataTrackerItem>> FetchDataTracker(IDataTracker criteria);
    }
}
