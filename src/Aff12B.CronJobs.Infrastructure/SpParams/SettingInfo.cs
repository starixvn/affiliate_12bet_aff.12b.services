using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class SettingInfo : IdenticalSurfixCriteriaBase, ISettingInfo
    {
        public SettingInfo(string prefixProvider) : base(prefixProvider)
        {
        }

        public virtual string SName { get; set; }

        protected override string SuffixSettingKey => "Task_Aff_GetSetting";

        public override bool IsValid() => true;
    }
}
