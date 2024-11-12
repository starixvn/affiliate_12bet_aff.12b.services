namespace Aff12B.CronJobs.Infrastructure.Configurations
{
    public class TimeTrackerSetting
    {
        public IEnumerable<string> SourcePrefix { get; set; }

        public DateTime DefaultTime { get; set; }
    }
}
