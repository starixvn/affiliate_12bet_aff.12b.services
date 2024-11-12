using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams.Commands
{
    public class InsertThirdPartyRecodInfoCommand : IdenticalSuffixNonQueryCommand, IPushDataRecord
    {
        public InsertThirdPartyRecodInfoCommand(string prefixProvider = null) : base(prefixProvider)
        {
        }

        protected override string SuffixSettingKey => "Task_API_Queue_Import";
        public override bool IsValid() => true;

        public string DataSource { get; set; }

        public string XmlData { get; set; }

        public int ErrCode { get; set; } = 0;

        public string ErrMsg { get; set; } = string.Empty;
    }
}
