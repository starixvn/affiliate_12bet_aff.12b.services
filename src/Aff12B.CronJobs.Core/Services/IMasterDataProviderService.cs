using Aff12B.CronJobs.Core.Domain.DTOs.AffLink;

namespace Aff12B.CronJobs.Core.Services
{
    public interface IMasterDataProviderService
    {
        Task<MasterInfoValidation> GetMasterDataAsync(string sitePrefix);
    }
}
