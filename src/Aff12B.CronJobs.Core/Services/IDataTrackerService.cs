using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.CronJobs.Core.Services
{
    public interface IDataTrackerService
    {
        Task<IEnumerable<DataTrackerItem>> FetchTrackerAsync(IEnumerable<string> prefixProviders, IEnumerable<int> dataTypes, IEnumerable<int> statuses);

        Task UpdateTrackerStatusAsync(DataTrackerItem item, int status);

        Task InsertDataTrackerAsync(IDataTracker command);
    }
}
