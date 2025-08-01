using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class InsertAffMemberTimeTrackerCommand(string prefixProvider, string spName) : IdenticalSuffixNonQueryCommand(prefixProvider), IInsertAffMemberTimeTracker
    {
        public DateTime LastImportedTime { get; set; }

        public string SName => spName;

        protected override string SuffixSettingKey => "Task_Aff_MemberInfo_ImportedTime";

        public override bool IsValid() => true;

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; } = string.Empty;
    }
}
