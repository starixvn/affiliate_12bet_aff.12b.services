using Aff12B.CronJobs.Core.Domain;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class SettingInfo(string prefixProvider) : IdenticalSurfixCriteriaBase(prefixProvider), ISettingInfo
    {
        public virtual string SName { get; set; }

        protected override string SuffixSettingKey => "Task_Aff_GetSetting";

        public override bool IsValid() => true;
    }
}
