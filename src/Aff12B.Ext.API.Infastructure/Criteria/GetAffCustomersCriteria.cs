namespace Aff12B.Ext.API.Infastructure.Criteria
{
    public class GetAffCustomersCriteria : PagingCriteria
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
    }
}