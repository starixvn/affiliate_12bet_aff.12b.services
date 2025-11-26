using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;

namespace Aff12B.CronJobs.Core.Repository
{
    public interface IAffInfoRepository
    {
        Task<IEnumerable<AffInfo>> GetAffInfosAsync(IPagingInfo settingInfo);
    }
}
