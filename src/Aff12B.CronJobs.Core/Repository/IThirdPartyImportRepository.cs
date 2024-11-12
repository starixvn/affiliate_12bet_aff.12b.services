using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Core.Repository
{
    public interface IThirdPartyImportRepository
    {
        Task ImportRecordAsync(IPushDataRecord betRecord);
    }
}
