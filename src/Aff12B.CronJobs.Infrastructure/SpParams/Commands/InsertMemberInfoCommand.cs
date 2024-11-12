using Aff12B.CronJobs.Core.Domain;
using Fanex.Data;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class InsertMemberInfoCommand : IdenticalSuffixNonQueryCommand, IAffMemberImportInfo
    {
        public InsertMemberInfoCommand(string prefixProvider) : base(prefixProvider)
        { }

        [SpParam(Ignored = true)]
        public string SettingPrefix { get; set; }

        [SpParam(Ignored = true)]
        public string SiteName { get; set; }

        public string XmlData { get; set; }

        public int ErrCode { get; set; } = 0;

        public string ErrMsg { get; set; } = string.Empty;

        protected override string SuffixSettingKey => "Task_Aff_MemberInfo_Import";

        public override bool IsValid() => true;
    }
}
