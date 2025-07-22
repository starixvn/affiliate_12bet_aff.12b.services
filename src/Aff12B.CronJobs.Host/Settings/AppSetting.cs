namespace Aff12B.CronJobs.Host.Settings
{
    public class AppSetting
    {
        public string LicenseeDataAPI { get; set; }

        public bool IsApiDataModifiable { get; set; }

        public IEnumerable<string> SupportDBPrefix { get; set; }
    }
}
