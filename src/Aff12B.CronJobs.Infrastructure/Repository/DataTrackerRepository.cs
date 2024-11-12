


using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class DataTrackerRepository : IDataTrackerRepository
    {
        private readonly IDynamicRepository _repo;

        public DataTrackerRepository(IDynamicRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<IEnumerable<DataTrackerItem>> FetchDataTracker(IDataTracker criteria)
           => _repo.FetchAsync<DataTrackerItem>((ICriteria)criteria);

        public Task UpdateDataTrackerAsync(IDataTracker command)
            => _repo.ExecuteAsync((UpdateDataTrackerCommand)command);
    }
}
