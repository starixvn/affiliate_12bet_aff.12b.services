namespace Aff12B.CronJobs.Core.Domain.DTOs
{
    public class DataTrackerItem
    {
        public string PrefixSiteName { get; set; }

        public long PushTrackerId { get; set; }

        public int StatusId { get; set; }

        public int PushDataType { get; set; }

        public string PushDataInfo { get; set; }
    }
}
