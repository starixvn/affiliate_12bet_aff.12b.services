namespace Aff12B.CronJobs.Core.Domain
{
    public interface IDataTracker
    {
        int PushDataType { get; }

        int StatusId { get; }

        int PushTrackerId { get; }
    }
}
