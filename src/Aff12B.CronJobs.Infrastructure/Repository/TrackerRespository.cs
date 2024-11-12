using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Aff12B.CronJobs.Infrastructure.SpParams.Commands;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class TrackerRespository : ITrackerRepository
    {
        private readonly IDynamicRepository _repo;

        public TrackerRespository(IDynamicRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Task<TrackerInfo> GetLastedTrackerAsync(ITrackerSource timeInfo)
            => _repo.GetAsync<TrackerInfo>((GetLastedImportTimeCriteria)timeInfo);


        public Task ImportTimeTrackerAsync(IImportTimeTracker timeInfo)
            => _repo.ExecuteAsync((InsertImportTrackerCommand)timeInfo);
    }
}
