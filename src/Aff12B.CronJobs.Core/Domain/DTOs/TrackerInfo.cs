

namespace Aff12B.CronJobs.Core.Domain.DTOs
{
    public class TrackerInfo
    {
        public DateTime CalledTime { get; set; }

        public string KeyValue { get; set; }

        public bool IsReset { get; set; }
    }
}
