using Aff12B.CronJobs.Core.Domain;
using Aff12B.CronJobs.Infrastructure.Constants;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class InsertDataTrackerCommand(string prefixProvider) : IdenticalSuffixNonQueryCommand(prefixProvider), INonQueryCommand, IDataTracker
    {
        public int PushDataType { get; set; }

        public string PushDataInfo { get; set; }

        public int PushTrackerId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        [SpParam(Ignored = true)]
        public int StatusId => DataTrackerStatus.Success;

        protected override string SuffixSettingKey => "Task_API_PushDataTracker_Import";

        public override bool IsValid()
            => true;
    }
}
