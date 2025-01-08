namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class SiteNameInfo(string prefixProvider) : SettingInfo(prefixProvider)
    {
        public override string SName => "BrandName";
    }
}