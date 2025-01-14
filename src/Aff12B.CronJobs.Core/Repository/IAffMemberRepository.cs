using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Core.Repository
{
    public interface IAffMemberRepository
    {
        Task ImportMemberAsync(IAffMemberImportInfo memberInfo);

        Task<DateTime> GetLastedMemberImportedTimeAsync(ISettingInfo setting);

        Task<string> GetSiteNameAsync(ISettingInfo settingInfo);

        Task InsertAffMemberTimeTrackerAsync(IInsertAffMemberTimeTracker timeTracker);
    }
}
