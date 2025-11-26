using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Core.Domain.DTOs;
using Aff12B.CronJobs.Core.Repository;
using Aff12B.CronJobs.Infrastructure.SpParams;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.Repository
{
    public class AffInfoRepository(IDynamicRepository dynamicRepository) : IAffInfoRepository
    {
        public async Task<IEnumerable<AffInfo>> GetAffInfosAsync(IPagingInfo settingInfo)
            => await dynamicRepository.FetchAsync<AffInfo>((GetAffInfosCriteria)settingInfo);
    }
}
