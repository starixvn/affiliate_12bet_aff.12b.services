using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Core.Tests.Domain.TestDoubles
{
    /// <summary>
    /// Test double implementation of IImportTimeTracker for testing
    /// </summary>
    public class TestImportTimeTracker : IImportTimeTracker
    {
        public string DataSource { get; set; }
        public DateTime CalledTime { get; set; }
        public bool IsReset { get; set; }
        public string KeyValue { get; set; }

        public TestImportTimeTracker()
        {
            DataSource = "TestDataSource";
            CalledTime = DateTime.UtcNow;
            IsReset = false;
            KeyValue = "test-key-value";
        }

        public TestImportTimeTracker(string dataSource, DateTime calledTime, bool isReset, string keyValue)
        {
            DataSource = dataSource;
            CalledTime = calledTime;
            IsReset = isReset;
            KeyValue = keyValue;
        }
    }
}
