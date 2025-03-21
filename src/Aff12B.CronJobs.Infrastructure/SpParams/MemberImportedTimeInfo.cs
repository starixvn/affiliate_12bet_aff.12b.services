namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class MemberImportedTimeInfo(string prefixProvider, string sName) : SettingInfo(prefixProvider)
    {
        public override string SName => sName;
    }
}