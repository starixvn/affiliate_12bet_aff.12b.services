namespace Aff12B.CronJobs.Core.Domain
{
    public interface IInsertAffMemberTimeTracker : ISettingInfo
    {
        DateTime LastImportedTime { get; }
    }
}
