using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetDepositInfoCriteria : CriteriaBase
    {
        public GetDepositInfoCriteria(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public override string GetSettingKey()
            => "API_DepositInfo_Get";

        public override bool IsValid()
            => true;

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
