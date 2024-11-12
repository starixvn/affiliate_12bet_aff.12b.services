using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class GetLastedImportTimeCriteria : IdenticalSurfixCriteriaBase, ITrackerSource
    {
        public GetLastedImportTimeCriteria(string prefixProvider = null)
            : base(prefixProvider) { }

        protected override string SuffixSettingKey => "Task_API_TimeTracker_Sel";

        public override bool IsValid() => true;

        public string DataSource { get; set; }
    }
}
