using Aff12B.CronJobs.Core.Domain;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class UpdateDataTrackerCommand : IdenticalSuffixNonQueryCommand, INonQueryCommand, IDataTracker
    {
        public UpdateDataTrackerCommand(string prefixProvider) : base(prefixProvider)
        { }

        public int PushTrackerId { get; set; }

        public int StatusId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        [SpParam(Ignored = true)]
        public int PushDataType => default;

        protected override string SuffixSettingKey => "Task_API_PushDataTracker_Upd";

        public override bool IsValid()
            => true;
    }
}
