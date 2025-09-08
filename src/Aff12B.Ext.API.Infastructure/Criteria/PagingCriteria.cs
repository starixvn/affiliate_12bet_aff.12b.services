using Fanex.Data;
using Fanex.Data.Repository;

namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public abstract class PagingCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "_PageIndex")]
        public int PageIndex { get; set; }

        [SpParam(SqlParamName = "_PageSize")]
        public int PageSize { get; set; }
    }
}
