using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetTpBetRecordCriteria : CriteriaBase
    {
        public GetTpBetRecordCriteria(DateTime fromDate, DateTime toDate, int pageIdex, int pageSize)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageIndex = pageIdex;
            PageSize = pageSize;
        }

        public override string GetSettingKey() => "API_Get_TpBetRecord_Dynamic";

        public override bool IsValid() => true;

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
