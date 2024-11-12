namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class MemberImportedTimeInfo : SettingInfo
    {
        public MemberImportedTimeInfo(string prefixProvider) : base(prefixProvider)
        {
        }

        public override string SName => "MemberInfoLastImportedTime";
    }
}