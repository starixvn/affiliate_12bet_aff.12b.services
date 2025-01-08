namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class MemberImportedTimeInfo(string prefixProvider) : SettingInfo(prefixProvider)
    {
        public override string SName => "MemberInfoLastImportedTime";
    }
}