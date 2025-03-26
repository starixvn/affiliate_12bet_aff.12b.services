using Fanex.Data;
using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetAffCustomersCriteria : CriteriaBase
    {
        private const string SP_NAME = "Aff_CustomerSel";

        public GetAffCustomersCriteria(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public override string GetSettingKey() => SP_NAME;

        public override bool IsValid() => true;

        [SpParam(SqlParamName = "_FromDate")]
        public DateTime FromDate { get; }

        [SpParam(SqlParamName = "_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "_PageIndex")]
        public int PageIndex { get; set; }

        [SpParam(SqlParamName = "_PageSize")]
        public int PageSize { get; }
    }
}