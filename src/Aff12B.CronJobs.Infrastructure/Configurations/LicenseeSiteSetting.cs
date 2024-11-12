
namespace Aff12B.CronJobs.Infrastructure.Configurations
{
    public class LicenseeSiteSetting
    {
        public IEnumerable<string> SupportedSites { get; set; }

        public IEnumerable<LicenseeSiteMapping> LicenseeSiteMapping { get; set; }

        public string GetCodePrefix(string site)
        {
            var siteMapping = LicenseeSiteMapping.FirstOrDefault(m => m.SiteName.Equals(site, System.StringComparison.InvariantCultureIgnoreCase));
            return siteMapping == null ? site : siteMapping.SettingCodePrefix;
        }
    }

    public class LicenseeSiteMapping
    {
        public string SiteName { get; set; }

        public string SettingCodePrefix { get; set; }
    }
}
