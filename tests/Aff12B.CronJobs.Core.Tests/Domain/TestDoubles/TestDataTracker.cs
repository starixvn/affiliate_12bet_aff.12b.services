using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Core.Tests.Domain.TestDoubles
{
    /// <summary>
    /// Test double implementation of IDataTracker for testing
    /// </summary>
    public class TestDataTracker : IDataTracker
    {
        public int PushDataType { get; set; }
        public int StatusId { get; set; }
        public long PushTrackerId { get; set; }

        public TestDataTracker()
        {
            PushDataType = 1;
            StatusId = 1;
            PushTrackerId = 12345;
        }

        public TestDataTracker(int pushDataType, int statusId, long pushTrackerId)
        {
            PushDataType = pushDataType;
            StatusId = statusId;
            PushTrackerId = pushTrackerId;
        }
    }
}
