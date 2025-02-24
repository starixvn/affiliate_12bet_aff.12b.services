using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetTpWager9WCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey()
            => "API_TpWager9W_Get";

        public override bool IsValid()
            => true;
    }
}
