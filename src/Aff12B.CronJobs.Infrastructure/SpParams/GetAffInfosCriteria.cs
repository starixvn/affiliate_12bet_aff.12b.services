using Aff12B.CronJobs.Core.Domain;
using Fanex.Data;

namespace Aff12B.CronJobs.Infrastructure.SpParams
{
    public class GetAffInfosCriteria(string prefixProvider) : IdenticalSurfixCriteriaBase(prefixProvider), IPagingInfo
    {
        [SpParam(Ignored = true)]
        public string SettingPrefix { get; set; }

        public int DealId { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        protected override string SuffixSettingKey => "Aff_API_AffiliateDetailInfo_Sel";

        public override bool IsValid() => true;
    }
}
