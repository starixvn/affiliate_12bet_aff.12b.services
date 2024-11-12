using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Core.Services;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;

namespace Aff12B.CronJobs.Infrastructure.Services
{
    public class DataTrackerService : IDataTrackerService
    {
        private readonly IDataTrackerRepository _repository;

        public DataTrackerService(IDataTrackerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DataTrackerItem>> FetchTrackerAsync(IEnumerable<string> prefixProviders, IEnumerable<int> dataTypes, IEnumerable<int> statuses)
        {
            var listTracker = new List<DataTrackerItem>();
            foreach (var prefixProvider in prefixProviders)
            {
                foreach (var dataType in dataTypes)
                {
                    foreach (var status in statuses)
                    {
                        listTracker.AddRange(await FetchAndAssignValue(new DataTrackerCriteria(prefixProvider)
                        {
                            PushDataType = dataType,
                            StatusId = status
                        }, prefixProvider));
                    }
                }
            }
            return listTracker;
        }

        public Task UpdateTrackerStatusAsync(DataTrackerItem item, int status)
        {
            return _repository.UpdateDataTrackerAsync(new UpdateDataTrackerCommand(item.PrefixSiteName)
            {
                StatusId = status,
                PushTrackerId = item.PushTrackerId
            });
        }

        private async Task<IEnumerable<DataTrackerItem>> FetchAndAssignValue(DataTrackerCriteria criteria, string prefixProvider)
        {
            var items = await _repository.FetchDataTracker(criteria);
            foreach (var item in items)
            {
                item.PrefixSiteName = prefixProvider;
            }
            return items;
        }
    }
}
