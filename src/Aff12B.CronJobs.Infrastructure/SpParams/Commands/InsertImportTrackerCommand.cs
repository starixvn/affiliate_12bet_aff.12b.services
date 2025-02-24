using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class InsertImportTrackerCommand(string prefixProvider = null) : IdenticalSuffixNonQueryCommand(prefixProvider), IImportTimeTracker
    {
        protected override string SuffixSettingKey => "Task_API_TimeTracker_Import";

        public override bool IsValid() => true;

        public string DataSource { get; set; }

        public DateTime CalledTime { get; set; }

        public bool IsReset { get; set; }

        public string KeyValue { get; set; } = string.Empty;

        public int ErrCode { get; set; } = 0;

        public string ErrMsg { get; set; } = string.Empty;
    }
}
