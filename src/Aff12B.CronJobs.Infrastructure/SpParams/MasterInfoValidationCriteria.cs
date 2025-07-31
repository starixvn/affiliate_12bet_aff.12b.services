namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class MasterInfoValidationCriteria(string prefixProvider) : IdenticalSurfixCriteriaBase(prefixProvider)
    {
        protected override string SuffixSettingKey => "Aff_ClickStatistics_BulkCache";

        public override bool IsValid() => true;
    }
}