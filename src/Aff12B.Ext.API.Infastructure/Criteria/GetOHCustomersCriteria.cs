namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetOHCustomersCriteria : PagingCriteria
    {
        private const string SP_NAME = "OH_CustomerSel";

        public GetOHCustomersCriteria(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize)
        {
            FromDate = fromDate;
            ToDate = toDate;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public override string GetSettingKey() => SP_NAME;

        public override bool IsValid() => true;
    }
}
