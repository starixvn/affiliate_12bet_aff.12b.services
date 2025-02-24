using Aff12B.CronJobs.Core.Domain;
using Fanex.Data;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class DataTrackerCriteria(string prefixProvider) : IdenticalSurfixCriteriaBase(prefixProvider), IDataTracker
    {
        protected override string SuffixSettingKey => "Task_API_PushDataTracker_Sel";

        public int PushDataType { get; set; }

        public int StatusId { get; set; }

        [SpParam(Ignored = true)]
        public long PushTrackerId => default;

        public override bool IsValid()
            => true;
    }
}
